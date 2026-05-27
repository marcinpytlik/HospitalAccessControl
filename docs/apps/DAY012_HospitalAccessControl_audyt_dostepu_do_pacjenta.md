# DAY012 — HospitalAccessControl  
## Audyt dostępu do pacjenta

## Cel dnia

Celem DAY012 jest dodanie audytu aplikacyjnego dla dostępu do szczegółów pacjenta.

Po zakończeniu tego dnia aplikacja będzie zapisywać wpis do tabeli:

```text
audit.AccessLog
```

za każdym razem, gdy użytkownik spróbuje wejść na stronę:

```text
/Patients/Details/{id}
```

Chcemy zapisywać zarówno:

```text
udany dostęp do pacjenta
```

jak i:

```text
próbę wejścia do pacjenta, który nie istnieje albo jest niewidoczny przez RLS
```

---

## Efekt końcowy DAY012

Po zakończeniu tego dnia powinieneś mieć:

- DTO do zapisu audytu,
- interfejs `IAuditService`,
- implementację `AuditService`,
- rejestrację audytu w DI,
- zapis zdarzenia przy wejściu w szczegóły pacjenta,
- wpisy w tabeli `audit.AccessLog`,
- testy dla użytkowników:
  - `doctor.cardio`,
  - `doctor.ortho`,
  - `it.admin`,
  - `unknown.user`,
- możliwość pokazania audytu w SQL Server.

---

## Dlaczego audyt jest ważny?

Sama kontrola dostępu nie wystarczy.

W systemie medycznym ważne jest również:

```text
kto próbował zobaczyć dane pacjenta,
kiedy próbował,
czy operacja się udała,
jakiego pacjenta dotyczyła,
z jakiego użytkownika aplikacyjnego wykonano próbę.
```

Dzięki temu możemy później odpowiedzieć na pytania:

```text
Kto oglądał dane pacjenta?
Czy ktoś próbował wejść do danych bez dostępu?
Czy administrator techniczny próbował podejrzeć dane medyczne?
Czy użytkownik próbował ręcznie wpisać identyfikator pacjenta?
```

To jest bardzo dobry element projektu bezpieczeństwa.

---

# 1. Co będziemy audytować?

W DAY012 audytujemy akcję:

```text
ViewPatientDetails
```

Czyli wejście na stronę:

```text
/Patients/Details/{id}
```

Będziemy zapisywać:

```text
DomainLogin
PatientId
ActionCode
ObjectName
AccessDate
ClientHost
ApplicationName
WasSuccessful
AdditionalInfo
```

---

# 2. Jak interpretujemy sukces i porażkę?

## Udany dostęp

Przykład:

```text
HOSPITAL\doctor.cardio
wchodzi na /Patients/Details/1
pacjent jest z CARD
RLS pozwala zobaczyć rekord
```

W audycie:

```text
WasSuccessful = true
PatientId = 1
ActionCode = ViewPatientDetails
```

---

## Nieudany dostęp

Przykład:

```text
HOSPITAL\doctor.cardio
wchodzi ręcznie na /Patients/Details/11
pacjent jest z ORTH
RLS ukrywa rekord
```

Aplikacja dostaje:

```text
Patient = null
```

W audycie:

```text
WasSuccessful = false
PatientId = 11
ActionCode = ViewPatientDetails
AdditionalInfo = Patient not found or access denied
```

---

# 3. Ważna decyzja bezpieczeństwa

Nie rozróżniamy w UI:

```text
pacjent nie istnieje
pacjent istnieje, ale brak dostępu
```

Komunikat pozostaje:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

Ale w audycie zapisujemy próbę dostępu.

To pozwala później analizować podejrzane zachowania bez ujawniania informacji użytkownikowi.

---

# Krok 1 — przejście do katalogu projektu

Otwórz PowerShell i przejdź do katalogu projektu:

```powershell
Set-Location C:\Projects\HospitalAccessControl
```

Sprawdź lokalizację:

```powershell
Get-Location
```

Oczekiwany wynik:

```text
C:\Projects\HospitalAccessControl
```

Sprawdź build po DAY011:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 2 — sprawdzenie tabeli audit.AccessLog

