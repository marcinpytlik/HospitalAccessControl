# DAY013 — HospitalAccessControl  
## Rozbudowa audytu o RequestedPatientId

## Cel dnia

Celem DAY013 jest poprawienie modelu audytu aplikacyjnego tak, aby system zapisywał nie tylko faktycznie widocznego pacjenta, ale również identyfikator pacjenta, którego użytkownik próbował otworzyć.

Po DAY012 mamy działający audyt dostępu do szczegółów pacjenta, ale pojawia się ważny problem projektowy.

Jeżeli użytkownik wpisze adres:

```text
/Patients/Details/11
```

a Row-Level Security ukryje tego pacjenta, aplikacja dostanie:

```text
Patient = null
```

Wtedy nie powinniśmy zapisywać:

```text
PatientId = 11
```

jeżeli `PatientId` ma relację FK do `medical.Patients`, bo aplikacja z punktu widzenia RLS tego pacjenta nie widzi. Dodatkowo pacjent o takim identyfikatorze mógłby w ogóle nie istnieć.

Dlatego dodajemy osobne pole:

```text
RequestedPatientId
```

Dzięki temu audyt będzie rozróżniał:

```text
PatientId           -> pacjent faktycznie odczytany i dostępny
RequestedPatientId  -> identyfikator wpisany/żądany przez użytkownika
```

---

## Efekt końcowy DAY013

Po zakończeniu tego dnia powinieneś mieć:

- nowe pole w encji `AccessLog`:

```text
RequestedPatientId
```

- nową kolumnę w tabeli:

```text
audit.AccessLog.RequestedPatientId
```

- migrację EF Core:

```text
AddRequestedPatientIdToAccessLog
```

- zaktualizowany `AccessLogCreateDto`,
- zaktualizowany `AuditService`,
- poprawiony zapis audytu w `Patients/Details.cshtml.cs`,
- testy dla udanego i nieudanego dostępu,
- możliwość pokazania w audycie, że użytkownik próbował wejść do konkretnego identyfikatora pacjenta.

---

## Dlaczego to jest ważne?

Bez pola `RequestedPatientId` audyt nie jest pełny.

Przykład:

```text
HOSPITAL\doctor.cardio
wchodzi na /Patients/Details/11
RLS blokuje dostęp
aplikacja dostaje null
```

Jeżeli zapiszesz tylko:

```text
PatientId = NULL
WasSuccessful = false
```

to wiesz, że była nieudana próba, ale nie wiesz, czego dotyczyła.

Po dodaniu `RequestedPatientId` masz:

```text
PatientId = NULL
RequestedPatientId = 11
WasSuccessful = false
```

To jest dużo lepsze do analizy bezpieczeństwa.

---

# Krok 1 — przejście do katalogu projektu

Otwórz PowerShell:

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

Sprawdź build po DAY012:

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

# Krok 2 — sprawdzenie aktualnej tabeli audytu

Zanim zmienimy model, sprawdź aktualne kolumny tabeli.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT c.column_id, c.name, TYPE_NAME(c.user_type_id) AS DataType, c.is_nullable FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'audit.AccessLog') ORDER BY c.column_id;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT c.column_id, c.name, TYPE_NAME(c.user_type_id) AS DataType, c.is_nullable FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'audit.AccessLog') ORDER BY c.column_id;"
```

Na tym etapie prawdopodobnie nie ma jeszcze kolumny:

```text
RequestedPatientId
```

---

# Krok 3 — modyfikacja encji AccessLog

Otwórz plik:

```text
src\HospitalAccessControl.Domain\Entities\AccessLog.cs
```

Dodaj właściwość:

```csharp
public int? RequestedPatientId { get; set; }
```

Docelowo plik powinien wyglądać tak:

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class AccessLog
{
    public long AccessLogId { get; set; }

    public string DomainLogin { get; set; } = string.Empty;

    public int? PatientId { get; set; }

    public int? RequestedPatientId { get; set; }

    public Patient? Patient { get; set; }

    public string ActionCode { get; set; } = string.Empty;

    public string ObjectName { get; set; } = string.Empty;

    public DateTime AccessDate { get; set; }

    public string? ClientHost { get; set; }

    public string? ApplicationName { get; set; }

    public bool WasSuccessful { get; set; }

    public string? AdditionalInfo { get; set; }
}
```

---

