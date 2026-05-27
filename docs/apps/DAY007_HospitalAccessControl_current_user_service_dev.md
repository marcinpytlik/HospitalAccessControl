# DAY007 — HospitalAccessControl  
## CurrentUserService DEV — symulowany użytkownik domenowy

## Cel dnia

Celem DAY007 jest przygotowanie mechanizmu rozpoznawania aktualnego użytkownika aplikacji w trybie developerskim.

Na tym etapie **nie używamy jeszcze prawdziwego Active Directory**.

Zamiast tego aplikacja będzie pobierała użytkownika z pliku:

```text
appsettings.Development.json
```

Dzięki temu lokalnie na swoim komputerze możesz testować różne scenariusze dostępu, np.:

```text
HOSPITAL\doctor.cardio
HOSPITAL\doctor.ortho
HOSPITAL\nurse.cardio
HOSPITAL\registration.user
HOSPITAL\auditor.user
HOSPITAL\it.admin
```

bez logowania się na różne konta Windows.

---

## Efekt końcowy DAY007

Po zakończeniu tego dnia aplikacja będzie umiała:

- odczytać tryb bezpieczeństwa `Development`,
- odczytać symulowanego użytkownika z konfiguracji,
- udostępnić aktualnego użytkownika przez interfejs `ICurrentUserService`,
- pokazać aktualnego użytkownika na stronie głównej aplikacji,
- przygotować fundament pod `SESSION_CONTEXT` w DAY008.

---

## Dlaczego to robimy?

Docelowo aplikacja w środowisku labowym będzie działała z:

```text
Active Directory
Windows Authentication
APP01
SQL01
DC01
```

Ale podczas developmentu nie chcemy blokować pracy brakiem domeny.

Dlatego wprowadzamy dwa tryby:

```text
Development — użytkownik symulowany z konfiguracji
Windows     — użytkownik odczytany z HttpContext.User.Identity.Name
```

W DAY007 implementujemy tylko tryb:

```text
Development
```

Tryb `Windows` dodamy później.

---

## Docelowa logika

W trybie DEV aplikacja odczyta z konfiguracji:

```json
{
  "SecurityMode": "Development",
  "DevelopmentUser": {
    "DomainLogin": "HOSPITAL\\doctor.cardio",
    "SamAccountName": "doctor.cardio",
    "DisplayName": "Jan Kardiolog"
  }
}
```

A następnie `ICurrentUserService` zwróci:

```text
DomainLogin: HOSPITAL\doctor.cardio
SamAccountName: doctor.cardio
DisplayName: Jan Kardiolog
IsAuthenticated: true
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

Sprawdź build po DAY006:

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

# Krok 2 — utworzenie katalogów

Tworzymy katalogi dla obsługi aktualnego użytkownika.

## W projekcie Application

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Application\Common\Security
```

## W projekcie Web

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Web\Services
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application\Common
Get-ChildItem .\src\HospitalAccessControl.Web
```

---

# Krok 3 — usunięcie domyślnej klasy Class1.cs z Application

Jeżeli istnieje plik:

```text
src\HospitalAccessControl.Application\Class1.cs
```

usuń go:

```powershell
Remove-Item .\src\HospitalAccessControl.Application\Class1.cs -ErrorAction SilentlyContinue
```

---

# Krok 4 — utworzenie plików

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Common\Security\CurrentUserDto.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Common\Security\ICurrentUserService.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Services\DevelopmentUserOptions.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Web\Services\DevelopmentCurrentUserService.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Application\Common\Security
Get-ChildItem .\src\HospitalAccessControl.Web\Services
```

---

# Krok 5 — CurrentUserDto

## Plik

```text
src\HospitalAccessControl.Application\Common\Security\CurrentUserDto.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Common.Security;

public sealed class CurrentUserDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public string SamAccountName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public bool IsAuthenticated { get; init; }
}
```

## Po co jest `CurrentUserDto`?

Ten obiekt reprezentuje aktualnego użytkownika aplikacji.

Na razie będzie pochodził z konfiguracji DEV.

Później, w środowisku labowym, będzie pochodził z Windows Authentication.

---

# Krok 6 — ICurrentUserService

## Plik

```text
src\HospitalAccessControl.Application\Common\Security\ICurrentUserService.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Common.Security;

public interface ICurrentUserService
{
    CurrentUserDto GetCurrentUser();
}
```