Sprawdź, czy tabela istnieje.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT s.name AS SchemaName, t.name AS TableName FROM sys.tables t INNER JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = N'audit' AND t.name = N'AccessLog';"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT s.name AS SchemaName, t.name AS TableName FROM sys.tables t INNER JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = N'audit' AND t.name = N'AccessLog';"
```

Oczekiwany wynik:

```text
audit  AccessLog
```

---

# Krok 3 — utworzenie katalogów audytu

Tworzymy katalog w Application:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Application\Audit
```

Tworzymy katalog w Infrastructure:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Audit
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application
Get-ChildItem .\src\HospitalAccessControl.Infrastructure
```

---

# Krok 4 — utworzenie plików audytu

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Audit\AccessLogCreateDto.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Audit\IAuditService.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Audit\AuditService.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application\Audit
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Audit
```

---

# Krok 5 — AccessLogCreateDto

## Plik

```text
src\HospitalAccessControl.Application\Audit\AccessLogCreateDto.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Audit;

public sealed class AccessLogCreateDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public int? PatientId { get; init; }

    public string ActionCode { get; init; } = string.Empty;

    public string ObjectName { get; init; } = string.Empty;

    public string? ClientHost { get; init; }

    public string? ApplicationName { get; init; }

    public bool WasSuccessful { get; init; }

    public string? AdditionalInfo { get; init; }
}
```

---

## Po co DTO?

Nie chcemy, aby warstwa Web tworzyła bezpośrednio encję:

```text
AccessLog
```

Lepiej, aby Web przekazał tylko dane zdarzenia do serwisu aplikacyjnego:

```text
IAuditService
```

A szczegóły zapisu do bazy zostaną w Infrastructure.

---

# Krok 6 — IAuditService

## Plik

```text
src\HospitalAccessControl.Application\Audit\IAuditService.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Audit;

public interface IAuditService
{
    Task LogAccessAsync(
        AccessLogCreateDto accessLog,
        CancellationToken cancellationToken = default);
}
```

---

## Co robi interfejs?

Na razie ma jedną metodę:

```text
LogAccessAsync
```

W przyszłości możemy dodać:

```text
GetAccessLogsAsync
GetPatientAccessHistoryAsync
GetFailedAccessAttemptsAsync
```

---

# Krok 7 — AuditService

## Plik

```text
src\HospitalAccessControl.Infrastructure\Audit\AuditService.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Domain.Entities;
using HospitalAccessControl.Infrastructure.Data;

namespace HospitalAccessControl.Infrastructure.Audit;

public sealed class AuditService : IAuditService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public AuditService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAccessAsync(
        AccessLogCreateDto accessLog,
        CancellationToken cancellationToken = default)
    {
        var entity = new AccessLog
        {
            DomainLogin = accessLog.DomainLogin,
            PatientId = accessLog.PatientId,
            ActionCode = accessLog.ActionCode,
            ObjectName = accessLog.ObjectName,
            AccessDate = DateTime.UtcNow,
            ClientHost = accessLog.ClientHost,
            ApplicationName = accessLog.ApplicationName,
            WasSuccessful = accessLog.WasSuccessful,
            AdditionalInfo = accessLog.AdditionalInfo
        };

        _dbContext.AccessLogs.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
```

---

## Ważna uwaga

`AccessDate` ustawiamy jako:

```csharp
DateTime.UtcNow
```

Dlaczego UTC?

Bo w systemach technicznych i audytowych lepiej zapisywać czas w UTC.

W UI można go później przeliczać na lokalny czas.

---

# Krok 8 — rejestracja AuditService w DI

Otwórz plik:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Infrastructure.Audit;
```

W metodzie `AddInfrastructure`, przed:

```csharp
return services;
```

dodaj:

```csharp
services.AddScoped<IAuditService, AuditService>();
```

Przykład końcówki metody:

```csharp
services.AddScoped<ISqlSessionContextDiagnostics, SqlSessionContextDiagnostics>();
services.AddScoped<IPatientReadService, PatientReadService>();
services.AddScoped<IAuditService, AuditService>();

