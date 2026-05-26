# DAY000 — HospitalAccessControl  
## Przygotowanie stacji developerskiej do pisania kodu

## Cel dnia

Celem DAY000 jest przygotowanie komputera developerskiego do pracy nad aplikacją **HospitalAccessControl**.

Ten dzień wykonujemy **przed DAY001**, czyli zanim utworzymy solution `.NET`.

Po zakończeniu DAY000 powinieneś mieć przygotowane:

- system Windows,
- PowerShell,
- Visual Studio Code,
- rozszerzenia VS Code,
- .NET 10 SDK,
- Git,
- SQL Server lokalny,
- narzędzia EF Core,
- opcjonalnie `sqlcmd`,
- katalog roboczy projektu,
- podstawowe komendy weryfikacyjne.

---

## Założenie

Aplikację piszemy lokalnie na komputerze developerskim.

Na tym etapie **nie potrzebujesz jeszcze**:

```text
DC01
SQL01
APP01
CLIENT01
Active Directory
IIS
środowiska domenowego hospital.local
```

Na komputerze developerskim będziemy pisać aplikację w trybie DEV:

```text
lokalny Windows
lokalny SQL Server
symulowany użytkownik domenowy
SESSION_CONTEXT w SQL Server
```

Dopiero później aplikacja zostanie przeniesiona do środowiska laboratoryjnego.

---

# 1. System operacyjny

Rekomendowany system:

```text
Windows 11 Pro
```

Minimalnie może być:

```text
Windows 10/11
```

Ale do projektu najlepiej:

```text
Windows 11 Pro
```

Dlaczego?

- dobre wsparcie dla .NET,
- wygodna praca z PowerShell,
- możliwość instalacji SQL Server Developer,
- zgodność z docelowym środowiskiem Windows,
- łatwiejsze testowanie Windows Authentication w późniejszym etapie.

---

# 2. Katalog roboczy

Proponowany katalog projektu:

```text
C:\Projects\HospitalAccessControl
```

Utworzenie katalogu:

```powershell
New-Item -ItemType Directory -Force C:\Projects\HospitalAccessControl
```

Przejście do katalogu:

```powershell
Set-Location C:\Projects\HospitalAccessControl
```

Weryfikacja:

```powershell
Get-Location
```

Oczekiwany wynik:

```text
C:\Projects\HospitalAccessControl
```

---

# 3. PowerShell

Rekomenduję używać:

```text
PowerShell 7
```

Sprawdzenie wersji:

```powershell
$PSVersionTable
```

Najważniejsze pole:

```text
PSVersion
```

Dobrze, jeśli wersja zaczyna się od:

```text
7.x
```

Jeżeli używasz Windows PowerShell 5.1, też da się pracować, ale PowerShell 7 jest wygodniejszy.

---

# 4. .NET 10 SDK

## Co instalujemy?

Potrzebujesz:

```text
.NET 10 SDK
```

Nie wystarczy sam runtime.

Musi być SDK, bo będziemy używać:

```text
dotnet new
dotnet build
dotnet run
dotnet test
dotnet ef
dotnet publish
```

---

## Weryfikacja .NET

Sprawdź wersję:

```powershell
dotnet --version
```

Sprawdź zainstalowane SDK:

```powershell
dotnet --list-sdks
```

Oczekiwany wynik powinien zawierać wersję:

```text
10.0.xxx
```

Przykład:

```text
10.0.100 [C:\Program Files\dotnet\sdk]
```

---

## Weryfikacja runtime

```powershell
dotnet --list-runtimes
```

Powinieneś zobaczyć wpisy związane z:

```text
Microsoft.NETCore.App 10.x
Microsoft.AspNetCore.App 10.x
```

---

# 5. Visual Studio Code

## Co instalujemy?

Instalujemy:

```text
Visual Studio Code
```

To będzie główne środowisko pracy.

Po instalacji sprawdź, czy działa komenda:

```powershell
code --version
```

Jeżeli działa, możesz otworzyć katalog projektu z PowerShell:

```powershell
code C:\Projects\HospitalAccessControl
```

Albo po wejściu do katalogu:

```powershell
code .
```