# Krok 4 — konfiguracja EF Core

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\AccessLogConfiguration.cs
```

Dodaj konfigurację pola:

```csharp
builder.Property(x => x.RequestedPatientId)
    .HasColumnName("RequestedPatientId");
```

Dodaj indeks:

```csharp
builder.HasIndex(x => x.RequestedPatientId)
    .HasDatabaseName("IX_AccessLog_RequestedPatientId");
```

Przykładowy fragment konfiguracji:

```csharp
builder.Property(x => x.PatientId)
    .HasColumnName("PatientId");

builder.Property(x => x.RequestedPatientId)
    .HasColumnName("RequestedPatientId");

builder.HasIndex(x => x.PatientId)
    .HasDatabaseName("IX_AccessLog_PatientId");

builder.HasIndex(x => x.RequestedPatientId)
    .HasDatabaseName("IX_AccessLog_RequestedPatientId");
```

---

# Krok 5 — aktualizacja AccessLogCreateDto

Otwórz:

```text
src\HospitalAccessControl.Application\Audit\AccessLogCreateDto.cs
```

Dodaj:

```csharp
public int? RequestedPatientId { get; init; }
```

Docelowo:

```csharp
namespace HospitalAccessControl.Application.Audit;

public sealed class AccessLogCreateDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public int? PatientId { get; init; }

    public int? RequestedPatientId { get; init; }

    public string ActionCode { get; init; } = string.Empty;

    public string ObjectName { get; init; } = string.Empty;

    public string? ClientHost { get; init; }

    public string? ApplicationName { get; init; }

    public bool WasSuccessful { get; init; }

    public string? AdditionalInfo { get; init; }
}
```

---

# Krok 6 — aktualizacja AuditService

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Audit\AuditService.cs
```

Znajdź tworzenie encji:

```csharp
var entity = new AccessLog
{
    ...
};
```

Dodaj:

```csharp
RequestedPatientId = accessLog.RequestedPatientId,
```

Docelowo:

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
            RequestedPatientId = accessLog.RequestedPatientId,
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