return services;
```

---

# Krok 9 — modyfikacja Details.cshtml.cs

Teraz dodamy zapis audytu przy wejściu w szczegóły pacjenta.

Otwórz plik:

```text
src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml.cs
```

Zastąp zawartość pliku:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class DetailsModel : PageModel
{
    private const string ViewPatientDetailsAction = "ViewPatientDetails";

    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public DetailsModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public PatientDetailsDto? Patient { get; private set; }

    public bool AccessDeniedOrNotFound { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        Patient = await _patientReadService.GetPatientDetailsAsync(
            id,
            cancellationToken);

        var wasSuccessful = Patient is not null;

        if (!wasSuccessful)
        {
            AccessDeniedOrNotFound = true;
        }

        await _auditService.LogAccessAsync(
            new AccessLogCreateDto
            {
                DomainLogin = CurrentUser.DomainLogin,
                PatientId = id,
                ActionCode = ViewPatientDetailsAction,
                ObjectName = "medical.Patients",
                ClientHost = HttpContext.Connection.RemoteIpAddress?.ToString(),
                ApplicationName = "HospitalAccessControl.Web",
                WasSuccessful = wasSuccessful,
                AdditionalInfo = wasSuccessful
                    ? "Patient details viewed."
                    : "Patient not found or access denied."
            },
            cancellationToken);

        return Page();
    }
}
```

---

## Co się zmieniło?

Dodaliśmy:

```text
IAuditService
```

oraz zapis:

```csharp
await _auditService.LogAccessAsync(...)
```

Dla udanego wejścia:

```text
WasSuccessful = true
```

Dla nieudanego:

```text
WasSuccessful = false
```

---

# Krok 10 — build po dodaniu audytu

Wykonaj:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 11 — sprawdzenie początkowej liczby logów

Przed testem sprawdź liczbę rekordów w audycie.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS AccessLogCount FROM audit.AccessLog;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS AccessLogCount FROM audit.AccessLog;"
```

Na początku może być:

```text
0
```

albo więcej, jeżeli testowałeś wcześniej.

---

# Krok 12 — uruchomienie aplikacji

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz:

```text
https://localhost:xxxx/Patients
```

---

# Krok 13 — test 1: udany dostęp doctor.cardio

Ustaw w:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

użytkownika:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

Uruchom aplikację.

Wejdź na:

```text
/Patients/Details/1
```

Oczekiwany wynik w UI:

```text
szczegóły pacjenta są widoczne
```

Teraz sprawdź audyt.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Oczekiwany wpis:

```text
DomainLogin = HOSPITAL\doctor.cardio
PatientId = 1
ActionCode = ViewPatientDetails
WasSuccessful = 1
AdditionalInfo = Patient details viewed.
```

---

# Krok 14 — test 2: nieudany dostęp doctor.cardio do pacjenta ORTH

Dalej jako:

```text
HOSPITAL\doctor.cardio
```

wejdź ręcznie na:

```text
/Patients/Details/11
```

Oczekiwany wynik w UI:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

Sprawdź audyt:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Oczekiwany wpis:

```text
DomainLogin = HOSPITAL\doctor.cardio
PatientId = 11
ActionCode = ViewPatientDetails
WasSuccessful = 0
AdditionalInfo = Patient not found or access denied.
```

---

# Krok 15 — test 3: it.admin próbuje wejść do pacjenta

Zmień użytkownika:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\it.admin",
  "SamAccountName": "it.admin",
  "DisplayName": "Adam Administrator IT"
}
```

Uruchom aplikację i wejdź na:

```text
/Patients/Details/1
```

Oczekiwany wynik:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

Sprawdź audyt.

Oczekiwany wpis:

```text
DomainLogin = HOSPITAL\it.admin
PatientId = 1
WasSuccessful = 0
```

To jest bardzo dobry test pokazujący, że administrator techniczny nie ma automatycznego dostępu do danych medycznych.

---

# Krok 16 — test 4: unknown.user

Zmień użytkownika:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\unknown.user",
  "SamAccountName": "unknown.user",
  "DisplayName": "Nieznany użytkownik"
}
```

Wejdź na:

```text
/Patients/Details/1
```

Oczekiwany wynik:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

W audycie:

```text
DomainLogin = HOSPITAL\unknown.user
PatientId = 1
WasSuccessful = 0
```

---

# Krok 17 — zapytanie podsumowujące audyt

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT DomainLogin, WasSuccessful, COUNT(*) AS EventsCount FROM audit.AccessLog GROUP BY DomainLogin, WasSuccessful ORDER BY DomainLogin, WasSuccessful;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT DomainLogin, WasSuccessful, COUNT(*) AS EventsCount FROM audit.AccessLog GROUP BY DomainLogin, WasSuccessful ORDER BY DomainLogin, WasSuccessful;"
```

