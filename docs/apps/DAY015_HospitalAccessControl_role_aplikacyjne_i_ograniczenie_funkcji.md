# DAY015 — HospitalAccessControl  
## Role aplikacyjne i ograniczenie funkcji

## Cel dnia

Celem DAY015 jest dodanie kontroli funkcji aplikacyjnych na podstawie ról użytkownika.

Do tej pory mieliśmy bardzo mocną ochronę danych na poziomie SQL Server:

```text
SESSION_CONTEXT
Row-Level Security
```

To zabezpiecza rekordy pacjentów.

Teraz dodajemy drugi poziom:

```text
kontrola funkcji w aplikacji
```

Czyli:

```text
kto może wejść do panelu audytu,
kto może widzieć dane pacjentów,
kto ma funkcje techniczne.
```

---

## Efekt końcowy DAY015

Po zakończeniu tego dnia aplikacja będzie umiała:

- pobrać role aktualnego użytkownika z bazy,
- dołączyć role do `CurrentUserDto`,
- pokazać role na stronie głównej,
- ograniczyć wejście do `/Audit`,
- przygotować fundament pod dalszą kontrolę funkcji,
- zachować RLS jako główne zabezpieczenie danych.

---

## Ważna zasada

Role aplikacyjne nie zastępują RLS.

Role aplikacyjne odpowiadają za:

```text
dostęp do funkcji
```

RLS odpowiada za:

```text
dostęp do rekordów danych
```

Przykład:

```text
Auditor może wejść do /Audit
Auditor nie musi widzieć danych medycznych pacjentów
```

---

# Krok 1 — przejście do katalogu projektu

```powershell
Set-Location C:\Projects\HospitalAccessControl
dotnet build
```

---

# Krok 2 — sprawdzenie ról w bazie

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT u.DomainLogin, r.Code AS RoleCode FROM security.ApplicationUsers u INNER JOIN security.UserRoleAssignments ura ON ura.ApplicationUserId = u.ApplicationUserId INNER JOIN dictionary.ApplicationRoles r ON r.ApplicationRoleId = ura.ApplicationRoleId ORDER BY u.ApplicationUserId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT u.DomainLogin, r.Code AS RoleCode FROM security.ApplicationUsers u INNER JOIN security.UserRoleAssignments ura ON ura.ApplicationUserId = u.ApplicationUserId INNER JOIN dictionary.ApplicationRoles r ON r.ApplicationRoleId = ura.ApplicationRoleId ORDER BY u.ApplicationUserId;"
```

Oczekiwane role:

```text
doctor.cardio       Doctor
nurse.cardio        Nurse
registration.user   Registration
manager.cardio      DepartmentManager
auditor.user        Auditor
it.admin            ITAdministrator
```

---

# Krok 3 — rozszerzenie CurrentUserDto

Otwórz:

```text
src\HospitalAccessControl.Application\Common\Security\CurrentUserDto.cs
```

Zmień zawartość na:

```csharp
namespace HospitalAccessControl.Application.Common.Security;

public sealed class CurrentUserDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public string SamAccountName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public bool IsAuthenticated { get; init; }

    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    public bool HasRole(string roleCode)
    {
        return Roles.Contains(roleCode, StringComparer.OrdinalIgnoreCase);
    }
}
```

---

# Krok 4 — utworzenie IUserRoleReadService

Utwórz plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Common\Security\IUserRoleReadService.cs
```

Wklej:

```csharp
namespace HospitalAccessControl.Application.Common.Security;

public interface IUserRoleReadService
{
    Task<IReadOnlyList<string>> GetRoleCodesAsync(
        string domainLogin,
        CancellationToken cancellationToken = default);
}
```

---

# Krok 5 — implementacja UserRoleReadService

Utwórz plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Security\UserRoleReadService.cs
```

Wklej:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Security;

public sealed class UserRoleReadService : IUserRoleReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public UserRoleReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<string>> GetRoleCodesAsync(
        string domainLogin,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainLogin))
        {
            return Array.Empty<string>();
        }

        return await _dbContext.UserRoleAssignments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.ValidFrom <= DateTime.UtcNow)
            .Where(x => x.ValidTo == null || x.ValidTo >= DateTime.UtcNow)
            .Where(x => x.ApplicationUser.IsActive)
            .Where(x => x.ApplicationUser.DomainLogin == domainLogin)
            .Where(x => x.ApplicationRole.IsActive)
            .Select(x => x.ApplicationRole.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }
}
```

---

# Krok 6 — rejestracja DI

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Dodaj:

```csharp
services.AddScoped<IUserRoleReadService, UserRoleReadService>();
```

Jeżeli brakuje using:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.Security;
```

---

# Krok 7 — aktualizacja DevelopmentCurrentUserService

Otwórz:

```text
src\HospitalAccessControl.Web\Services\DevelopmentCurrentUserService.cs
```

Zmień całość na:

```csharp
using HospitalAccessControl.Application.Common.Security;
using Microsoft.Extensions.Options;

namespace HospitalAccessControl.Web.Services;

public sealed class DevelopmentCurrentUserService : ICurrentUserService
{
    private readonly DevelopmentUserOptions _options;
    private readonly IUserRoleReadService _userRoleReadService;