# Krok 7 — aktualizacja Details.cshtml.cs

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml.cs
```

W miejscu tworzenia `AccessLogCreateDto` zmień:

```csharp
PatientId = id,
```

na:

```csharp
PatientId = Patient?.PatientId,
RequestedPatientId = id,
```

Docelowy fragment:

```csharp
await _auditService.LogAccessAsync(
    new AccessLogCreateDto
    {
        DomainLogin = CurrentUser.DomainLogin,
        PatientId = Patient?.PatientId,
        RequestedPatientId = id,
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
```

---

# Krok 8 — build po zmianach

```powershell
dotnet build
```

Jeżeli build się nie uda, najczęstsze przyczyny:

```text
literówka w RequestedPatientId
brak właściwości w DTO
brak właściwości w encji
niezaktualizowany AuditService
```

---

# Krok 9 — utworzenie migracji

```powershell
dotnet ef migrations add AddRequestedPatientIdToAccessLog `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output-dir Data\Migrations
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Migrations | Select-Object Name
```

Powinna być migracja:

```text
*_AddRequestedPatientIdToAccessLog.cs
```

---

# Krok 10 — kontrola migracji

Otwórz plik migracji i sprawdź, czy zawiera:

```csharp
migrationBuilder.AddColumn<int>(
    name: "RequestedPatientId",
    schema: "audit",
    table: "AccessLog",
    type: "int",
    nullable: true);
```

oraz indeks:

```csharp
migrationBuilder.CreateIndex(
    name: "IX_AccessLog_RequestedPatientId",
    schema: "audit",
    table: "AccessLog",
    column: "RequestedPatientId");
```

---

# Krok 11 — update bazy

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

Oczekiwany wynik:

```text
Done.
```

---

# Krok 12 — weryfikacja kolumny

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT c.name, TYPE_NAME(c.user_type_id) AS DataType, c.is_nullable FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'audit.AccessLog') AND c.name = N'RequestedPatientId';"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT c.name, TYPE_NAME(c.user_type_id) AS DataType, c.is_nullable FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'audit.AccessLog') AND c.name = N'RequestedPatientId';"
```

Oczekiwany wynik:

```text
RequestedPatientId  int  1
```

---

# Krok 13 — test udanego dostępu

Ustaw użytkownika:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Wejdź na:

```text
/Patients/Details/1
```

Sprawdź audyt:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, RequestedPatientId, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Oczekiwany ostatni wpis:

```text
PatientId = 1
RequestedPatientId = 1
WasSuccessful = 1
```

---

# Krok 14 — test nieudanego dostępu do istniejącego pacjenta

Dalej jako:

```text
HOSPITAL\doctor.cardio
```

wejdź na:

```text
/Patients/Details/11
```

Oczekiwany wynik UI:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

Sprawdź audyt:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, RequestedPatientId, WasSuccessful, AdditionalInfo, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Oczekiwany ostatni wpis:

```text
PatientId = NULL
RequestedPatientId = 11
WasSuccessful = 0
```

---

# Krok 15 — test nieistniejącego pacjenta

Wejdź na:

```text
/Patients/Details/9999
```

Sprawdź audyt.

Oczekiwany wynik:

```text
PatientId = NULL
RequestedPatientId = 9999
WasSuccessful = 0
```

To jest bardzo ważna różnica.

---

# Krok 16 — zapytanie do analizy nieudanych prób

Utwórz plik:

```powershell
New-Item -ItemType File -Force .\sql\09_tests\24_failed_requested_patient_access.sql
```

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    RequestedPatientId,
    ActionCode,
    WasSuccessful,
    AdditionalInfo,
    AccessDate
FROM audit.AccessLog
WHERE WasSuccessful = 0
ORDER BY AccessLogId DESC;
GO
```

Uruchom:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\24_failed_requested_patient_access.sql
```

---

# Krok 17 — wygenerowanie skryptu SQL z migracji

```powershell
dotnet ef migrations script `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output .\sql\generated\004_AddRequestedPatientIdToAccessLog.sql
```

---

# Krok 18 — commit Git

```powershell
git status
git add .
git commit -m "DAY013 Add requested patient id to access audit"
```

---

# Kontrola końcowa DAY013

```text
[ ] Encja AccessLog ma RequestedPatientId
[ ] AccessLogConfiguration ma konfigurację RequestedPatientId
[ ] AccessLogCreateDto ma RequestedPatientId
[ ] AuditService mapuje RequestedPatientId
[ ] Details.cshtml.cs zapisuje PatientId = Patient?.PatientId
[ ] Details.cshtml.cs zapisuje RequestedPatientId = id
[ ] Utworzono migrację AddRequestedPatientIdToAccessLog
[ ] Wykonano database update
[ ] Kolumna RequestedPatientId istnieje w audit.AccessLog
[ ] Udany dostęp zapisuje PatientId i RequestedPatientId
[ ] Nieudany dostęp zapisuje RequestedPatientId i PatientId = NULL
[ ] Nieistniejący pacjent zapisuje RequestedPatientId i PatientId = NULL
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — błąd FK przy nieudanym dostępie

Objaw:

```text
The INSERT statement conflicted with the FOREIGN KEY constraint
```

Przyczyna:

```csharp
PatientId = id
```

zamiast:

```csharp
PatientId = Patient?.PatientId
```

Poprawka:

```csharp
PatientId = Patient?.PatientId,
RequestedPatientId = id,
```

---

## Problem 2 — migracja jest pusta

Sprawdź, czy dodałeś `RequestedPatientId` do encji domenowej:

```text
HospitalAccessControl.Domain\Entities\AccessLog.cs
```

---

## Problem 3 — kolumna nie istnieje w bazie

Sprawdź, czy wykonałeś:

```powershell
dotnet ef database update
```

---

## Problem 4 — audyt nie zapisuje RequestedPatientId

Sprawdź `AuditService` i `Details.cshtml.cs`.

---

# Efekt końcowy DAY013

Po zakończeniu DAY013 audyt jest dużo bardziej profesjonalny:

```text
PatientId           -> pacjent faktycznie widoczny i odczytany
RequestedPatientId  -> identyfikator żądany przez użytkownika
WasSuccessful       -> czy dostęp był skuteczny
```

Następny etap:

```text
DAY014 — Panel audytu w aplikacji
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach trzynastego etapu implementacji rozszerzono mechanizm audytu o pole `RequestedPatientId`, które przechowuje identyfikator pacjenta żądany przez użytkownika. Zmiana ta pozwala rejestrować próby dostępu do danych niewidocznych przez Row-Level Security lub do rekordów nieistniejących, bez naruszania integralności referencyjnej pola `PatientId`. Dzięki temu audyt umożliwia dokładniejszą analizę prób dostępu do danych medycznych.
