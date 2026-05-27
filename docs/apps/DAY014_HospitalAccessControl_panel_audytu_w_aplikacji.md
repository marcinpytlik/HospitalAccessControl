# DAY014 — HospitalAccessControl  
## Panel audytu w aplikacji

## Cel dnia

Celem DAY014 jest dodanie panelu audytu w aplikacji Razor Pages.

Po DAY013 audyt zapisuje już pełniejsze informacje:

```text
PatientId
RequestedPatientId
WasSuccessful
AdditionalInfo
```

Teraz dodamy stronę:

```text
/Audit
```

która pokaże ostatnie zdarzenia dostępu do danych pacjentów oraz osobną sekcję nieudanych prób.

---

## Efekt końcowy DAY014

Po zakończeniu tego dnia powinieneś mieć:

- `AccessLogListItemDto`,
- `IAuditReadService`,
- `AuditReadService`,
- Razor Page `/Audit`,
- link `Audyt` w menu,
- widok ostatnich zdarzeń,
- widok nieudanych prób,
- testy ręczne generowania audytu,
- gotowy materiał demonstracyjny.

---

# Krok 1 — przejście do katalogu projektu

```powershell
Set-Location C:\Projects\HospitalAccessControl
dotnet build
```

---

# Krok 2 — sprawdzenie danych audytu

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, RequestedPatientId, WasSuccessful, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Jeżeli tabela jest pusta, wejdź kilka razy na:

```text
/Patients/Details/1
/Patients/Details/11
/Patients/Details/9999
```

---

# Krok 3 — utworzenie plików Application

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Audit\AccessLogListItemDto.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Audit\IAuditReadService.cs
```

---

# Krok 4 — AccessLogListItemDto

Wklej do:

```text
src\HospitalAccessControl.Application\Audit\AccessLogListItemDto.cs
```

```csharp
namespace HospitalAccessControl.Application.Audit;

public sealed class AccessLogListItemDto
{
    public long AccessLogId { get; init; }

    public string DomainLogin { get; init; } = string.Empty;

    public int? PatientId { get; init; }

    public int? RequestedPatientId { get; init; }

    public string ActionCode { get; init; } = string.Empty;

    public string ObjectName { get; init; } = string.Empty;

    public DateTime AccessDate { get; init; }

    public string? ClientHost { get; init; }

    public string? ApplicationName { get; init; }

    public bool WasSuccessful { get; init; }

    public string? AdditionalInfo { get; init; }
}
```

---

# Krok 5 — IAuditReadService

Wklej do:

```text
src\HospitalAccessControl.Application\Audit\IAuditReadService.cs
```

```csharp
namespace HospitalAccessControl.Application.Audit;

public interface IAuditReadService
{
    Task<IReadOnlyList<AccessLogListItemDto>> GetLatestAsync(
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccessLogListItemDto>> GetFailedAttemptsAsync(
        int take = 100,
        CancellationToken cancellationToken = default);
}
```

---

# Krok 6 — AuditReadService

Utwórz plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Audit\AuditReadService.cs
```

Wklej:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Audit;

public sealed class AuditReadService : IAuditReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public AuditReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AccessLogListItemDto>> GetLatestAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessLogs
            .AsNoTracking()
            .OrderByDescending(x => x.AccessLogId)
            .Take(take)
            .Select(x => new AccessLogListItemDto
            {
                AccessLogId = x.AccessLogId,
                DomainLogin = x.DomainLogin,
                PatientId = x.PatientId,
                RequestedPatientId = x.RequestedPatientId,
                ActionCode = x.ActionCode,
                ObjectName = x.ObjectName,
                AccessDate = x.AccessDate,
                ClientHost = x.ClientHost,
                ApplicationName = x.ApplicationName,
                WasSuccessful = x.WasSuccessful,
                AdditionalInfo = x.AdditionalInfo
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccessLogListItemDto>> GetFailedAttemptsAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessLogs
            .AsNoTracking()
            .Where(x => !x.WasSuccessful)
            .OrderByDescending(x => x.AccessLogId)
            .Take(take)
            .Select(x => new AccessLogListItemDto
            {
                AccessLogId = x.AccessLogId,
                DomainLogin = x.DomainLogin,
                PatientId = x.PatientId,
                RequestedPatientId = x.RequestedPatientId,
                ActionCode = x.ActionCode,
                ObjectName = x.ObjectName,
                AccessDate = x.AccessDate,
                ClientHost = x.ClientHost,
                ApplicationName = x.ApplicationName,
                WasSuccessful = x.WasSuccessful,
                AdditionalInfo = x.AdditionalInfo
            })
            .ToListAsync(cancellationToken);
    }
}
```

---

# Krok 7 — rejestracja DI

W pliku:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

dodaj:

```csharp
services.AddScoped<IAuditReadService, AuditReadService>();
```

Sprawdź using:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Infrastructure.Audit;
```

---

# Krok 8 — utworzenie Razor Page

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Web\Pages\Audit
New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Audit\Index.cshtml
New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Audit\Index.cshtml.cs
```

---

# Krok 9 — Audit/Index.cshtml.cs

```csharp
using HospitalAccessControl.Application.Audit;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Audit;

public class IndexModel : PageModel
{
    private readonly IAuditReadService _auditReadService;