    public DevelopmentCurrentUserService(
        IOptions<DevelopmentUserOptions> options,
        IUserRoleReadService userRoleReadService)
    {
        _options = options.Value;
        _userRoleReadService = userRoleReadService;
    }

    public CurrentUserDto GetCurrentUser()
    {
        if (string.IsNullOrWhiteSpace(_options.DomainLogin))
        {
            return new CurrentUserDto
            {
                DomainLogin = string.Empty,
                SamAccountName = string.Empty,
                DisplayName = "Anonymous",
                IsAuthenticated = false,
                Roles = Array.Empty<string>()
            };
        }

        var roles = _userRoleReadService
            .GetRoleCodesAsync(_options.DomainLogin)
            .GetAwaiter()
            .GetResult();

        return new CurrentUserDto
        {
            DomainLogin = _options.DomainLogin,
            SamAccountName = _options.SamAccountName,
            DisplayName = _options.DisplayName,
            IsAuthenticated = true,
            Roles = roles
        };
    }
}
```

---

# Krok 8 — pokazanie ról na stronie głównej

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Index.cshtml
```

W tabeli aktualnego użytkownika dodaj:

```cshtml
<tr>
    <th>Role</th>
    <td>@string.Join(", ", Model.CurrentUser.Roles)</td>
</tr>
```

---

# Krok 9 — ograniczenie panelu audytu

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Audit\Index.cshtml.cs
```

Zmień na:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Audit;

public class IndexModel : PageModel
{
    private readonly IAuditReadService _auditReadService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(
        IAuditReadService auditReadService,
        ICurrentUserService currentUserService)
    {
        _auditReadService = auditReadService;
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public bool AccessDenied { get; private set; }

    public IReadOnlyList<AccessLogListItemDto> LatestEvents { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public IReadOnlyList<AccessLogListItemDto> FailedAttempts { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        if (!CurrentUser.HasRole("Auditor") &&
            !CurrentUser.HasRole("ITAdministrator"))
        {
            AccessDenied = true;
            return Page();
        }

        LatestEvents = await _auditReadService.GetLatestAsync(100, cancellationToken);
        FailedAttempts = await _auditReadService.GetFailedAttemptsAsync(50, cancellationToken);

        return Page();
    }
}
```

---

# Krok 10 — komunikat w widoku audytu

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Audit\Index.cshtml
```

Pod nagłówkiem dodaj:

```cshtml
@if (Model.AccessDenied)
{
    <div class="alert alert-danger">
        Brak uprawnień do panelu audytu.
    </div>

    return;
}
```

---

# Krok 11 — build

```powershell
dotnet build
```

---

# Krok 12 — test doctor.cardio

W `appsettings.Development.json` ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

Uruchom aplikację:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Sprawdź `/`.

Oczekiwane role:

```text
Doctor
```

Wejdź na:

```text
/Audit
```

Oczekiwane:

```text
Brak uprawnień do panelu audytu.
```

---

# Krok 13 — test auditor.user

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\auditor.user",
  "SamAccountName": "auditor.user",
  "DisplayName": "Alicja Audytor"
}
```

Oczekiwane role:

```text
Auditor
```

Wejdź na:

```text
/Audit
```

Oczekiwane:

```text
panel audytu widoczny
```

---

# Krok 14 — test it.admin

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\it.admin",
  "SamAccountName": "it.admin",
  "DisplayName": "Adam Administrator IT"
}
```

Oczekiwane role:

```text
ITAdministrator
```

`/Audit` powinien działać.

`/Patients` powinien nadal pokazywać 0 pacjentów, bo RLS nie daje IT dostępu do oddziałów.

---

# Krok 15 — commit

```powershell
git status
git add .
git commit -m "DAY015 Add application role checks"
```

---

# Kontrola końcowa DAY015

```text
[ ] CurrentUserDto ma Roles
[ ] CurrentUserDto ma HasRole()
[ ] Dodano IUserRoleReadService
[ ] Dodano UserRoleReadService
[ ] Zarejestrowano IUserRoleReadService
[ ] DevelopmentCurrentUserService pobiera role
[ ] Strona główna pokazuje role
[ ] /Audit blokuje doctor.cardio
[ ] /Audit pozwala auditor.user
[ ] /Audit pozwala it.admin
[ ] it.admin nadal nie widzi pacjentów
[ ] dotnet build działa
```

---

# Najczęstsze problemy

## Problem 1 — role są puste

Sprawdź, czy użytkownik istnieje w `security.ApplicationUsers` i ma wpis w `security.UserRoleAssignments`.

## Problem 2 — błąd DI

Sprawdź:

```csharp
services.AddScoped<IUserRoleReadService, UserRoleReadService>();
```

## Problem 3 — /Audit dostępny dla wszystkich

Sprawdź `Audit/Index.cshtml.cs`, czy warunek roli jest wykonywany przed odczytem audytu.

---

# Efekt końcowy DAY015

Aplikacja rozpoznaje role i ogranicza funkcje aplikacyjne.

Następny etap:

```text
DAY016 — Dynamic Data Masking dla PESEL
```
