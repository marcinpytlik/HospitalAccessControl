# DAY011 — HospitalAccessControl  
## Szczegóły pacjenta z ochroną Row-Level Security

## Cel dnia

Celem DAY011 jest dodanie strony szczegółów pacjenta:

```text
/Patients/Details/{id}
```

Najważniejsze jest to, że użytkownik nie powinien móc podejrzeć pacjenta z innego oddziału, nawet jeżeli ręcznie wpisze adres URL.

Przykład:

```text
HOSPITAL\doctor.cardio
```

widzi pacjentów z Kardiologii.

Jeżeli ten użytkownik spróbuje wejść ręcznie na pacjenta z Ortopedii, np.:

```text
/Patients/Details/11
```

aplikacja powinna pokazać:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

To nie będzie tylko walidacja w UI.  
To będzie efekt działania **Row-Level Security** w SQL Server.

---

## Efekt końcowy DAY011

Po zakończeniu tego dnia powinieneś mieć:

- DTO szczegółów pacjenta,
- rozszerzony `IPatientReadService`,
- implementację pobierania szczegółów pacjenta,
- stronę Razor Pages:

```text
/Patients/Details/{id}
```

- link `Szczegóły` na liście pacjentów,
- test dostępu do pacjenta z własnego oddziału,
- test ręcznego wejścia do pacjenta z innego oddziału,
- potwierdzenie, że RLS chroni szczegóły pacjenta.

---

## Dlaczego ten dzień jest ważny?

Lista pacjentów z DAY010 pokazała, że RLS filtruje zbiory danych.

DAY011 pokazuje ważniejszy scenariusz bezpieczeństwa:

```text
użytkownik zna PatientId
        ↓
wpisuje adres ręcznie
        ↓
aplikacja pyta SQL Server o pacjenta
        ↓
RLS ukrywa rekord
        ↓
aplikacja pokazuje brak dostępu
```

To jest bardzo dobry scenariusz demonstracyjny do pracy inżynierskiej.

---

# 1. Jak działa ochrona szczegółów pacjenta?

W aplikacji wykonamy zapytanie:

```csharp
_dbContext.Patients
    .Where(x => x.PatientId == patientId)
    .Select(...)
    .SingleOrDefaultAsync()
```

Nie dodamy warunku:

```csharp
x.DepartmentId == aktualnyOddzialUzytkownika
```

Dlaczego?

Bo SQL Server zrobi to przez RLS.

Jeżeli pacjent ma oddział, do którego użytkownik nie ma dostępu, SQL Server zachowa się tak, jakby tego rekordu nie było.

---

# 2. Przykład testowy

Z seedów mamy:

```text
PatientId 1-10   -> CARD / Kardiologia
PatientId 11-20  -> ORTH / Ortopedia
PatientId 21-30  -> NEUR / Neurologia
PatientId 31-35  -> EMER / Izba Przyjęć
PatientId 36-40  -> PED / Pediatria
```

Dla użytkownika:

```text
HOSPITAL\doctor.cardio
```

oczekujemy:

```text
/Patients/Details/1   -> działa
/Patients/Details/11  -> brak dostępu
```

Dla użytkownika:

```text
HOSPITAL\doctor.ortho
```

oczekujemy:

```text
/Patients/Details/11  -> działa
/Patients/Details/1   -> brak dostępu
```

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

Sprawdź build po DAY010:

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

# Krok 2 — szybka weryfikacja RLS w SQL Server

Zanim dodamy szczegóły pacjenta, sprawdzamy RLS.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId IN (1,11) ORDER BY PatientId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId IN (1,11) ORDER BY PatientId;"
```

Oczekiwany wynik dla `doctor.cardio`:

```text
PatientId = 1
DepartmentId = 1
```

Nie powinien pojawić się:

```text
PatientId = 11
DepartmentId = 2
```

Jeżeli pojawia się pacjent 11, RLS nie działa poprawnie.  
Wróć do DAY009.

---

# Krok 3 — utworzenie pliku DTO szczegółów pacjenta

Tworzymy plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Patients\PatientDetailsDto.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application\Patients
```