## Dlaczego interfejs jest w Application?

Warstwa Application nie powinna znać szczegółów ASP.NET Core ani `HttpContext`.

Dlatego definiujemy tylko kontrakt:

```text
daj mi aktualnego użytkownika
```

A implementacja może być różna:

```text
DevelopmentCurrentUserService
WindowsCurrentUserService
```

---

# Krok 7 — DevelopmentUserOptions

## Plik

```text
src\HospitalAccessControl.Web\Services\DevelopmentUserOptions.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Web.Services;

public sealed class DevelopmentUserOptions
{
    public string DomainLogin { get; set; } = string.Empty;

    public string SamAccountName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}
```

## Po co jest ta klasa?

Ta klasa mapuje fragment konfiguracji:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

na obiekt C#.

---

# Krok 8 — DevelopmentCurrentUserService

## Plik

```text
src\HospitalAccessControl.Web\Services\DevelopmentCurrentUserService.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Application.Common.Security;
using Microsoft.Extensions.Options;

namespace HospitalAccessControl.Web.Services;

public sealed class DevelopmentCurrentUserService : ICurrentUserService
{
    private readonly DevelopmentUserOptions _options;

    public DevelopmentCurrentUserService(IOptions<DevelopmentUserOptions> options)
    {
        _options = options.Value;
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
                IsAuthenticated = false
            };
        }

        return new CurrentUserDto
        {
            DomainLogin = _options.DomainLogin,
            SamAccountName = _options.SamAccountName,
            DisplayName = _options.DisplayName,
            IsAuthenticated = true
        };
    }
}
```

## Co robi ta klasa?

Czyta użytkownika z konfiguracji i zwraca go do aplikacji.

Przykład:

```text
HOSPITAL\doctor.cardio
```

---

# Krok 9 — konfiguracja appsettings.Development.json

Otwórz plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Uzupełnij go tak, aby zawierał `SecurityMode` i `DevelopmentUser`.

## Wariant dla lokalnego SQL Server Developer / default instance

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "SecurityMode": "Development",
  "DevelopmentUser": {
    "DomainLogin": "HOSPITAL\\doctor.cardio",
    "SamAccountName": "doctor.cardio",
    "DisplayName": "Jan Kardiolog"
  }
}
```

## Wariant dla SQL Express

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=.\\SQLEXPRESS;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "SecurityMode": "Development",
  "DevelopmentUser": {
    "DomainLogin": "HOSPITAL\\doctor.cardio",
    "SamAccountName": "doctor.cardio",
    "DisplayName": "Jan Kardiolog"
  }
}
```

---

# Krok 10 — rejestracja CurrentUserService w Program.cs

Otwórz plik:

```text
src\HospitalAccessControl.Web\Program.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Web.Services;
```

Następnie znajdź fragment:

```csharp
builder.Services.AddRazorPages();
```

Pod spodem dodaj:

```csharp
builder.Services.Configure<DevelopmentUserOptions>(
    builder.Configuration.GetSection("DevelopmentUser"));

var securityMode = builder.Configuration["SecurityMode"];

if (string.Equals(securityMode, "Development", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported SecurityMode: '{securityMode}'.");
}
```

## Docelowy fragment Program.cs

Fragment powinien wyglądać podobnie:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.DependencyInjection;
using HospitalAccessControl.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<DevelopmentUserOptions>(
    builder.Configuration.GetSection("DevelopmentUser"));

var securityMode = builder.Configuration["SecurityMode"];

