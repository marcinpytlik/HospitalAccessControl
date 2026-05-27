# DAY010 — HospitalAccessControl  
## Lista pacjentów w Razor Pages z filtrowaniem przez Row-Level Security

## Cel dnia

Celem DAY010 jest utworzenie pierwszej realnej funkcji aplikacji:

```text
/Patients
```

czyli listy pacjentów.

Najważniejsze jest to, że lista pacjentów **nie będzie filtrowana ręcznie w kodzie aplikacji po oddziale**.

Filtrowanie ma wykonać SQL Server przez mechanizm:

```text
Row-Level Security
```

oparty o:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

Czyli aplikacja wykona zwykłe zapytanie:

```csharp
_dbContext.Patients.ToListAsync()
```

a SQL Server sam ograniczy wynik do pacjentów widocznych dla aktualnego użytkownika.

---

## Efekt końcowy DAY010

Po zakończeniu tego dnia powinieneś mieć:

- katalog `Application/Patients`,
- DTO do listy pacjentów,
- interfejs `IPatientReadService`,
- implementację `PatientReadService`,
- rejestrację serwisu w DI,
- stronę Razor Pages:

```text
/Patients
```

- link do pacjentów w menu,
- testy ręczne dla różnych użytkowników DEV,
- potwierdzenie, że RLS działa z poziomu aplikacji.

---

## Dlaczego ten dzień jest ważny?

Do tej pory mieliśmy:

```text
DAY007 — aplikacja zna aktualnego użytkownika
DAY008 — aplikacja przekazuje użytkownika do SQL Server przez SESSION_CONTEXT
DAY009 — SQL Server filtruje dane przez RLS
```

W DAY010 pierwszy raz zobaczymy efekt w aplikacji webowej.

Przykład:

```text
HOSPITAL\doctor.cardio
```

powinien widzieć:

```text
10 pacjentów Kardiologii
```

A:

```text
HOSPITAL\doctor.ortho
```

powinien widzieć:

```text
10 pacjentów Ortopedii
```

A:

```text
HOSPITAL\it.admin
```

powinien widzieć:

```text
0 pacjentów
```

To będzie bardzo mocny element demonstracji pracy.

---

# 1. Architektura funkcji Patients

Utworzymy prosty przepływ:

```text
Patients/Index.cshtml
        ↓
Patients/Index.cshtml.cs
        ↓
IPatientReadService
        ↓
PatientReadService
        ↓
HospitalAccessControlDbContext
        ↓
SQL Server
        ↓
RLS
```

Aplikacja zapyta o pacjentów, ale nie będzie sama sprawdzać oddziału.

To robi SQL Server.

---

# 2. Co pokażemy na liście?

Na liście pacjentów pokażemy:

```text
PatientId
MedicalNumber
FirstName
LastName
DepartmentCode
DepartmentName
PatientStatusCode
DateOfBirth
```

Na tym etapie nie pokazujemy PESEL, bo później dodamy temat maskowania danych.

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

Sprawdź build po DAY009:

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

# Krok 2 — sprawdzenie, czy RLS działa w SQL Server

Zanim piszemy UI, sprawdzamy, czy RLS z DAY009 działa.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

Oczekiwany wynik:

```text
DepartmentId = 1
PatientsCount = 10
```

Jeżeli wynik pokazuje 40 pacjentów, RLS nie działa albo jest wyłączony.  
Wróć do DAY009.

---

# Krok 3 — utworzenie katalogów

Tworzymy katalog na logikę pacjentów w Application:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Application\Patients
```

Tworzymy katalog Razor Pages:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Web\Pages\Patients
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application
Get-ChildItem .\src\HospitalAccessControl.Web\Pages
```

---

# Krok 4 — utworzenie plików

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Patients\PatientListItemDto.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Patients\IPatientReadService.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Patients\PatientReadService.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Patients\Index.cshtml

New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Pages\Patients\Index.cshtml.cs
```

Uwaga: jeżeli katalog `Infrastructure\Patients` jeszcze nie istnieje, utwórz go:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Patients
```

Jeżeli wcześniej wykonałeś komendę tworzenia pliku przed katalogiem i dostałeś błąd, wykonaj najpierw utworzenie katalogu, a potem ponownie:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Patients\PatientReadService.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application\Patients
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Patients
Get-ChildItem .\src\HospitalAccessControl.Web\Pages\Patients
```

---

# Krok 5 — PatientListItemDto

## Plik

```text
src\HospitalAccessControl.Application\Patients\PatientListItemDto.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Patients;

public sealed class PatientListItemDto
{
    public int PatientId { get; init; }

    public string MedicalNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateOnly DateOfBirth { get; init; }

    public string GenderCode { get; init; } = string.Empty;