Powinieneś zobaczyć:

```text
PatientDetailsDto.cs
PatientListItemDto.cs
IPatientReadService.cs
```

---

# Krok 4 — PatientDetailsDto

## Plik

```text
src\HospitalAccessControl.Application\Patients\PatientDetailsDto.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Patients;

public sealed class PatientDetailsDto
{
    public int PatientId { get; init; }

    public string MedicalNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Pesel { get; init; } = string.Empty;

    public DateOnly DateOfBirth { get; init; }

    public string GenderCode { get; init; } = string.Empty;

    public string PatientStatusCode { get; init; } = string.Empty;

    public int DepartmentId { get; init; }

    public string DepartmentCode { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public IReadOnlyList<PatientMedicalRecordDto> MedicalRecords { get; init; }
        = Array.Empty<PatientMedicalRecordDto>();
}

public sealed class PatientMedicalRecordDto
{
    public int MedicalRecordId { get; init; }

    public string RecordTypeCode { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Diagnosis { get; init; }

    public string? Treatment { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}
```

---

## Uwaga o PESEL

W szczegółach pacjenta pokazujemy `Pesel`, bo później będzie można na tym dobrze zademonstrować:

```text
Dynamic Data Masking
```

Na tym etapie PESEL jest jeszcze widoczny.

W kolejnych etapach możemy dodać maskowanie albo ograniczyć widoczność w zależności od roli.

---

# Krok 5 — rozszerzenie IPatientReadService

## Plik

```text
src\HospitalAccessControl.Application\Patients\IPatientReadService.cs
```

## Zmień zawartość na:

```csharp
namespace HospitalAccessControl.Application.Patients;

public interface IPatientReadService
{
    Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default);

    Task<PatientDetailsDto?> GetPatientDetailsAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}
```

---

## Co dodaliśmy?

Dodaliśmy metodę:

```csharp
GetPatientDetailsAsync(int patientId)
```

Zwraca:

```text
PatientDetailsDto
```

albo:

```text
null
```

Jeżeli SQL Server przez RLS ukryje pacjenta, aplikacja dostanie `null`.

---

# Krok 6 — rozszerzenie PatientReadService

## Plik

```text
src\HospitalAccessControl.Infrastructure\Patients\PatientReadService.cs
```

## Zmień zawartość na:

```csharp
using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Patients;

public sealed class PatientReadService : IPatientReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public PatientReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new PatientListItemDto
            {
                PatientId = x.PatientId,
                MedicalNumber = x.MedicalNumber,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                GenderCode = x.GenderCode,
                PatientStatusCode = x.PatientStatusCode,
                DepartmentId = x.DepartmentId,
                DepartmentCode = x.Department.Code,
                DepartmentName = x.Department.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PatientDetailsDto?> GetPatientDetailsAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => x.PatientId == patientId)
            .Select(x => new PatientDetailsDto
            {
                PatientId = x.PatientId,
                MedicalNumber = x.MedicalNumber,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Pesel = x.Pesel,
                DateOfBirth = x.DateOfBirth,
                GenderCode = x.GenderCode,
                PatientStatusCode = x.PatientStatusCode,
                DepartmentId = x.DepartmentId,
                DepartmentCode = x.Department.Code,
                DepartmentName = x.Department.Name,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                MedicalRecords = x.MedicalRecords
                    .Where(r => !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new PatientMedicalRecordDto
                    {
                        MedicalRecordId = r.MedicalRecordId,
                        RecordTypeCode = r.RecordTypeCode,
                        Title = r.Title,
                        Description = r.Description,
                        Diagnosis = r.Diagnosis,
                        Treatment = r.Treatment,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
```

---

## Ważna obserwacja

W metodzie:

```csharp
GetPatientDetailsAsync
```

nie ma ręcznego sprawdzania:

```csharp
czy aktualny użytkownik ma dostęp do oddziału
```

Jest tylko:

```csharp
.Where(x => x.PatientId == patientId)
```

A mimo to SQL Server ukryje rekord, jeżeli użytkownik nie ma dostępu.

To jest dokładnie to, co chcemy pokazać w projekcie.

---

# Krok 7 — utworzenie strony Details

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml

New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Web\Pages\Patients
```

Powinieneś zobaczyć:

```text
Index.cshtml
Index.cshtml.cs
Details.cshtml
Details.cshtml.cs
```

---

# Krok 8 — Details.cshtml.cs

## Plik

```text
src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class DetailsModel : PageModel
{
    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;

    public DetailsModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
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

        if (Patient is null)
        {
            AccessDeniedOrNotFound = true;
        }

        return Page();
    }
}
```

---

## Dlaczego nie zwracamy `NotFound()`?

Można by zwrócić:

```csharp
return NotFound();
```

Ale na potrzeby demo i pracy lepiej pokazać czytelny komunikat:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

To pokazuje, że aplikacja nie zdradza, czy rekord istnieje.

To jest dobra praktyka bezpieczeństwa.

---

# Krok 9 — Details.cshtml

## Plik

```text
src\HospitalAccessControl.Web\Pages\Patients\Details.cshtml
```

## Zawartość

Wklej:

```cshtml
@page "{id:int}"
@model HospitalAccessControl.Web.Pages.Patients.DetailsModel

@{
    ViewData["Title"] = "Szczegóły pacjenta";
}

<h1>Szczegóły pacjenta</h1>

<div class="alert alert-info">
    <strong>Aktualny użytkownik:</strong>
    @Model.CurrentUser.DomainLogin
    <br />
    <strong>Nazwa:</strong>
    @Model.CurrentUser.DisplayName
</div>

@if (Model.AccessDeniedOrNotFound || Model.Patient is null)
{
    <div class="alert alert-warning">
        Nie znaleziono pacjenta albo brak dostępu.
    </div>

    <p>
        <a asp-page="/Patients/Index" class="btn btn-secondary">Powrót do listy</a>
    </p>
}
else
{
    <div class="card mb-3">
        <div class="card-header">
            Dane pacjenta
        </div>
        <div class="card-body">
            <dl class="row">
                <dt class="col-sm-3">Id</dt>
                <dd class="col-sm-9">@Model.Patient.PatientId</dd>

                <dt class="col-sm-3">Numer medyczny</dt>
                <dd class="col-sm-9">@Model.Patient.MedicalNumber</dd>

                <dt class="col-sm-3">Imię</dt>
                <dd class="col-sm-9">@Model.Patient.FirstName</dd>

                <dt class="col-sm-3">Nazwisko</dt>
                <dd class="col-sm-9">@Model.Patient.LastName</dd>

                <dt class="col-sm-3">PESEL</dt>
                <dd class="col-sm-9">@Model.Patient.Pesel</dd>

                <dt class="col-sm-3">Data urodzenia</dt>
                <dd class="col-sm-9">@Model.Patient.DateOfBirth.ToString("yyyy-MM-dd")</dd>

                <dt class="col-sm-3">Płeć</dt>
                <dd class="col-sm-9">@Model.Patient.GenderCode</dd>

                <dt class="col-sm-3">Status</dt>
                <dd class="col-sm-9">@Model.Patient.PatientStatusCode</dd>

                <dt class="col-sm-3">Oddział</dt>
                <dd class="col-sm-9">
                    @Model.Patient.DepartmentCode
                    —
                    @Model.Patient.DepartmentName
                </dd>

                <dt class="col-sm-3">Utworzono</dt>
                <dd class="col-sm-9">@Model.Patient.CreatedAt</dd>

                <dt class="col-sm-3">Utworzył</dt>
                <dd class="col-sm-9">@Model.Patient.CreatedBy</dd>
            </dl>
        </div>
    </div>

    <h2>Dokumentacja medyczna</h2>

    @if (Model.Patient.MedicalRecords.Count == 0)
    {
        <div class="alert alert-secondary">
            Brak wpisów medycznych dla pacjenta.
        </div>
    }
    else
    {
        <table class="table table-striped table-bordered">
            <thead>
                <tr>
                    <th>Id</th>
                    <th>Typ</th>
                    <th>Tytuł</th>
                    <th>Diagnoza</th>
                    <th>Leczenie</th>
                    <th>Utworzono</th>
                    <th>Utworzył</th>
                </tr>
            </thead>
            <tbody>
            @foreach (var record in Model.Patient.MedicalRecords)
            {
                <tr>
                    <td>@record.MedicalRecordId</td>
                    <td>@record.RecordTypeCode</td>
                    <td>@record.Title</td>
                    <td>@record.Diagnosis</td>
                    <td>@record.Treatment</td>
                    <td>@record.CreatedAt</td>
                    <td>@record.CreatedBy</td>
                </tr>
            }
            </tbody>
        </table>
    }

    <p>
        <a asp-page="/Patients/Index" class="btn btn-secondary">Powrót do listy</a>
    </p>
}
```

---

# Krok 10 — dodanie linku Szczegóły na liście pacjentów

Otwórz plik:

```text
src\HospitalAccessControl.Web\Pages\Patients\Index.cshtml
```

Znajdź nagłówek tabeli:

```cshtml
<tr>
    <th>Id</th>
    <th>Numer medyczny</th>
    <th>Imię</th>
    <th>Nazwisko</th>
    <th>Data urodzenia</th>
    <th>Płeć</th>
    <th>Status</th>
    <th>Oddział</th>