Przykładowy wynik:

```text
HOSPITAL\doctor.cardio   0   1
HOSPITAL\doctor.cardio   1   1
HOSPITAL\it.admin        0   1
HOSPITAL\unknown.user    0   1
```

---

# Krok 18 — zapytanie: tylko nieudane próby

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (20) AccessLogId, DomainLogin, PatientId, ActionCode, AdditionalInfo, AccessDate FROM audit.AccessLog WHERE WasSuccessful = 0 ORDER BY AccessLogId DESC;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (20) AccessLogId, DomainLogin, PatientId, ActionCode, AdditionalInfo, AccessDate FROM audit.AccessLog WHERE WasSuccessful = 0 ORDER BY AccessLogId DESC;"
```

To zapytanie jest bardzo dobre do demonstracji na obronie.

---

# Krok 19 — zapis wyników audytu do pliku

Możesz zapisać wynik do pliku:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (50) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;" -o .\sql\09_tests\21_access_log_results.txt
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (50) AccessLogId, DomainLogin, PatientId, ActionCode, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;" -o .\sql\09_tests\21_access_log_results.txt
```

Sprawdź:

```powershell
Get-Content .\sql\09_tests\21_access_log_results.txt
```

Ten plik możesz trzymać jako artefakt testowy.

---

# Krok 20 — opcjonalny plik SQL do repo

Warto dodać plik z zapytaniami audytowymi.

Utwórz:

```powershell
New-Item -ItemType File -Force .\sql\09_tests\21_test_access_log.sql
```

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    ActionCode,
    ObjectName,
    WasSuccessful,
    AdditionalInfo,
    AccessDate,
    ClientHost,
    ApplicationName
FROM audit.AccessLog
ORDER BY AccessLogId DESC;
GO

SELECT
    DomainLogin,
    WasSuccessful,
    COUNT(*) AS EventsCount
FROM audit.AccessLog
GROUP BY
    DomainLogin,
    WasSuccessful
ORDER BY
    DomainLogin,
    WasSuccessful;
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    ActionCode,
    AdditionalInfo,
    AccessDate
FROM audit.AccessLog
WHERE WasSuccessful = 0
ORDER BY AccessLogId DESC;
GO
```

Uruchom:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\21_test_access_log.sql
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\21_test_access_log.sql
```

---

# Krok 21 — build końcowy

Wykonaj:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 22 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY012 Add patient access audit logging"
```

---

# Kontrola końcowa DAY012

Lista kontrolna:

```text
[ ] Sprawdzono istnienie tabeli audit.AccessLog
[ ] Utworzono katalog Application\Audit
[ ] Utworzono katalog Infrastructure\Audit
[ ] Utworzono AccessLogCreateDto
[ ] Utworzono IAuditService
[ ] Utworzono AuditService
[ ] Zarejestrowano IAuditService w DI
[ ] Details.cshtml.cs zapisuje audyt
[ ] Udany dostęp zapisuje WasSuccessful = true
[ ] Nieudany dostęp zapisuje WasSuccessful = false
[ ] doctor.cardio /Patients/Details/1 zapisuje sukces
[ ] doctor.cardio /Patients/Details/11 zapisuje nieudaną próbę
[ ] it.admin /Patients/Details/1 zapisuje nieudaną próbę
[ ] unknown.user /Patients/Details/1 zapisuje nieudaną próbę
[ ] Utworzono opcjonalnie 21_test_access_log.sql
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — IAuditService nie jest zarejestrowany

Objaw:

```text
Unable to resolve service for type 'HospitalAccessControl.Application.Audit.IAuditService'
```

Rozwiązanie:

Sprawdź `ServiceCollectionExtensions.cs`.

Powinno być:

```csharp
services.AddScoped<IAuditService, AuditService>();
```

oraz `using`:

```csharp
using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Infrastructure.Audit;
```

---

## Problem 2 — audyt nie zapisuje się do tabeli

Sprawdź, czy metoda `LogAccessAsync` wykonuje:

```csharp
_dbContext.AccessLogs.Add(entity);
await _dbContext.SaveChangesAsync(cancellationToken);
```

Sprawdź, czy `Details.cshtml.cs` wywołuje:

```csharp
await _auditService.LogAccessAsync(...)
```

---

## Problem 3 — błąd FK do pacjenta przy nieudanym dostępie

W naszej tabeli `AccessLog.PatientId` ma relację do `Patients`.

Jeżeli użytkownik próbuje wejść do pacjenta, który istnieje fizycznie w bazie, ale jest ukryty przez RLS, zapis z `PatientId` powinien działać, bo rekord istnieje.

Jeżeli jednak wpiszesz kompletnie nieistniejący identyfikator, np.:

```text
/Patients/Details/9999
```

i relacja FK wymaga istniejącego pacjenta, zapis audytu z `PatientId = 9999` może się nie udać.

Najprostsze rozwiązanie na tym etapie:

W `Details.cshtml.cs` ustaw `PatientId` tak:

```csharp
PatientId = Patient?.PatientId
```

zamiast:

```csharp
PatientId = id
```

Wtedy przy braku rekordu zapisze się `NULL`.

Alternatywnie można przebudować audyt tak, aby miał pole:

```text
RequestedPatientId
```

bez FK. To będzie lepsze projektowo, ale wymaga migracji.

Na teraz zostajemy przy prostszym wariancie.

---

## Problem 4 — nieudany dostęp do istniejącego pacjenta zapisuje błąd FK

Jeżeli RLS ukrywa pacjenta, ale FK blokuje zapis audytu, bo aplikacja nie widzi pacjenta przez RLS, to zastosuj bezpieczniejszy wariant:

W `Details.cshtml.cs` zmień:

```csharp
PatientId = id,
```

na:

```csharp
PatientId = Patient?.PatientId,
```

Wtedy:

```text
udany dostęp -> zapisze PatientId
nieudany dostęp -> PatientId = NULL
```

A `AdditionalInfo` nadal zawiera informację:

```text
Patient not found or access denied.
```

To jest bezpieczne i zgodne z aktualnym modelem bazy.

---

## Problem 5 — chcesz zapisywać RequestedPatientId dla nieudanych prób

To jest bardzo dobra poprawka projektowa, ale zrobimy ją później jako osobny dzień, np.:

```text
DAY013 — Rozbudowa audytu o RequestedPatientId i panel audytu
```

Wtedy dodamy do `AccessLog` pole:

```text
RequestedPatientId
```

i migrację.

---

# Efekt końcowy DAY012

Po zakończeniu DAY012 aplikacja zapisuje audyt dostępu do szczegółów pacjenta.

Mamy teraz pełny przepływ:

```text
użytkownik DEV
        ↓
SESSION_CONTEXT
        ↓
RLS
        ↓
szczegóły pacjenta
        ↓
audit.AccessLog
```

Możemy pokazać:

```text
kto widział pacjenta,
kto próbował wejść bez dostępu,
kiedy próba miała miejsce,
czy próba była udana.
```

Następny etap może być jednym z dwóch wariantów:

```text
DAY013 — Panel audytu w aplikacji
```

albo:

```text
DAY013 — Rozbudowa audytu o RequestedPatientId
```

Rekomendacja:

```text
DAY013 — Rozbudowa audytu o RequestedPatientId
DAY014 — Panel audytu w aplikacji
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach dwunastego etapu implementacji dodano mechanizm audytu dostępu do szczegółów pacjenta. Utworzono interfejs `IAuditService`, obiekt `AccessLogCreateDto` oraz implementację `AuditService`, która zapisuje zdarzenia do tabeli `audit.AccessLog`. Strona `/Patients/Details/{id}` została rozszerzona o zapis audytu zarówno dla udanych, jak i nieudanych prób dostępu. Dzięki temu system umożliwia analizę, którzy użytkownicy przeglądali dane pacjentów oraz kto próbował uzyskać dostęp do danych niewidocznych ze względu na mechanizm Row-Level Security.