    public string PatientStatusCode { get; init; } = string.Empty;

    public int DepartmentId { get; init; }

    public string DepartmentCode { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;
}
```

---

## Po co DTO?

Nie chcemy przekazywać encji EF Core bezpośrednio do widoku.

DTO pozwala pokazać tylko to, co potrzebne na liście.

Na tym etapie celowo nie pokazujemy:

```text
Pesel
MedicalRecords
CreatedBy
UpdatedBy
```

---

# Krok 6 — IPatientReadService

## Plik

```text
src\HospitalAccessControl.Application\Patients\IPatientReadService.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Patients;

public interface IPatientReadService
{
    Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default);
}
```

---

## Dlaczego interfejs jest w Application?

Warstwa Web nie powinna znać szczegółów EF Core.

Strona Razor Pages woła:

```text
IPatientReadService
```

a implementacja w Infrastructure wykonuje zapytania do bazy.

---

# Krok 7 — PatientReadService

## Plik

```text
src\HospitalAccessControl.Infrastructure\Patients\PatientReadService.cs
```

## Zawartość

Wklej:

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
}
```

---

## Ważna rzecz

W tym kodzie **nie ma filtrowania po aktualnym użytkowniku**.

Nie robimy czegoś takiego:

```csharp
.Where(x => x.DepartmentId == currentUser.DepartmentId)
```

Dlaczego?

Bo filtrowanie robi SQL Server przez RLS.

To jest cel projektu.

---

# Krok 8 — rejestracja PatientReadService w DI

Otwórz plik:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Infrastructure.Patients;
```

W metodzie `AddInfrastructure`, przed:

```csharp
return services;
```

dodaj:

```csharp
services.AddScoped<IPatientReadService, PatientReadService>();
```

Docelowo końcówka metody powinna mieć między innymi:

```csharp
services.AddScoped<ISqlSessionContextDiagnostics, SqlSessionContextDiagnostics>();
services.AddScoped<IPatientReadService, PatientReadService>();

return services;
```

---

# Krok 9 — Razor Page: Index.cshtml.cs

## Plik

```text
src\HospitalAccessControl.Web\Pages\Patients\Index.cshtml.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class IndexModel : PageModel
{
    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public IReadOnlyList<PatientListItemDto> Patients { get; private set; }
        = Array.Empty<PatientListItemDto>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        Patients = await _patientReadService.GetPatientsAsync(cancellationToken);
    }
}
```

---

## Co robi PageModel?

Strona:

```text
/Patients
```

pobiera:

```text
aktualnego użytkownika
listę pacjentów
```

i przekazuje dane do widoku.

---

# Krok 10 — Razor Page: Index.cshtml

## Plik

```text
src\HospitalAccessControl.Web\Pages\Patients\Index.cshtml
```

## Zawartość

Wklej:

```cshtml
@page
@model HospitalAccessControl.Web.Pages.Patients.IndexModel

@{
    ViewData["Title"] = "Pacjenci";
}

<h1>Pacjenci</h1>

<p>
    Lista pacjentów widocznych dla aktualnego użytkownika.
</p>

<div class="alert alert-info">
    <strong>Aktualny użytkownik:</strong>
    @Model.CurrentUser.DomainLogin
    <br />
    <strong>Nazwa:</strong>
    @Model.CurrentUser.DisplayName
</div>

<div class="alert alert-secondary">
    <strong>Liczba widocznych pacjentów:</strong>
    @Model.Patients.Count
</div>

@if (Model.Patients.Count == 0)
{
    <div class="alert alert-warning">
        Brak pacjentów do wyświetlenia albo użytkownik nie ma dostępu do danych medycznych.
    </div>
}
else
{
    <table class="table table-striped table-bordered">
        <thead>
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
        </thead>
        <tbody>
        @foreach (var patient in Model.Patients)
        {
            <tr>
                <td>@patient.PatientId</td>
                <td>@patient.MedicalNumber</td>
                <td>@patient.FirstName</td>
                <td>@patient.LastName</td>
                <td>@patient.DateOfBirth</td>
                <td>@patient.GenderCode</td>
                <td>@patient.PatientStatusCode</td>
                <td>
                    @patient.DepartmentCode
                    —
                    @patient.DepartmentName
                </td>
            </tr>
        }
        </tbody>
    </table>
}
```

---

# Krok 11 — dodanie linku w menu

Otwórz plik:

```text
src\HospitalAccessControl.Web\Pages\Shared\_Layout.cshtml
```

Znajdź menu z linkami, zwykle fragment podobny do:

```cshtml
<ul class="navbar-nav flex-grow-1">
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
    </li>
</ul>
```

Dodaj link do pacjentów:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Patients/Index">Pacjenci</a>
</li>
```

Przykład:

```cshtml
<ul class="navbar-nav flex-grow-1">
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Patients/Index">Pacjenci</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
    </li>
</ul>
```

---

# Krok 12 — build po dodaniu strony

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

# Krok 13 — uruchomienie aplikacji

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz adres:

```text
https://localhost:xxxx/Patients
```

albo kliknij w menu:

```text
Pacjenci
```

---

# Krok 14 — test 1: doctor.cardio

Otwórz plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Ustaw:

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

Wejdź na:

```text
/Patients
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 10
Oddział: CARD — Kardiologia
```

Nie powinno być pacjentów z:

```text
ORTH
NEUR
EMER
PED
```

---

# Krok 15 — test 2: doctor.ortho

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

Uruchom aplikację ponownie:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Wejdź na:

```text
/Patients
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 10
Oddział: ORTH — Ortopedia
```

---

# Krok 16 — test 3: doctor.neuro

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.neuro",
  "SamAccountName": "doctor.neuro",
  "DisplayName": "Anna Neurolog"
}
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 10
Oddział: NEUR — Neurologia
```

---

# Krok 17 — test 4: nurse.ped

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\nurse.ped",
  "SamAccountName": "nurse.ped",
  "DisplayName": "Magdalena Pielęgniarka Pediatria"
}
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 5
Oddział: PED — Pediatria
```

---

# Krok 18 — test 5: it.admin

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\it.admin",
  "SamAccountName": "it.admin",
  "DisplayName": "Adam Administrator IT"
}
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 0
Brak pacjentów do wyświetlenia albo użytkownik nie ma dostępu do danych medycznych.
```

To jest bardzo ważny test do pokazania na obronie.

---

# Krok 19 — test 6: registration.user

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\registration.user",
  "SamAccountName": "registration.user",
  "DisplayName": "Karolina Rejestracja"
}
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 0
```

Dlaczego?

Bo w DAY006 `registration.user` nie dostał przypisania do oddziału.

To jest celowe.

---

# Krok 20 — test 7: registration.emer

Ustaw:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\registration.emer",
  "SamAccountName": "registration.emer",
  "DisplayName": "Tomasz Rejestracja Izba Przyjęć"
}
```

Oczekiwany wynik:

```text
Liczba widocznych pacjentów: 5
Oddział: EMER — Izba Przyjęć
```

---

# Krok 21 — weryfikacja SQL z poziomu aplikacji

To jest ważne:

Aplikacja nie robi ręcznego filtrowania po użytkowniku.

Możesz pokazać w kodzie:

```csharp
return await _dbContext.Patients
    .AsNoTracking()
    .Where(x => !x.IsDeleted)
    .OrderBy(x => x.LastName)
    .ThenBy(x => x.FirstName)
    .Select(...)
    .ToListAsync(cancellationToken);
```

Nie ma tam warunku:

```csharp
x.DepartmentId == currentUser.DepartmentId
```

A mimo to aplikacja pokazuje różne wyniki dla różnych użytkowników.

To jest dowód, że filtr działa w SQL Server.

---

# Krok 22 — opcjonalne logowanie zapytania EF Core

Jeżeli chcesz zobaczyć SQL generowany przez EF Core, w `ServiceCollectionExtensions.cs` możesz tymczasowo dodać:

```csharp
options.EnableSensitiveDataLogging();
options.EnableDetailedErrors();
```

Przykład:

```csharp
options.UseSqlServer(connectionString);
options.AddInterceptors(sessionContextInterceptor);
options.EnableDetailedErrors();
```

Uwaga:

`EnableSensitiveDataLogging` nie jest zalecane w produkcji.

Do demo lokalnego można użyć ostrożnie, ale na razie nie jest konieczne.

---

# Krok 23 — build końcowy

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

# Krok 24 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY010 Add patients list with RLS filtering"
```

---

# Kontrola końcowa DAY010

Lista kontrolna:

```text
[ ] Utworzono katalog Application\Patients
[ ] Utworzono katalog Infrastructure\Patients
[ ] Utworzono katalog Web\Pages\Patients
[ ] Utworzono PatientListItemDto
[ ] Utworzono IPatientReadService
[ ] Utworzono PatientReadService
[ ] Zarejestrowano IPatientReadService w DI
[ ] Utworzono Patients\Index.cshtml.cs
[ ] Utworzono Patients\Index.cshtml
[ ] Dodano link Pacjenci w menu
[ ] dotnet build kończy się sukcesem
[ ] /Patients działa
[ ] doctor.cardio widzi 10 pacjentów CARD
[ ] doctor.ortho widzi 10 pacjentów ORTH
[ ] doctor.neuro widzi 10 pacjentów NEUR
[ ] nurse.ped widzi 5 pacjentów PED
[ ] registration.emer widzi 5 pacjentów EMER
[ ] it.admin widzi 0 pacjentów
[ ] registration.user widzi 0 pacjentów
[ ] Kod aplikacji nie filtruje ręcznie po DepartmentId użytkownika
```

