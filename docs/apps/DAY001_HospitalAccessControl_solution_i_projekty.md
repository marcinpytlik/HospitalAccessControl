# DAY001 — HospitalAccessControl  
## Utworzenie solution, projektów, referencji i pierwszego builda

## Cel dnia

Celem DAY001 jest przygotowanie pustego, ale poprawnie zbudowanego szkieletu aplikacji **HospitalAccessControl** w technologii **.NET 10**.

Po zakończeniu tego dnia powinieneś mieć:

- katalog projektu,
- solution `.sln`,
- projekty warstwowe,
- projekty testowe,
- referencje między projektami,
- podstawowe paczki NuGet,
- działający `dotnet build`.

---

## Założenia

Projekt piszemy lokalnie na komputerze developerskim, bez konieczności posiadania gotowej infrastruktury:

```text
DC01
SQL01
APP01
CLIENT01
```

Na tym etapie nie potrzebujemy jeszcze:

- Active Directory,
- SQL Server,
- IIS,
- APP01,
- RLS,
- bazy danych.

Na DAY001 potrzebujesz tylko:

```text
Windows 11
PowerShell
Visual Studio Code
.NET 10 SDK
Git — opcjonalnie
```

---

## Docelowa struktura katalogów

Po wykonaniu DAY001 projekt będzie wyglądał mniej więcej tak:

```text
HospitalAccessControl
├── HospitalAccessControl.sln
├── src
│   ├── HospitalAccessControl.Domain
│   ├── HospitalAccessControl.Application
│   ├── HospitalAccessControl.Infrastructure
│   └── HospitalAccessControl.Web
│
├── tests
│   ├── HospitalAccessControl.Tests
│   └── HospitalAccessControl.IntegrationTests
│
├── docs
├── sql
└── scripts
```

---

## Warstwy aplikacji

| Projekt | Typ | Przeznaczenie |
|---|---|---|
| `HospitalAccessControl.Domain` | Class Library | Encje domenowe i podstawowe modele |
| `HospitalAccessControl.Application` | Class Library | DTO, interfejsy, usługi aplikacyjne |
| `HospitalAccessControl.Infrastructure` | Class Library | EF Core, SQL Server, konfiguracje encji |
| `HospitalAccessControl.Web` | Razor Pages | Interfejs webowy aplikacji |
| `HospitalAccessControl.Tests` | xUnit | Testy jednostkowe |
| `HospitalAccessControl.IntegrationTests` | xUnit | Testy integracyjne |

---

# Krok 1 — utworzenie katalogu projektu

Otwórz PowerShell.

Utwórz katalog projektu:

```powershell
New-Item -ItemType Directory -Force C:\Projects\HospitalAccessControl
Set-Location C:\Projects\HospitalAccessControl
```

Sprawdź, czy jesteś w dobrym katalogu:

```powershell
Get-Location
```

Oczekiwany wynik:

```text
Path
----
C:\Projects\HospitalAccessControl
```

---

# Krok 2 — utworzenie katalogów roboczych

```powershell
New-Item -ItemType Directory -Force .\src
New-Item -ItemType Directory -Force .\tests
New-Item -ItemType Directory -Force .\docs
New-Item -ItemType Directory -Force .\sql
New-Item -ItemType Directory -Force .\scripts
```

Sprawdź katalogi:

```powershell
Get-ChildItem
```

Powinieneś zobaczyć:

```text
docs
scripts
sql
src
tests
```

---

# Krok 3 — sprawdzenie .NET SDK

Sprawdź wersję .NET:

```powershell
dotnet --version
```

Sprawdź zainstalowane SDK:

```powershell
dotnet --list-sdks
```

Oczekujemy SDK dla .NET 10, czyli coś w stylu:

```text
10.0.xxx
```

Jeżeli nie masz SDK .NET 10, zatrzymaj się tutaj i doinstaluj .NET 10 SDK.

---

# Krok 4 — utworzenie solution

```powershell
dotnet new sln -n HospitalAccessControl
```

Po wykonaniu powinien powstać plik:

```text
HospitalAccessControl.sln
```

Sprawdź:

```powershell
Get-ChildItem *.sln
```

---

# Krok 5 — utworzenie projektów aplikacji

## Projekt Domain