    public IndexModel(IAuditReadService auditReadService)
    {
        _auditReadService = auditReadService;
    }

    public IReadOnlyList<AccessLogListItemDto> LatestEvents { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public IReadOnlyList<AccessLogListItemDto> FailedAttempts { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        LatestEvents = await _auditReadService.GetLatestAsync(100, cancellationToken);
        FailedAttempts = await _auditReadService.GetFailedAttemptsAsync(50, cancellationToken);
    }
}
```

---

# Krok 10 — Audit/Index.cshtml

```cshtml
@page
@model HospitalAccessControl.Web.Pages.Audit.IndexModel

@{
    ViewData["Title"] = "Audyt";
}

<h1>Audyt dostępu</h1>

<p>
    Panel pokazuje zdarzenia dostępu do danych pacjentów.
</p>

<div class="alert alert-info">
    <strong>Ostatnie zdarzenia:</strong> @Model.LatestEvents.Count
    <br />
    <strong>Nieudane próby:</strong> @Model.FailedAttempts.Count
</div>

<h2>Nieudane próby dostępu</h2>

@if (Model.FailedAttempts.Count == 0)
{
    <div class="alert alert-success">
        Brak nieudanych prób dostępu.
    </div>
}
else
{
    <table class="table table-striped table-bordered">
        <thead>
            <tr>
                <th>Id</th>
                <th>Użytkownik</th>
                <th>PatientId</th>
                <th>RequestedPatientId</th>
                <th>Akcja</th>
                <th>Data UTC</th>
                <th>Informacja</th>
            </tr>
        </thead>
        <tbody>
        @foreach (var item in Model.FailedAttempts)
        {
            <tr>
                <td>@item.AccessLogId</td>
                <td>@item.DomainLogin</td>
                <td>@item.PatientId</td>
                <td>@item.RequestedPatientId</td>
                <td>@item.ActionCode</td>
                <td>@item.AccessDate</td>
                <td>@item.AdditionalInfo</td>
            </tr>
        }
        </tbody>
    </table>
}

<h2>Ostatnie zdarzenia</h2>

<table class="table table-striped table-bordered">
    <thead>
        <tr>
            <th>Id</th>
            <th>Użytkownik</th>
            <th>PatientId</th>
            <th>RequestedPatientId</th>
            <th>Akcja</th>
            <th>Sukces</th>
            <th>Data UTC</th>
            <th>Host</th>
            <th>Aplikacja</th>
            <th>Info</th>
        </tr>
    </thead>
    <tbody>
    @foreach (var item in Model.LatestEvents)
    {
        <tr>
            <td>@item.AccessLogId</td>
            <td>@item.DomainLogin</td>
            <td>@item.PatientId</td>
            <td>@item.RequestedPatientId</td>
            <td>@item.ActionCode</td>
            <td>@item.WasSuccessful</td>
            <td>@item.AccessDate</td>
            <td>@item.ClientHost</td>
            <td>@item.ApplicationName</td>
            <td>@item.AdditionalInfo</td>
        </tr>
    }
    </tbody>
</table>
```

---

# Krok 11 — link w menu

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Shared\_Layout.cshtml
```

Dodaj:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Audit/Index">Audyt</a>
</li>
```

---

# Krok 12 — build i uruchomienie

```powershell
dotnet build
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Wejdź na:

```text
/Audit
```

---

# Krok 13 — testy

Wygeneruj zdarzenia:

```text
/Patients/Details/1     jako doctor.cardio
/Patients/Details/11    jako doctor.cardio
/Patients/Details/9999  jako doctor.cardio
```

Potem wejdź:

```text
/Audit
```

Oczekiwane:

```text
sekcja Nieudane próby zawiera RequestedPatientId = 11 i 9999
sekcja Ostatnie zdarzenia pokazuje wszystkie próby
```

---

# Krok 14 — kontrola SQL

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (20) AccessLogId, DomainLogin, PatientId, RequestedPatientId, WasSuccessful, AdditionalInfo FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

---

# Krok 15 — commit

```powershell
git status
git add .
git commit -m "DAY014 Add audit panel"
```

---

# Kontrola końcowa DAY014

```text
[ ] Utworzono AccessLogListItemDto
[ ] Utworzono IAuditReadService
[ ] Utworzono AuditReadService
[ ] Zarejestrowano IAuditReadService
[ ] Utworzono /Audit
[ ] Dodano link Audyt w menu
[ ] Panel pokazuje ostatnie zdarzenia
[ ] Panel pokazuje nieudane próby
[ ] Panel pokazuje RequestedPatientId
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — brak danych w panelu

Najpierw wygeneruj zdarzenia przez `/Patients/Details/{id}`.

## Problem 2 — DI error

Sprawdź:

```csharp
services.AddScoped<IAuditReadService, AuditReadService>();
```

## Problem 3 — brak RequestedPatientId

Wróć do DAY013 i sprawdź migrację.

---

# Efekt końcowy DAY014

Aplikacja ma panel audytu, który pozwala pokazać próby dostępu do danych pacjentów.

Następny etap:

```text
DAY015 — Role aplikacyjne i ograniczenie funkcji
```
