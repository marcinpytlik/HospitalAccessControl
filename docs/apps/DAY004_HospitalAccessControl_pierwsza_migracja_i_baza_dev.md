# DAY004 — HospitalAccessControl  
## Pierwsza migracja EF Core i utworzenie lokalnej bazy DEV

## Cel dnia

Celem DAY004 jest utworzenie pierwszej migracji Entity Framework Core oraz założenie lokalnej bazy danych:

```text
HospitalAccessControlDb_Dev
```

Po zakończeniu tego dnia powinieneś mieć:

- skonfigurowany connection string,
- działające narzędzie `dotnet ef`,
- pierwszą migrację `InitialCreate`,
- utworzoną lokalną bazę SQL Server,
- schematy SQL:
  - `dictionary`,
  - `security`,
  - `medical`,
  - `audit`,
- tabele wynikające z encji domenowych,
- tabelę `__EFMigrationsHistory`,
- działający `dotnet build`.

---

## Założenia

Projekt jest już po:

```text
DAY001 — solution i projekty
DAY002 — encje domenowe
DAY003 — DbContext i konfiguracje EF Core
```

Na DAY004 pracujemy głównie z:

```text
src\HospitalAccessControl.Infrastructure
src\HospitalAccessControl.Web
```

Do wykonania DAY004 potrzebujesz lokalnego SQL Server.

Może to być:

```text
SQL Server 2022 Developer Edition
SQL Server Express
LocalDB — opcjonalnie, ale dla tego projektu lepiej SQL Server Developer albo Express
```

---

## Ważna decyzja projektowa

Na tym etapie tworzymy bazę przez **EF Core Migrations**.

Mechanizmy bezpieczeństwa, takie jak:

```text
Row-Level Security
SQL Server Audit
Dynamic Data Masking
loginy AD
role bazodanowe
GRANT/DENY
```

będą później tworzone osobnymi skryptami T-SQL.

Czyli przyjmujemy model:

```text
EF Core -> tabele, relacje, indeksy
T-SQL   -> security, RLS, audit, DDM, role SQL
```

To jest bardzo dobre podejście dla tej pracy, bo łączy aplikację .NET z technicznymi mechanizmami SQL Server.

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

Sprawdź build po DAY003:

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

# Krok 2 — sprawdzenie lokalnego SQL Server

## Wariant A — SQL Server Developer / default instance

Sprawdź usługę SQL Server:

```powershell
Get-Service MSSQLSERVER
```

Jeżeli usługa działa, zobaczysz coś podobnego:

```text
Status   Name        DisplayName
------   ----        -----------
Running  MSSQLSERVER SQL Server (MSSQLSERVER)
```

## Wariant B — SQL Server Express

Sprawdź usługę:

```powershell
Get-Service MSSQL`$SQLEXPRESS
```

Albo:

```powershell
Get-Service | Where-Object DisplayName -like "*SQL Server*"
```

## Wariant C — szybki test połączenia przez sqlcmd

Jeżeli masz `sqlcmd`, sprawdź:

### Default instance

```powershell
sqlcmd -S localhost -E -Q "SELECT @@VERSION AS SqlVersion;"
```

### SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@VERSION AS SqlVersion;"
```

Jeżeli `sqlcmd` nie jest zainstalowany, na razie nie blokuje to pracy.  
Możesz sprawdzić bazę później przez SSMS albo rozszerzenie SQL Server w VS Code.

---

# Krok 3 — sprawdzenie connection string

Otwórz plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

## Wariant dla lokalnego SQL Server Developer / default instance

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Wariant dla SQL Express

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=.\\SQLEXPRESS;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Wariant dla LocalDB

Jeżeli używasz LocalDB:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=(localdb)\\MSSQLLocalDB;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Dla tego projektu rekomenduję:

```text
SQL Server 2022 Developer Edition
```

bo później będziemy testować mechanizmy typowo SQL Serverowe.

---

# Krok 4 — sprawdzenie środowiska ASP.NET Core

Migracje EF Core korzystają ze startup project, czyli z projektu Web.

Upewnij się, że środowisko jest ustawione na Development:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

Sprawdź:

```powershell
$env:ASPNETCORE_ENVIRONMENT
```

Oczekiwany wynik:

```text
Development
```

---

# Krok 5 — sprawdzenie narzędzia dotnet-ef

Sprawdź, czy masz `dotnet ef`:

```powershell
dotnet ef --version
```

Jeżeli zobaczysz wersję, jest dobrze.

Jeżeli dostaniesz komunikat, że polecenie nie istnieje, zainstaluj narzędzie:

```powershell
dotnet tool install --global dotnet-ef
```

Jeżeli masz starszą wersję, zaktualizuj:

```powershell
dotnet tool update --global dotnet-ef
```