```powershell
dotnet new classlib -n HospitalAccessControl.Domain -o .\src\HospitalAccessControl.Domain -f net10.0
```

## Projekt Application

```powershell
dotnet new classlib -n HospitalAccessControl.Application -o .\src\HospitalAccessControl.Application -f net10.0
```

## Projekt Infrastructure

```powershell
dotnet new classlib -n HospitalAccessControl.Infrastructure -o .\src\HospitalAccessControl.Infrastructure -f net10.0
```

## Projekt Web

```powershell
dotnet new webapp -n HospitalAccessControl.Web -o .\src\HospitalAccessControl.Web -f net10.0
```

---

# Krok 6 — utworzenie projektów testowych

## Testy jednostkowe

```powershell
dotnet new xunit -n HospitalAccessControl.Tests -o .\tests\HospitalAccessControl.Tests -f net10.0
```

## Testy integracyjne

```powershell
dotnet new xunit -n HospitalAccessControl.IntegrationTests -o .\tests\HospitalAccessControl.IntegrationTests -f net10.0
```

---

# Krok 7 — dodanie projektów do solution

```powershell
dotnet sln .\HospitalAccessControl.sln add .\src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
dotnet sln .\HospitalAccessControl.sln add .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
dotnet sln .\HospitalAccessControl.sln add .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj
dotnet sln .\HospitalAccessControl.sln add .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
dotnet sln .\HospitalAccessControl.sln add .\tests\HospitalAccessControl.Tests\HospitalAccessControl.Tests.csproj
dotnet sln .\HospitalAccessControl.sln add .\tests\HospitalAccessControl.IntegrationTests\HospitalAccessControl.IntegrationTests.csproj
```

Weryfikacja:

```powershell
dotnet sln .\HospitalAccessControl.sln list
```

Powinieneś zobaczyć wszystkie projekty:

```text
src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj
src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
tests\HospitalAccessControl.Tests\HospitalAccessControl.Tests.csproj
tests\HospitalAccessControl.IntegrationTests\HospitalAccessControl.IntegrationTests.csproj
```

---

# Krok 8 — dodanie referencji między projektami

## Application → Domain

```powershell
dotnet add .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj reference .\src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
```

## Infrastructure → Domain

```powershell
dotnet add .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj reference .\src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
```

## Infrastructure → Application

```powershell
dotnet add .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj reference .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
```

## Web → Application

```powershell
dotnet add .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
```

## Web → Infrastructure

```powershell
dotnet add .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj
```

## Tests → Domain

```powershell
dotnet add .\tests\HospitalAccessControl.Tests\HospitalAccessControl.Tests.csproj reference .\src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
```

## Tests → Application

```powershell
dotnet add .\tests\HospitalAccessControl.Tests\HospitalAccessControl.Tests.csproj reference .\src\HospitalAccessControl.Application\HospitalAccessControl.Application.csproj
```

## IntegrationTests → Web

```powershell
dotnet add .\tests\HospitalAccessControl.IntegrationTests\HospitalAccessControl.IntegrationTests.csproj reference .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

---

# Krok 9 — dodanie paczek NuGet

## Paczki dla Infrastructure

```powershell
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

Te paczki będą potrzebne do:

- `DbContext`,
- konfiguracji encji,
- SQL Server,
- migracji EF Core.

---

## Paczki dla Web

```powershell
dotnet add .\src\HospitalAccessControl.Web package Microsoft.EntityFrameworkCore.Design
dotnet add .\src\HospitalAccessControl.Web package Microsoft.AspNetCore.Authentication.Negotiate
```

`Microsoft.AspNetCore.Authentication.Negotiate` przyda się później do Windows Authentication w środowisku labowym.

Na DAY001 nie będziemy tego jeszcze konfigurować.

---

## Paczki dla testów

```powershell
dotnet add .\tests\HospitalAccessControl.Tests package FluentAssertions
dotnet add .\tests\HospitalAccessControl.IntegrationTests package FluentAssertions
dotnet add .\tests\HospitalAccessControl.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing
```

---

# Krok 10 — restore i build

Najpierw restore:

```powershell
dotnet restore
```

Potem build:

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

# Krok 11 — uruchomienie aplikacji webowej

Uruchom aplikację:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