---

# 6. Rozszerzenia Visual Studio Code

Poniżej lista rozszerzeń, które rekomenduję do tego projektu.

## 6.1. C# Dev Kit

Nazwa:

```text
C# Dev Kit
```

Identyfikator:

```text
ms-dotnettools.csdevkit
```

Instalacja z PowerShell:

```powershell
code --install-extension ms-dotnettools.csdevkit
```

Do czego służy?

- obsługa projektów C#,
- lepsza nawigacja po solution,
- IntelliSense,
- podgląd testów,
- integracja z .NET.

---

## 6.2. C# extension

Nazwa:

```text
C#
```

Identyfikator:

```text
ms-dotnettools.csharp
```

Instalacja:

```powershell
code --install-extension ms-dotnettools.csharp
```

Do czego służy?

- podstawowe wsparcie dla C#,
- IntelliSense,
- debugowanie,
- obsługa składni.

---

## 6.3. .NET Install Tool

Nazwa:

```text
.NET Install Tool
```

Identyfikator:

```text
ms-dotnettools.vscode-dotnet-runtime
```

Instalacja:

```powershell
code --install-extension ms-dotnettools.vscode-dotnet-runtime
```

Do czego służy?

- pomaga VS Code wykrywać runtime .NET,
- przydatne dla rozszerzeń C#.

---

## 6.4. SQL Server extension

Nazwa:

```text
SQL Server (mssql)
```

Identyfikator:

```text
ms-mssql.mssql
```

Instalacja:

```powershell
code --install-extension ms-mssql.mssql
```

Do czego służy?

- połączenie z SQL Server z poziomu VS Code,
- wykonywanie zapytań SQL,
- przeglądanie wyników,
- testowanie bazy `HospitalAccessControlDb_Dev`.

---

## 6.5. PowerShell

Nazwa:

```text
PowerShell
```

Identyfikator:

```text
ms-vscode.PowerShell
```

Instalacja:

```powershell
code --install-extension ms-vscode.PowerShell
```

Do czego służy?

- edycja skryptów PowerShell,
- kolorowanie składni,
- IntelliSense,
- uruchamianie skryptów `.ps1`.

---

## 6.6. GitLens

Nazwa:

```text
GitLens
```

Identyfikator:

```text
eamodio.gitlens
```

Instalacja:

```powershell
code --install-extension eamodio.gitlens
```

Do czego służy?

- wygodna praca z historią Git,
- podgląd zmian,
- historia plików,
- analiza commitów.

---

## 6.7. EditorConfig

Nazwa:

```text
EditorConfig for VS Code
```

Identyfikator:

```text
EditorConfig.EditorConfig
```

Instalacja:

```powershell
code --install-extension EditorConfig.EditorConfig
```

Do czego służy?

- spójne formatowanie kodu,
- jednolite wcięcia,
- zasady końca linii,
- przydatne w repozytorium.

---

## 6.8. Markdown All in One

Nazwa:

```text
Markdown All in One
```

Identyfikator:

```text
yzhang.markdown-all-in-one
```

Instalacja:

```powershell
code --install-extension yzhang.markdown-all-in-one
```

Do czego służy?

- wygodna edycja dokumentacji Markdown,
- spisy treści,
- skróty,
- podgląd dokumentacji.

---

## 6.9. Markdown Preview Mermaid Support

Nazwa:

```text
Markdown Preview Mermaid Support
```

Identyfikator:

```text
bierner.markdown-mermaid
```

Instalacja:

```powershell
code --install-extension bierner.markdown-mermaid
```

Do czego służy?

- podgląd diagramów Mermaid w Markdown,
- przydatne do ERD,
- przydatne do diagramów architektury.

---

## 6.10. XML

Nazwa:

```text
XML
```

Identyfikator:

```text
redhat.vscode-xml
```

Instalacja:

```powershell
code --install-extension redhat.vscode-xml
```

Do czego służy?

- edycja plików XML,
- przydatne przy `.csproj`,
- przydatne przy konfiguracjach.

---

## 6.11. NuGet Gallery

Nazwa:

```text
NuGet Gallery
```

Identyfikator:

```text
patcx.vscode-nuget-gallery
```

Instalacja:

```powershell
code --install-extension patcx.vscode-nuget-gallery
```

Do czego służy?

- przeglądanie paczek NuGet z VS Code,
- szybkie sprawdzanie wersji paczek.

To rozszerzenie jest opcjonalne, bo paczki i tak będziemy dodawać komendami `dotnet add package`.

---

## 6.12. REST Client

Nazwa:

```text
REST Client
```

Identyfikator:

```text
humao.rest-client
```

Instalacja:

```powershell
code --install-extension humao.rest-client
```

Do czego służy?

- testowanie endpointów HTTP,
- przyda się, jeśli później dodamy API,
- opcjonalne przy Razor Pages.

---

# 7. Instalacja wszystkich rozszerzeń jedną komendą

Możesz wkleić całość:

```powershell
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.vscode-dotnet-runtime
code --install-extension ms-mssql.mssql
code --install-extension ms-vscode.PowerShell
code --install-extension eamodio.gitlens
code --install-extension EditorConfig.EditorConfig
code --install-extension yzhang.markdown-all-in-one
code --install-extension bierner.markdown-mermaid
code --install-extension redhat.vscode-xml
code --install-extension patcx.vscode-nuget-gallery
code --install-extension humao.rest-client
```

Weryfikacja zainstalowanych rozszerzeń:

```powershell
code --list-extensions
```

Możesz też sprawdzić konkretne:

```powershell
code --list-extensions | Select-String "csdevkit"
code --list-extensions | Select-String "mssql"
code --list-extensions | Select-String "PowerShell"
```

---

# 8. Git

## Instalacja

Potrzebujesz:

```text
Git for Windows
```

## Weryfikacja

```powershell
git --version
```

Oczekiwany wynik:

```text
git version x.y.z.windows.x
```

## Podstawowa konfiguracja

Ustaw nazwę:

```powershell
git config --global user.name "Marcin"
```

Ustaw e-mail:

```powershell
git config --global user.email "marcinpytlik73@gmail.com"
```

Sprawdź konfigurację:

```powershell
git config --global --list
```

---

# 9. SQL Server lokalny

## Rekomendacja

Do projektu rekomenduję:

```text
SQL Server 2022 Developer Edition
```

Dlaczego?

- zgodność z docelowym projektem,
- pełne funkcje SQL Server,
- możliwość testowania RLS,
- możliwość testowania Dynamic Data Masking,
- możliwość testowania SQL Server Audit,
- wygodniejsze środowisko niż LocalDB.

Alternatywnie możesz użyć:

```text
SQL Server Express
```

ale dla tej pracy najlepszy jest Developer Edition.

---

## Sprawdzenie usługi SQL Server

### Default instance

```powershell
Get-Service MSSQLSERVER
```

Oczekiwany stan:

```text
Running
```

### SQL Express

```powershell
Get-Service MSSQL`$SQLEXPRESS
```

Oczekiwany stan:

```text
Running
```

## Uruchomienie usługi

### Default instance

```powershell
Start-Service MSSQLSERVER
```

### SQL Express

```powershell
Start-Service MSSQL`$SQLEXPRESS
```

---

# 10. sqlcmd

`sqlcmd` jest opcjonalne, ale bardzo wygodne.

Będziemy go używać do szybkiej weryfikacji bazy, np.:

```powershell
sqlcmd -S localhost -E -Q "SELECT @@VERSION;"
```

## Weryfikacja

```powershell
sqlcmd -?
```

Jeżeli komenda działa, jest dobrze.

## Test połączenia — default instance

```powershell
sqlcmd -S localhost -E -Q "SELECT @@SERVERNAME AS ServerName, @@VERSION AS Version;"
```