---

# Najczęstsze problemy

## Problem 1 — błąd: katalog Infrastructure\Patients nie istnieje

Objaw:

```text
Could not find a part of the path
```

Rozwiązanie:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Patients
```

Potem utwórz plik ponownie:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Patients\PatientReadService.cs
```

---

## Problem 2 — błąd: IPatientReadService nie jest zarejestrowany

Objaw:

```text
Unable to resolve service for type 'HospitalAccessControl.Application.Patients.IPatientReadService'
```

Rozwiązanie:

Sprawdź `ServiceCollectionExtensions.cs`.

Powinno być:

```csharp
services.AddScoped<IPatientReadService, PatientReadService>();
```

oraz `using`:

```csharp
using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Infrastructure.Patients;
```

---

## Problem 3 — wszyscy widzą 40 pacjentów

To oznacza, że RLS nie działa albo polityka jest wyłączona.

Sprawdź stan polityki:

```sql
SELECT
    SCHEMA_NAME(schema_id) AS SchemaName,
    name,
    is_enabled
FROM sys.security_policies
WHERE name = N'HospitalDepartmentSecurityPolicy';
```

Wynik powinien mieć:

```text
is_enabled = 1
```

Jeżeli jest `0`, włącz:

```sql
ALTER SECURITY POLICY security.HospitalDepartmentSecurityPolicy
WITH (STATE = ON);
```

---

## Problem 4 — wszyscy widzą 0 pacjentów

Najczęstsze przyczyny:

- `SESSION_CONTEXT` nie jest ustawiany,
- `DevelopmentUser.DomainLogin` nie zgadza się z `security.ApplicationUsers.DomainLogin`,
- interceptor nie działa,
- aplikacja działa z innym `appsettings`.

Sprawdź stronę główną z DAY008:

```text
SESSION_CONTEXT('CurrentUser')
```

Powinna pokazywać np.:

```text
HOSPITAL\doctor.cardio
```

Sprawdź w bazie:

```sql
SELECT DomainLogin
FROM security.ApplicationUsers
ORDER BY ApplicationUserId;
```

---

## Problem 5 — błąd JSON przy zmianie użytkownika

W JSON backslash musi być zapisany jako:

```json
"HOSPITAL\\doctor.cardio"
```

Nie tak:

```json
"HOSPITAL\doctor.cardio"
```

---

## Problem 6 — błąd namespace w Razor Page

Sprawdź, czy w `Index.cshtml.cs` masz:

```csharp
namespace HospitalAccessControl.Web.Pages.Patients;
```

A w `Index.cshtml`:

```cshtml
@model HospitalAccessControl.Web.Pages.Patients.IndexModel
```

---

## Problem 7 — błąd z DateOnly w widoku

Jeżeli wyświetlanie `DateOnly` sprawia problem, możesz tymczasowo zmienić:

```cshtml
<td>@patient.DateOfBirth</td>
```

na:

```cshtml
<td>@patient.DateOfBirth.ToString("yyyy-MM-dd")</td>
```

---

# Efekt końcowy DAY010

Po zakończeniu DAY010 aplikacja ma pierwszą realną funkcję biznesową:

```text
Lista pacjentów
```

i pokazuje działanie bezpieczeństwa w praktyce:

```text
różny użytkownik
        ↓
SESSION_CONTEXT
        ↓
RLS
        ↓
różna lista pacjentów
```

To jest pierwszy bardzo dobry moment demonstracyjny projektu.

Następny etap:

```text
DAY011 — Szczegóły pacjenta
```

W DAY011 dodamy stronę:

```text
/Patients/Details/{id}
```

i sprawdzimy, że użytkownik nie może wejść w szczegóły pacjenta z innego oddziału nawet przez ręcznie wpisany adres URL.

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach dziesiątego etapu implementacji przygotowano pierwszą funkcję biznesową aplikacji — listę pacjentów. Utworzono obiekt DTO `PatientListItemDto`, interfejs `IPatientReadService`, implementację `PatientReadService` oraz stronę Razor Pages `/Patients`. Lista pacjentów pobierana jest z bazy danych bez ręcznego filtrowania po oddziale w kodzie aplikacji. Ograniczenie widoczności rekordów realizowane jest przez mechanizm Row-Level Security w SQL Server na podstawie wartości `SESSION_CONTEXT(N'CurrentUser')`. Testy ręczne potwierdziły, że różni użytkownicy widzą wyłącznie pacjentów przypisanych do swoich oddziałów, a użytkownicy bez przypisania do oddziału nie widzą danych medycznych.