</tr>
```

Dodaj kolumnę:

```cshtml
<th>Akcje</th>
```

Czyli:

```cshtml
<tr>
    <th>Id</th>
    <th>Numer medyczny</th>
    <th>Imię</th>
    <th>Nazwisko</th>
    <th>Data urodzenia</th>
    <th>Płeć</th>
    <th>Status</th>
    <th>Oddział</th>
    <th>Akcje</th>
</tr>
```

Następnie w wierszu danych dodaj:

```cshtml
<td>
    <a asp-page="/Patients/Details"
       asp-route-id="@patient.PatientId"
       class="btn btn-sm btn-primary">
        Szczegóły
    </a>
</td>
```

Czyli końcówka wiersza powinna wyglądać podobnie:

```cshtml
<td>
    @patient.DepartmentCode
    —
    @patient.DepartmentName
</td>
<td>
    <a asp-page="/Patients/Details"
       asp-route-id="@patient.PatientId"
       class="btn btn-sm btn-primary">
        Szczegóły
    </a>
</td>
```

---

# Krok 11 — build po dodaniu szczegółów

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

# Krok 12 — uruchomienie aplikacji

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz:

```text
https://localhost:xxxx/Patients
```

Na liście powinien pojawić się przycisk:

```text
Szczegóły
```

---

# Krok 13 — test 1: doctor.cardio widzi pacjenta CARD

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
/Patients
```

Kliknij szczegóły pierwszego pacjenta.

Oczekiwany wynik:

```text
szczegóły pacjenta są widoczne
oddział CARD — Kardiologia
widać dokumentację medyczną
```

Możesz też wejść ręcznie:

```text
/Patients/Details/1
```

Oczekiwany wynik:

```text
pacjent widoczny
```

---

# Krok 14 — test 2: doctor.cardio próbuje wejść do pacjenta ORTH

Dalej jako:

```text
HOSPITAL\doctor.cardio
```

wejdź ręcznie na:

```text
/Patients/Details/11
```

Oczekiwany wynik:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

Dlaczego?

Bo pacjent 11 należy do:

```text
DepartmentId = 2
ORTH — Ortopedia
```

a `doctor.cardio` ma dostęp tylko do:

```text
DepartmentId = 1
CARD — Kardiologia
```

---

# Krok 15 — test 3: doctor.ortho widzi pacjenta ORTH

Zatrzymaj aplikację:

```text
CTRL + C
```