## Test połączenia — SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@SERVERNAME AS ServerName, @@VERSION AS Version;"
```

Jeżeli `sqlcmd` nie jest zainstalowany, możesz wykonywać zapytania przez:

```text
VS Code SQL Server extension
SSMS
Azure Data Studio — opcjonalnie
```

W tym projekcie będziemy preferować VS Code.

---

# 11. Narzędzie dotnet-ef

Do migracji EF Core potrzebujesz narzędzia:

```text
dotnet-ef
```

## Sprawdzenie

```powershell
dotnet ef --version
```

Jeżeli działa, zobaczysz numer wersji.

## Instalacja

```powershell
dotnet tool install --global dotnet-ef
```

## Aktualizacja

```powershell
dotnet tool update --global dotnet-ef
```

## Weryfikacja po instalacji

```powershell
dotnet ef --version
```

Jeżeli po instalacji komenda nie działa, zamknij PowerShell i otwórz ponownie.

---

# 12. Opcjonalnie: SSMS

To jest opcjonalne.

Ponieważ preferowanym środowiskiem jest VS Code, SQL będziemy robić głównie przez:

```text
VS Code SQL Server extension
sqlcmd
skrypty PowerShell
```

Ale SSMS może być przydatny do:

- szybkiego podglądu bazy,
- sprawdzenia tabel,
- testowania zapytań,
- podglądu uprawnień.

---

# 13. Opcjonalnie: Docker Desktop

Do tego projektu **nie jest wymagany** Docker.

Nie używamy Dockera w głównym scenariuszu.

Środowisko projektowe jest oparte o:

```text
Windows
SQL Server lokalny
.NET 10
VS Code
PowerShell
```

---

# 14. Proponowane ustawienia VS Code

## Katalog `.vscode`

W repozytorium później utworzymy:

```text
.vscode
```

i pliki:

```text
settings.json
extensions.json
tasks.json
launch.json
```

Na DAY000 jeszcze nie musisz ich tworzyć, ale docelowo będą przydatne.

## Proponowany `.vscode/extensions.json`

Później możemy dodać do repo plik:

```json
{
  "recommendations": [
    "ms-dotnettools.csdevkit",
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "ms-mssql.mssql",
    "ms-vscode.PowerShell",
    "eamodio.gitlens",
    "EditorConfig.EditorConfig",
    "yzhang.markdown-all-in-one",
    "bierner.markdown-mermaid",
    "redhat.vscode-xml",
    "patcx.vscode-nuget-gallery",
    "humao.rest-client"
  ]
}
```

Dzięki temu VS Code sam zaproponuje instalację rozszerzeń po otwarciu repo.

---

# 15. Zmienna środowiskowa ASPNETCORE_ENVIRONMENT

Do pracy developerskiej będziemy używać środowiska:

```text
Development
```

Ustawienie w bieżącej sesji PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

Sprawdzenie:

```powershell
$env:ASPNETCORE_ENVIRONMENT
```

Oczekiwany wynik:

```text
Development
```

To będzie ważne przy:

- connection string,
- migracjach EF Core,
- trybie symulowanego użytkownika,
- `appsettings.Development.json`.

---

# 16. Lokalny connection string — plan

W aplikacji użyjemy lokalnej bazy:

```text
HospitalAccessControlDb_Dev
```

## SQL Server Developer / default instance

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## SQL Express

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=.\\SQLEXPRESS;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

# 17. Tryb DEV użytkownika

Na stacji developerskiej nie będziemy wymagać prawdziwego Active Directory.

Użytkownik będzie symulowany przez konfigurację:

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

Później będziesz mógł zmieniać użytkownika:

```text
HOSPITAL\doctor.cardio
HOSPITAL\doctor.ortho
HOSPITAL\nurse.cardio
HOSPITAL\registration.user
HOSPITAL\auditor.user
HOSPITAL\it.admin
```

bez przełączania kont Windows.

---

# 18. Lista kontrolna DAY000

```text
[ ] System Windows jest gotowy
[ ] Utworzono katalog C:\Projects\HospitalAccessControl
[ ] PowerShell działa
[ ] Zainstalowano .NET 10 SDK
[ ] dotnet --version działa
[ ] dotnet --list-sdks pokazuje 10.0.xxx
[ ] Zainstalowano Visual Studio Code
[ ] code --version działa
[ ] Zainstalowano rozszerzenie C# Dev Kit
[ ] Zainstalowano rozszerzenie C#
[ ] Zainstalowano rozszerzenie .NET Install Tool
[ ] Zainstalowano rozszerzenie SQL Server mssql
[ ] Zainstalowano rozszerzenie PowerShell
[ ] Zainstalowano rozszerzenie GitLens
[ ] Zainstalowano rozszerzenie EditorConfig
[ ] Zainstalowano rozszerzenie Markdown All in One
[ ] Zainstalowano rozszerzenie Mermaid Preview
[ ] Zainstalowano Git
[ ] git --version działa
[ ] Skonfigurowano git user.name
[ ] Skonfigurowano git user.email
[ ] Zainstalowano SQL Server 2022 Developer albo SQL Express
[ ] Usługa SQL Server działa
[ ] Opcjonalnie działa sqlcmd
[ ] Zainstalowano dotnet-ef
[ ] dotnet ef --version działa
[ ] Ustawiono ASPNETCORE_ENVIRONMENT na Development w sesji roboczej
```

---

# 19. Komendy kontrolne — szybki test stacji

Wklej na końcu DAY000:

```powershell
dotnet --version
dotnet --list-sdks
dotnet --list-runtimes