if (string.Equals(securityMode, "Development", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported SecurityMode: '{securityMode}'.");
}

var app = builder.Build();
```

Reszta pliku może zostać taka, jak była.

---

# Krok 11 — build po rejestracji usługi

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

# Krok 12 — wyświetlenie aktualnego użytkownika na stronie głównej

Teraz dodamy prosty test na stronie głównej.

## Plik PageModel

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Index.cshtml.cs
```

Zastąp zawartość pliku:

```csharp
using HospitalAccessControl.Application.Common.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public void OnGet()
    {
        CurrentUser = _currentUserService.GetCurrentUser();
    }
}
```

---

## Plik widoku

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Index.cshtml
```

Zastąp zawartość pliku:

```cshtml
@page
@model IndexModel

@{
    ViewData["Title"] = "HospitalAccessControl";
}

<div class="text-center">
    <h1 class="display-4">HospitalAccessControl</h1>
    <p>Demonstracyjna aplikacja kontroli dostępu do danych medycznych.</p>
</div>

<hr />

<h2>Aktualny użytkownik DEV</h2>

<table class="table">
    <tbody>
        <tr>
            <th>IsAuthenticated</th>
            <td>@Model.CurrentUser.IsAuthenticated</td>
        </tr>
        <tr>
            <th>DomainLogin</th>
            <td>@Model.CurrentUser.DomainLogin</td>
        </tr>
        <tr>
            <th>SamAccountName</th>
            <td>@Model.CurrentUser.SamAccountName</td>
        </tr>
        <tr>
            <th>DisplayName</th>
            <td>@Model.CurrentUser.DisplayName</td>
        </tr>
    </tbody>
</table>
```

## Po co to robimy?

To jest prosty test, czy DI i `ICurrentUserService` działają.

Na stronie głównej powinieneś zobaczyć:

```text
HOSPITAL\doctor.cardio
doctor.cardio
Jan Kardiolog
```

---

# Krok 13 — uruchomienie aplikacji

Wykonaj:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz adres z terminala, np.:

```text
https://localhost:xxxx
```

Na stronie powinieneś zobaczyć sekcję:

```text
Aktualny użytkownik DEV
```

oraz dane:

```text
IsAuthenticated: True
DomainLogin: HOSPITAL\doctor.cardio
SamAccountName: doctor.cardio
DisplayName: Jan Kardiolog
```

Zatrzymanie aplikacji:

```text
CTRL + C
```

---

# Krok 14 — test zmiany użytkownika

Otwórz:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Zmień użytkownika z:

```json
"DomainLogin": "HOSPITAL\\doctor.cardio",
"SamAccountName": "doctor.cardio",
"DisplayName": "Jan Kardiolog"
```

na:

```json
"DomainLogin": "HOSPITAL\\doctor.ortho",
"SamAccountName": "doctor.ortho",
"DisplayName": "Marek Ortopeda"
```

Uruchom aplikację ponownie:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Oczekiwany wynik na stronie:

```text
DomainLogin: HOSPITAL\doctor.ortho
SamAccountName: doctor.ortho
DisplayName: Marek Ortopeda
```

To potwierdza, że lokalnie możesz symulować różnych użytkowników bez AD.

---

# Krok 15 — lista użytkowników do testów DEV

Możesz testować następujące wartości:

## Lekarz kardiologii

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

## Lekarz ortopedii

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.ortho",
  "SamAccountName": "doctor.ortho",
  "DisplayName": "Marek Ortopeda"
}
```

## Lekarz neurologii

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.neuro",
  "SamAccountName": "doctor.neuro",
  "DisplayName": "Anna Neurolog"
}
```

## Pielęgniarka kardiologii

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\nurse.cardio",
  "SamAccountName": "nurse.cardio",
  "DisplayName": "Ewa Pielęgniarka Kardiologia"
}
```

## Rejestracja ogólna

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\registration.user",
  "SamAccountName": "registration.user",
  "DisplayName": "Karolina Rejestracja"
}
```

## Audytor

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\auditor.user",
  "SamAccountName": "auditor.user",
  "DisplayName": "Alicja Audytor"
}
```

## Administrator IT

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\it.admin",
  "SamAccountName": "it.admin",
  "DisplayName": "Adam Administrator IT"
}
```

---

# Krok 16 — sprawdzenie, czy użytkownik istnieje w bazie

Na razie `DevelopmentCurrentUserService` tylko czyta konfigurację.

Ale możemy sprawdzić, czy taki użytkownik jest już w tabeli:

```text
security.ApplicationUsers
```

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationUserId, DomainLogin, SamAccountName, DisplayName, IsActive FROM security.ApplicationUsers ORDER BY ApplicationUserId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationUserId, DomainLogin, SamAccountName, DisplayName, IsActive FROM security.ApplicationUsers ORDER BY ApplicationUserId;"
```

Powinieneś zobaczyć użytkowników z DAY006.

---

# Krok 17 — build końcowy

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