Zmień użytkownika:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.ortho",
  "SamAccountName": "doctor.ortho",
  "DisplayName": "Marek Ortopeda"
}
```

Uruchom aplikację:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Wejdź na:

```text
/Patients/Details/11
```

Oczekiwany wynik:

```text
pacjent widoczny
oddział ORTH — Ortopedia
```

---

# Krok 16 — test 4: doctor.ortho nie widzi pacjenta CARD

Dalej jako:

```text
HOSPITAL\doctor.ortho
```

wejdź ręcznie na:

```text
/Patients/Details/1
```

Oczekiwany wynik:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

---

# Krok 17 — test 5: it.admin nie widzi żadnego pacjenta

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
/Patients
```

Oczekiwany wynik:

```text
0 pacjentów
```

Wejdź ręcznie na:

```text
/Patients/Details/1
```

Oczekiwany wynik:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

To jest jeden z najważniejszych testów bezpieczeństwa.

---

# Krok 18 — test 6: brak lub błędny użytkownik

Ustaw użytkownika, którego nie ma w tabeli `security.ApplicationUsers`:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\unknown.user",
  "SamAccountName": "unknown.user",
  "DisplayName": "Nieznany użytkownik"
}
```

Oczekiwany wynik na:

```text
/Patients
```

```text
0 pacjentów
```

Oczekiwany wynik na:

```text
/Patients/Details/1
```

```text
Nie znaleziono pacjenta albo brak dostępu.
```

---

# Krok 19 — test SQL potwierdzający Details

To samo zachowanie możesz potwierdzić w SQL.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId = 1; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId = 11;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId = 1; SELECT PatientId, MedicalNumber, DepartmentId FROM medical.Patients WHERE PatientId = 11;"
```

Oczekiwany wynik:

```text
pierwszy SELECT zwraca pacjenta 1
drugi SELECT nie zwraca nic
```

---

# Krok 20 — opcjonalnie: poprawa komunikatu w UI

W komunikacie:

```text
Nie znaleziono pacjenta albo brak dostępu.
```

celowo nie rozróżniamy:

```text
pacjent nie istnieje
pacjent istnieje, ale brak dostępu
```

Dlaczego?

Bo aplikacja nie powinna zdradzać, że rekord istnieje, jeżeli użytkownik nie ma do niego uprawnień.

To można opisać w pracy jako element ograniczania ujawniania informacji.

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
git commit -m "DAY011 Add patient details protected by RLS"
```

---

# Kontrola końcowa DAY011

Lista kontrolna:

```text
[ ] Utworzono PatientDetailsDto
[ ] Utworzono PatientMedicalRecordDto
[ ] Rozszerzono IPatientReadService o GetPatientDetailsAsync
[ ] Rozszerzono PatientReadService o GetPatientDetailsAsync
[ ] Utworzono Patients\Details.cshtml.cs
[ ] Utworzono Patients\Details.cshtml
[ ] Dodano link Szczegóły na liście pacjentów
[ ] dotnet build kończy się sukcesem
[ ] doctor.cardio widzi /Patients/Details/1
[ ] doctor.cardio nie widzi /Patients/Details/11
[ ] doctor.ortho widzi /Patients/Details/11
[ ] doctor.ortho nie widzi /Patients/Details/1
[ ] it.admin nie widzi /Patients/Details/1
[ ] unknown.user nie widzi /Patients/Details/1
[ ] Komunikat nie zdradza, czy pacjent istnieje, czy brak dostępu
```

---

# Najczęstsze problemy

## Problem 1 — Details zwraca zawsze brak dostępu

Najczęstsze przyczyny:

- RLS nie dostaje `SESSION_CONTEXT`,
- interceptor z DAY008 nie działa,
- `DevelopmentUser.DomainLogin` nie zgadza się z bazą,
- RLS jest zbyt restrykcyjny.

Sprawdź stronę główną:

```text
SESSION_CONTEXT('CurrentUser')
```

Powinna pokazać np.:

```text
HOSPITAL\doctor.cardio
```

Sprawdź bazę:

```sql
SELECT ApplicationUserId, DomainLogin
FROM security.ApplicationUsers
ORDER BY ApplicationUserId;
```

---

## Problem 2 — Details pokazuje pacjenta z innego oddziału

To oznacza, że RLS nie działa albo jest wyłączony.

Sprawdź:

```sql
SELECT
    SCHEMA_NAME(schema_id) AS SchemaName,
    name,
    is_enabled