Po instalacji zamknij i otwórz PowerShell albo sprawdź jeszcze raz:

```powershell
dotnet ef --version
```

---

# Krok 6 — sprawdzenie paczek EF Core

Sprawdź paczki w projekcie Infrastructure:

```powershell
dotnet list .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj package
```

Powinieneś mieć między innymi:

```text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
```

Sprawdź paczki w projekcie Web:

```powershell
dotnet list .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj package
```

Powinieneś mieć:

```text
Microsoft.EntityFrameworkCore.Design
```

Jeżeli czegoś brakuje, dodaj:

```powershell
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.Design

dotnet add .\src\HospitalAccessControl.Web package Microsoft.EntityFrameworkCore.Design
```

---

# Krok 7 — pierwsza migracja InitialCreate

Wykonaj:

```powershell
dotnet ef migrations add InitialCreate `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output-dir Data\Migrations
```

## Co robi ta komenda?

Tworzy migrację na podstawie:

```text
encji domenowych
DbContext
konfiguracji EF Core
```

Migracja zostanie zapisana w:

```text
src\HospitalAccessControl.Infrastructure\Data\Migrations
```

## Oczekiwany efekt

Powinny powstać pliki podobne do:

```text
2026xxxxxxxxxx_InitialCreate.cs
2026xxxxxxxxxx_InitialCreate.Designer.cs
HospitalAccessControlDbContextModelSnapshot.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Migrations
```

---

# Krok 8 — kontrola migracji przed update database

To ważny krok.

Otwórz plik migracji:

```text
src\HospitalAccessControl.Infrastructure\Data\Migrations\*_InitialCreate.cs
```

Sprawdź, czy migracja zawiera tworzenie schematów:

```csharp
migrationBuilder.EnsureSchema(
    name: "dictionary");

migrationBuilder.EnsureSchema(
    name: "security");

migrationBuilder.EnsureSchema(
    name: "medical");

migrationBuilder.EnsureSchema(
    name: "audit");
```

Sprawdź, czy tabele są tworzone w dobrych schematach:

```text
dictionary.Departments
dictionary.ApplicationRoles
security.ApplicationUsers
security.UserDepartmentAccess
security.UserRoleAssignments
medical.Patients
medical.MedicalRecords
audit.AccessLog
```

Jeżeli tak — idziemy dalej.

---

# Krok 9 — utworzenie bazy przez database update

Wykonaj:

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

## Co robi ta komenda?

Ta komenda:

```text
łączy się z SQL Server,
tworzy bazę HospitalAccessControlDb_Dev, jeśli nie istnieje,
tworzy schematy,
tworzy tabele,
tworzy indeksy,
wpisuje migrację do __EFMigrationsHistory.
```

---

# Krok 10 — weryfikacja bazy w SQL Server

## Weryfikacja przez sqlcmd — default instance

```powershell
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases WHERE name = N'HospitalAccessControlDb_Dev';"
```

## Weryfikacja przez sqlcmd — SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT name FROM sys.databases WHERE name = N'HospitalAccessControlDb_Dev';"
```

Oczekiwany wynik:

```text
HospitalAccessControlDb_Dev
```

---

# Krok 11 — weryfikacja schematów

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT name FROM sys.schemas WHERE name IN ('dictionary','security','medical','audit') ORDER BY name;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT name FROM sys.schemas WHERE name IN ('dictionary','security','medical','audit') ORDER BY name;"
```

Oczekiwany wynik:

```text
audit
dictionary
medical
security
```

---

# Krok 12 — weryfikacja tabel

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT s.name AS SchemaName, t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id ORDER BY s.name, t.name;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT s.name AS SchemaName, t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id ORDER BY s.name, t.name;"
```

Powinieneś zobaczyć między innymi:

```text
audit        AccessLog
dictionary   ApplicationRoles
dictionary   Departments
medical      MedicalRecords
medical      Patients
security     ApplicationUsers
security     UserDepartmentAccess
security     UserRoleAssignments
dbo          __EFMigrationsHistory
```

---

# Krok 13 — weryfikacja migracji EF

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory;"
```

Powinieneś zobaczyć migrację:

```text
InitialCreate
```

---

# Krok 14 — build po migracji

Wróć do katalogu projektu i wykonaj:

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

# Krok 15 — uruchomienie aplikacji

Uruchom aplikację:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz adres z terminala, np.:

```text
https://localhost:xxxx
```

Na razie aplikacja nadal pokazuje domyślną stronę Razor Pages.  
To jest poprawne.

Zatrzymaj aplikację:

```text
CTRL + C
```

---

# Krok 16 — wygenerowanie skryptu SQL z migracji

To jest bardzo dobry materiał do repo i dokumentacji pracy.

Utwórz katalog:

```powershell
New-Item -ItemType Directory -Force .\sql\generated
```

Wygeneruj skrypt:

```powershell
dotnet ef migrations script `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output .\sql\generated\001_InitialCreate.sql
```