# Krok 18 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY007 Add development current user service"
```

---

# Kontrola końcowa DAY007

Lista kontrolna:

```text
[ ] Utworzono katalog Application\Common\Security
[ ] Utworzono katalog Web\Services
[ ] Utworzono CurrentUserDto
[ ] Utworzono ICurrentUserService
[ ] Utworzono DevelopmentUserOptions
[ ] Utworzono DevelopmentCurrentUserService
[ ] Dodano SecurityMode do appsettings.Development.json
[ ] Dodano DevelopmentUser do appsettings.Development.json
[ ] Zarejestrowano ICurrentUserService w Program.cs
[ ] Index.cshtml.cs używa ICurrentUserService
[ ] Index.cshtml pokazuje aktualnego użytkownika
[ ] dotnet build kończy się sukcesem
[ ] dotnet run uruchamia aplikację
[ ] Na stronie widać HOSPITAL\doctor.cardio
[ ] Zmiana użytkownika w appsettings.Development.json działa
```

---

# Najczęstsze problemy

## Problem 1 — aplikacja nie widzi ICurrentUserService

Objaw:

```text
Unable to resolve service for type 'HospitalAccessControl.Application.Common.Security.ICurrentUserService'
```

Rozwiązanie:

Sprawdź, czy w `Program.cs` masz:

```csharp
builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
```

Sprawdź też, czy masz odpowiednie `using`:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Web.Services;
```

---

## Problem 2 — błąd SecurityMode

Objaw:

```text
Unsupported SecurityMode: ''
```

albo:

```text
Unsupported SecurityMode: 'null'
```

Rozwiązanie:

Sprawdź plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Powinien zawierać:

```json
"SecurityMode": "Development"
```

Sprawdź też, czy aplikacja działa w środowisku Development:

```powershell
$env:ASPNETCORE_ENVIRONMENT
```

Jeżeli trzeba:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

---

## Problem 3 — brak danych użytkownika na stronie

Objaw:

```text
DomainLogin jest pusty
DisplayName = Anonymous
IsAuthenticated = False
```

Rozwiązanie:

Sprawdź sekcję:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.cardio",
  "SamAccountName": "doctor.cardio",
  "DisplayName": "Jan Kardiolog"
}
```

Uwaga:

W JSON dla backslasha używamy podwójnego znaku:

```text
\\
```

Czyli:

```json
"HOSPITAL\\doctor.cardio"
```

a nie:

```json
"HOSPITAL\doctor.cardio"
```

---

## Problem 4 — błąd składni JSON

Objaw:

```text
Failed to load configuration
```

albo aplikacja nie startuje.

Rozwiązanie:

Sprawdź przecinki w `appsettings.Development.json`.

Poprawny przykład:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "SecurityMode": "Development",
  "DevelopmentUser": {
    "DomainLogin": "HOSPITAL\\doctor.cardio",
    "SamAccountName": "doctor.cardio",
    "DisplayName": "Jan Kardiolog"
  }
}
```

---

## Problem 5 — Web nie widzi namespace z Application

Objaw:

```text
The type or namespace name 'Application' does not exist in the namespace 'HospitalAccessControl'
```

Rozwiązanie:

Sprawdź referencję Web do Application:

```powershell
dotnet list .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference
```

Jeżeli jej nie ma:

```powershell
dotnet add .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
```

---

# Efekt końcowy DAY007

Po zakończeniu DAY007 aplikacja lokalnie rozpoznaje symulowanego użytkownika domenowego.

Przykład:

```text
HOSPITAL\doctor.cardio
```

Dzięki temu możemy przejść do kolejnego kroku:

```text
DAY008 — SESSION_CONTEXT dla SQL Server
```

W DAY008 aplikacja zacznie przekazywać aktualnego użytkownika do SQL Server przez:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

To będzie fundament pod:

```text
DAY009 — Row-Level Security
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach siódmego etapu implementacji przygotowano mechanizm odczytu aktualnego użytkownika aplikacji w trybie developerskim. Utworzono interfejs `ICurrentUserService`, obiekt `CurrentUserDto` oraz implementację `DevelopmentCurrentUserService`, która pobiera symulowanego użytkownika domenowego z pliku konfiguracyjnego `appsettings.Development.json`. Dzięki temu możliwe jest lokalne testowanie scenariuszy dostępu dla różnych użytkowników bez konieczności posiadania gotowego środowiska Active Directory. Mechanizm ten stanowi podstawę do późniejszego przekazywania tożsamości użytkownika do SQL Server za pomocą `SESSION_CONTEXT`.