FROM sys.security_policies
WHERE name = N'HospitalDepartmentSecurityPolicy';
```

Oczekiwany wynik:

```text
is_enabled = 1
```

Jeżeli jest `0`, włącz:

```sql
ALTER SECURITY POLICY security.HospitalDepartmentSecurityPolicy
WITH (STATE = ON);
```

---

## Problem 3 — błąd namespace w Details.cshtml

Sprawdź początek pliku:

```cshtml
@page "{id:int}"
@model HospitalAccessControl.Web.Pages.Patients.DetailsModel
```

Sprawdź namespace w PageModel:

```csharp
namespace HospitalAccessControl.Web.Pages.Patients;
```

---

## Problem 4 — błąd po dodaniu metody do interfejsu

Objaw:

```text
PatientReadService does not implement interface member
```

Oznacza to, że w `IPatientReadService` dodano:

```csharp
GetPatientDetailsAsync
```

ale w `PatientReadService` jeszcze nie ma tej metody albo sygnatura się różni.

Sygnatury muszą być zgodne:

```csharp
Task<PatientDetailsDto?> GetPatientDetailsAsync(
    int patientId,
    CancellationToken cancellationToken = default);
```

---

## Problem 5 — MedicalRecords powoduje problem w projekcji

Jeżeli projekcja z `MedicalRecords` sprawia problem, na chwilę uprość `GetPatientDetailsAsync`, ustawiając:

```csharp
MedicalRecords = Array.Empty<PatientMedicalRecordDto>()
```

Potem wróć do wersji z dokumentacją.

Docelowo jednak wersja z `MedicalRecords` powinna działać poprawnie.

---

## Problem 6 — DateOnly w Razor Pages

Jeżeli widok ma problem z formatowaniem daty, używaj:

```cshtml
@Model.Patient.DateOfBirth.ToString("yyyy-MM-dd")
```

zamiast:

```cshtml
@Model.Patient.DateOfBirth
```

---

# Efekt końcowy DAY011

Po zakończeniu DAY011 aplikacja obsługuje:

```text
/Patients
/Patients/Details/{id}
```

i chroni dane na poziomie SQL Server.

Scenariusz bezpieczeństwa:

```text
użytkownik widzi listę swoich pacjentów
użytkownik może wejść w szczegóły swoich pacjentów
użytkownik nie może podejrzeć pacjenta z innego oddziału po URL
administrator IT bez przypisania nie widzi danych medycznych
```

To jest bardzo dobry etap do pokazania na demo.

Następny etap:

```text
DAY012 — Audyt dostępu do pacjenta
```

W DAY012 dodamy zapisywanie zdarzenia do:

```text
audit.AccessLog
```

przy wejściu w szczegóły pacjenta.

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach jedenastego etapu implementacji dodano stronę szczegółów pacjenta `/Patients/Details/{id}`. Rozszerzono warstwę aplikacyjną o DTO szczegółów pacjenta oraz metodę `GetPatientDetailsAsync` w serwisie odczytu pacjentów. Strona szczegółów umożliwia wyświetlenie danych pacjenta oraz powiązanej dokumentacji medycznej. Dostęp do szczegółów pacjenta jest chroniony przez Row-Level Security w SQL Server, dzięki czemu użytkownik nie może uzyskać dostępu do pacjenta z innego oddziału nawet po ręcznym wpisaniu identyfikatora w adresie URL. W przypadku braku dostępu aplikacja zwraca ogólny komunikat „Nie znaleziono pacjenta albo brak dostępu”, nie ujawniając, czy rekord faktycznie istnieje.