Sprawdź plik:

```powershell
Get-ChildItem .\sql\generated
```

Powinieneś mieć:

```text
001_InitialCreate.sql
```

Ten plik możesz później dołączyć jako artefakt techniczny pracy.

---

# Krok 17 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY004 Add initial EF Core migration and create dev database"
```

---

# Kontrola końcowa DAY004

Lista kontrolna:

```text
[ ] dotnet build działa przed migracją
[ ] appsettings.Development.json ma poprawny connection string
[ ] ASPNETCORE_ENVIRONMENT ustawione na Development
[ ] dotnet ef --version działa
[ ] paczki EF Core są dodane
[ ] utworzono migrację InitialCreate
[ ] powstał katalog Data\Migrations
[ ] migracja zawiera schematy dictionary, security, medical, audit
[ ] database update zakończył się sukcesem
[ ] baza HospitalAccessControlDb_Dev istnieje
[ ] tabele zostały utworzone
[ ] istnieje tabela __EFMigrationsHistory
[ ] wygenerowano sql\generated\001_InitialCreate.sql
[ ] dotnet build kończy się sukcesem
[ ] aplikacja webowa uruchamia się
```

---

# Najczęstsze problemy

## Problem 1 — `dotnet ef` nie działa

Objaw:

```text
Could not execute because the specified command or file was not found.
```

Rozwiązanie:

```powershell
dotnet tool install --global dotnet-ef
```

Albo aktualizacja:

```powershell
dotnet tool update --global dotnet-ef
```

Potem zamknij i otwórz PowerShell.

---

## Problem 2 — brak DbContext

Objaw:

```text
No DbContext was found in assembly
```

Sprawdź, czy komenda ma poprawny kontekst:

```powershell
--context HospitalAccessControlDbContext
```

Sprawdź też, czy `HospitalAccessControlDbContext` jest publiczny:

```csharp
public sealed class HospitalAccessControlDbContext : DbContext
```

---

## Problem 3 — brak connection string

Objaw:

```text
Connection string 'HospitalAccessControlDb' was not found.
```

Rozwiązanie:

Sprawdź plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Powinien mieć:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Sprawdź też środowisko:

```powershell
$env:ASPNETCORE_ENVIRONMENT
```

Ustaw:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

---

## Problem 4 — nie można połączyć się z SQL Server

Objaw:

```text
A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

Sprawdź, czy działa SQL Server:

```powershell
Get-Service MSSQLSERVER
```

Dla SQL Express:

```powershell
Get-Service MSSQL`$SQLEXPRESS
```

Sprawdź connection string:

```text
Server=localhost
```

albo:

```text
Server=.\SQLEXPRESS
```

---

## Problem 5 — błąd certyfikatu SQL Server

Objaw:

```text
The certificate chain was issued by an authority that is not trusted.
```

Rozwiązanie:

W connection string dodaj:

```text
TrustServerCertificate=True;
```

Czyli:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## Problem 6 — migracja nie tworzy schematów

Jeżeli migracja tworzy tabele w `dbo`, a nie w schematach:

```text
dictionary
security
medical
audit
```

to sprawdź konfiguracje:

```csharp
builder.ToTable("Departments", "dictionary");
builder.ToTable("ApplicationUsers", "security");
builder.ToTable("Patients", "medical");
builder.ToTable("AccessLog", "audit");
```

Potem usuń błędną migrację:

```powershell
dotnet ef migrations remove `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

I utwórz ją ponownie.

---

## Problem 7 — baza już istnieje i chcesz zacząć od nowa

Jeżeli to tylko środowisko DEV, możesz usunąć bazę.

### Default instance

```powershell
sqlcmd -S localhost -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

### SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

Potem wykonaj ponownie:

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

---

# Efekt końcowy DAY004

Po zakończeniu DAY004 masz:

```text
lokalną bazę HospitalAccessControlDb_Dev
pierwszą migrację InitialCreate
schematy dictionary/security/medical/audit
tabele pacjentów, użytkowników, ról, dostępów i audytu
wygenerowany skrypt SQL z migracji
```

To jest pierwszy moment, w którym projekt ma realną bazę danych.

Następny etap:

```text
DAY005 — Seed danych słownikowych
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach czwartego etapu implementacji utworzono pierwszą migrację Entity Framework Core oraz wygenerowano lokalną bazę danych `HospitalAccessControlDb_Dev`. Migracja utworzyła schematy logiczne `dictionary`, `security`, `medical` oraz `audit`, a także podstawowe tabele wynikające z modelu domenowego aplikacji. Wygenerowano również skrypt SQL migracji, który może zostać wykorzystany jako artefakt techniczny w dokumentacji pracy inżynierskiej.