W terminalu zobaczysz adres, np.:

```text
Now listening on: https://localhost:xxxx
Now listening on: http://localhost:yyyy
```

Otwórz przeglądarkę i wejdź na podany adres.

Na razie zobaczysz domyślną stronę Razor Pages.

Zatrzymanie aplikacji:

```text
CTRL + C
```

---

# Krok 12 — otwarcie projektu w VS Code

W katalogu projektu uruchom:

```powershell
code .
```

Jeśli komenda `code` nie działa, otwórz VS Code ręcznie i wybierz:

```text
File -> Open Folder -> C:\Projects\HospitalAccessControl
```

---

# Krok 13 — opcjonalny pierwszy commit Git

Jeśli używasz Gita:

```powershell
git init
git status
```

Utwórz `.gitignore`:

```powershell
dotnet new gitignore
```

Pierwszy commit:

```powershell
git add .
git commit -m "DAY001 Create solution and project structure"
```

---

# Kontrola końcowa DAY001

Przed zakończeniem dnia sprawdź:

```powershell
dotnet restore
dotnet build
dotnet sln .\HospitalAccessControl.sln list
```

Lista kontrolna:

```text
[ ] Istnieje katalog C:\Projects\HospitalAccessControl
[ ] Istnieje plik HospitalAccessControl.sln
[ ] Istnieje katalog src
[ ] Istnieje katalog tests
[ ] Istnieje projekt Domain
[ ] Istnieje projekt Application
[ ] Istnieje projekt Infrastructure
[ ] Istnieje projekt Web
[ ] Istnieje projekt Tests
[ ] Istnieje projekt IntegrationTests
[ ] Projekty są dodane do solution
[ ] Referencje między projektami są dodane
[ ] Paczki NuGet są zainstalowane
[ ] dotnet restore przechodzi
[ ] dotnet build kończy się sukcesem
[ ] dotnet run uruchamia aplikację webową
```

---

# Najczęstsze problemy

## Problem 1 — brak .NET 10 SDK

Objaw:

```text
Invalid framework identifier 'net10.0'
```

albo:

```text
The framework 'Microsoft.NETCore.App', version '10.0.0' was not found
```

Rozwiązanie:

Zainstaluj .NET 10 SDK, a potem sprawdź:

```powershell
dotnet --list-sdks
```

---

## Problem 2 — literówka w ścieżce

Objaw:

```text
Could not find project or directory
```

Rozwiązanie:

Sprawdź aktualny katalog:

```powershell
Get-Location
```

Sprawdź strukturę:

```powershell
Get-ChildItem
Get-ChildItem .\src
Get-ChildItem .\tests
```

---

## Problem 3 — projekt już istnieje

Objaw:

```text
Creating this template will make changes to existing files
```

Rozwiązanie:

Jeżeli chcesz zacząć od nowa, usuń katalog projektu:

```powershell
Remove-Item -Recurse -Force C:\Projects\HospitalAccessControl
```

Potem zacznij DAY001 od początku.

---

## Problem 4 — błąd restore paczek NuGet

Objaw:

```text
Unable to load the service index for source https://api.nuget.org/v3/index.json
```

Możliwe przyczyny:

- brak Internetu,
- proxy,
- blokada firmowa,
- problem z NuGet source.

Sprawdź źródła NuGet:

```powershell
dotnet nuget list source
```

---

# Efekt końcowy DAY001

Po zakończeniu DAY001 masz gotowy fundament:

```text
HospitalAccessControl.Domain
HospitalAccessControl.Application
HospitalAccessControl.Infrastructure
HospitalAccessControl.Web
HospitalAccessControl.Tests
HospitalAccessControl.IntegrationTests
```

Projekt się buduje i można przejść do:

```text
DAY002 — Encje domenowe
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach pierwszego etapu implementacji aplikacji utworzono strukturę solution .NET 10 składającą się z projektów warstwowych: Domain, Application, Infrastructure oraz Web. Dodano również projekty testowe dla testów jednostkowych i integracyjnych. Skonfigurowano zależności między projektami oraz zainstalowano podstawowe pakiety NuGet wymagane do dalszej implementacji aplikacji, Entity Framework Core, SQL Server oraz przyszłej integracji z Windows Authentication.
