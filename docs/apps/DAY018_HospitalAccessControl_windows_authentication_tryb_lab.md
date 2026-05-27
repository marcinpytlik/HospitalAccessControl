# DAY018 — HospitalAccessControl  
## Windows Authentication — tryb LAB

## Cel dnia

Celem DAY018 jest przygotowanie aplikacji do pracy z Windows Authentication w środowisku domenowym.

Do tej pory aplikacja działała w trybie:

```text
Development
```

czyli użytkownik był symulowany przez `appsettings.Development.json`.

Teraz dodajemy tryb:

```text
Windows
```

w którym użytkownik zostanie odczytany z:

```csharp
HttpContext.User.Identity.Name
```

---

## Efekt końcowy DAY018

Po zakończeniu dnia powinieneś mieć:

- `WindowsCurrentUserService`,
- obsługę `SecurityMode = Windows`,
- konfigurację `AddNegotiate`,
- middleware `UseAuthentication` i `UseAuthorization`,
- instrukcję IIS,
- test w środowisku domenowym.

---

# Krok 1 — paczka Negotiate

```powershell
dotnet list .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj package
```

Jeżeli brakuje:

```powershell
dotnet add .\src\HospitalAccessControl.Web package Microsoft.AspNetCore.Authentication.Negotiate
```

---

# Krok 2 — WindowsCurrentUserService

Utwórz plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Services\WindowsCurrentUserService.cs
```

Wklej:

```csharp
using HospitalAccessControl.Application.Common.Security;

namespace HospitalAccessControl.Web.Services;

public sealed class WindowsCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRoleReadService _userRoleReadService;

    public WindowsCurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUserRoleReadService userRoleReadService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRoleReadService = userRoleReadService;
    }

    public CurrentUserDto GetCurrentUser()
    {
        var identityName = _httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .Name;

        if (string.IsNullOrWhiteSpace(identityName))
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

        var samAccountName = identityName.Contains('\\')
            ? identityName.Split('\\')[1]
            : identityName;

        var roles = _userRoleReadService
            .GetRoleCodesAsync(identityName)
            .GetAwaiter()
            .GetResult();

        return new CurrentUserDto
        {
            DomainLogin = identityName,
            SamAccountName = samAccountName,
            DisplayName = identityName,
            IsAuthenticated = true,
            Roles = roles
        };
    }
}
```

---

# Krok 3 — Program.cs

Otwórz:

```text
src\HospitalAccessControl.Web\Program.cs
```

Dodaj using:

```csharp
using Microsoft.AspNetCore.Authentication.Negotiate;
```

Dodaj usługi:

```csharp
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization();
```

Zmień wybór `ICurrentUserService`:

```csharp
var securityMode = builder.Configuration["SecurityMode"];

if (string.Equals(securityMode, "Development", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else if (string.Equals(securityMode, "Windows", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, WindowsCurrentUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported SecurityMode: '{securityMode}'.");
}
```

Po:

```csharp
var app = builder.Build();
```

dodaj przed mapowaniem Razor Pages:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

---

# Krok 4 — appsettings.Lab.json

Utwórz:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\appsettings.Lab.json
```

Wklej:

```json
{
  "SecurityMode": "Windows",
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=SQL01;Database=HospitalAccessControlDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

# Krok 5 — test lokalny

Lokalnie poza domeną możesz zobaczyć:

```text
KOMPUTER\blad
```

To normalne.

W pełni testujemy na:

```text
CLIENT01 -> APP01 -> SQL01 -> DC01
```

---

# Krok 6 — IIS

Na APP01:

```text
Anonymous Authentication = Disabled
Windows Authentication = Enabled
```

Application Pool:

```text
HOSPITAL\svc_hac_app
```

---

# Krok 7 — test domenowy

Zaloguj się na CLIENT01 jako:

```text
HOSPITAL\doctor.cardio
```

Otwórz aplikację.

Oczekiwane na stronie głównej:

```text
DomainLogin = HOSPITAL\doctor.cardio
SESSION_CONTEXT = HOSPITAL\doctor.cardio
Roles = Doctor
```

---

# Krok 8 — test RLS

`/Patients` jako doctor.cardio:

```text
10 pacjentów CARD
```

`/Patients` jako doctor.ortho:

```text
10 pacjentów ORTH
```

---

# Krok 9 — commit

```powershell
dotnet build
git status
git add .
git commit -m "DAY018 Add Windows Authentication mode"
```

---

# Kontrola końcowa DAY018

```text
[ ] Dodano WindowsCurrentUserService
[ ] Dodano AddNegotiate
[ ] Dodano UseAuthentication
[ ] Dodano UseAuthorization
[ ] SecurityMode obsługuje Windows
[ ] Tryb Development nadal działa
[ ] appsettings.Lab.json istnieje
[ ] Opisano konfigurację IIS
```

---

# Efekt końcowy DAY018

Aplikacja jest gotowa do pracy z Windows Authentication.

Następny etap:

```text
DAY019 — Publikacja na APP01 / IIS
```