code --version
code --list-extensions

git --version
git config --global --list

dotnet ef --version

Get-Service | Where-Object DisplayName -like "*SQL Server*"
```

Jeżeli używasz default instance SQL Server:

```powershell
sqlcmd -S localhost -E -Q "SELECT @@SERVERNAME AS ServerName, @@VERSION AS Version;"
```

Jeżeli używasz SQL Express:

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@SERVERNAME AS ServerName, @@VERSION AS Version;"
```

---

# 20. Najczęstsze problemy

## Problem 1 — `dotnet` nie działa

Objaw:

```text
dotnet : The term 'dotnet' is not recognized
```

Rozwiązanie:

- zainstaluj .NET 10 SDK,
- zamknij i otwórz PowerShell,
- sprawdź ponownie:

```powershell
dotnet --version
```

---

## Problem 2 — `code` nie działa

Objaw:

```text
code : The term 'code' is not recognized
```

Rozwiązanie:

W VS Code uruchom:

```text
Ctrl + Shift + P
```

Wpisz:

```text
Shell Command: Install 'code' command in PATH
```

Na Windows zwykle pomaga też restart PowerShell po instalacji VS Code.

---

## Problem 3 — `dotnet ef` nie działa

Objaw:

```text
Could not execute because the specified command or file was not found.
```

Rozwiązanie:

```powershell
dotnet tool install --global dotnet-ef
```

Potem zamknij i otwórz PowerShell.

---

## Problem 4 — SQL Server nie działa

Sprawdź usługi:

```powershell
Get-Service | Where-Object DisplayName -like "*SQL Server*"
```

Uruchom usługę.

Default instance:

```powershell
Start-Service MSSQLSERVER
```

SQL Express:

```powershell
Start-Service MSSQL`$SQLEXPRESS
```

---

## Problem 5 — `sqlcmd` nie działa

Objaw:

```text
sqlcmd : The term 'sqlcmd' is not recognized
```

Rozwiązanie:

- zainstaluj narzędzia SQL Server command-line tools,
- albo używaj rozszerzenia SQL Server w VS Code,
- albo używaj SSMS opcjonalnie.

Brak `sqlcmd` nie blokuje pisania aplikacji, ale utrudnia szybkie testy z PowerShell.

---

# 21. Efekt końcowy DAY000

Po zakończeniu DAY000 stacja developerska jest gotowa do pracy.

Możesz przejść do:

```text
DAY001 — utworzenie solution, projektów, referencji i pierwszego builda
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach etapu przygotowawczego skonfigurowano stację developerską wykorzystywaną do implementacji aplikacji HospitalAccessControl. Zainstalowano środowisko .NET 10 SDK, Visual Studio Code wraz z rozszerzeniami do pracy z C#, SQL Server, PowerShell i Markdown, a także Git, lokalny SQL Server oraz narzędzie `dotnet-ef`. Przygotowano również katalog roboczy projektu oraz założenia pracy w trybie developerskim, w którym użytkownik domenowy będzie symulowany lokalnie bez konieczności posiadania gotowej infrastruktury Active Directory.
