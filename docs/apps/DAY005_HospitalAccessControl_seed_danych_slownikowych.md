# DAY005 — HospitalAccessControl  
## Seed danych słownikowych

## Cel dnia

Celem DAY005 jest dodanie pierwszych danych słownikowych do bazy danych **HospitalAccessControlDb_Dev**.

Po zakończeniu tego dnia powinieneś mieć dane w tabelach:

```text
dictionary.Departments
dictionary.ApplicationRoles
```

Na tym etapie uzupełniamy dwa najważniejsze słowniki:

```text
oddziały szpitalne
role aplikacyjne
```

Dzięki temu w kolejnych dniach będzie można dodać:

```text
użytkowników aplikacyjnych,
przypisania użytkowników do oddziałów,
przypisania użytkowników do ról,
pacjentów testowych.
```

---

## Założenia

Projekt jest już po:

```text
DAY001 — solution i projekty
DAY002 — encje domenowe
DAY003 — DbContext i konfiguracje EF Core
DAY004 — pierwsza migracja i baza DEV
```

Powinna istnieć lokalna baza:

```text
HospitalAccessControlDb_Dev
```

Powinny istnieć tabele:

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

---

## Dlaczego zaczynamy od słowników?

Słowniki są fundamentem danych testowych.

Najpierw potrzebujemy oddziałów:

```text
CARD — Kardiologia
ORTH — Ortopedia
NEUR — Neurologia
EMER — Izba Przyjęć
PED — Pediatria
```

oraz ról:

```text
Doctor
Nurse
Registration
DepartmentManager
Auditor
ITAdministrator
```

Dopiero później możemy dodać użytkowników, np.:

```text
HOSPITAL\doctor.cardio
```

i przypisać go do:

```text
rola: Doctor
oddział: Kardiologia
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

Sprawdź build:

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

# Krok 2 — sprawdzenie aktualnej migracji

Sprawdź listę migracji:

```powershell
dotnet ef migrations list `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

Powinieneś zobaczyć:

```text
InitialCreate
```

Jeżeli migracja `InitialCreate` nie istnieje, wróć do DAY004.

---

# Krok 3 — utworzenie katalogu Seed

Tworzymy katalog na klasy seedujące:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data
```

Powinieneś zobaczyć między innymi:

```text
Configurations
Migrations
Seed
HospitalAccessControlDbContext.cs
```

---

# Krok 4 — utworzenie plików seed

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\DepartmentSeed.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\ApplicationRoleSeed.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Seed
```

Powinieneś zobaczyć:

```text
ApplicationRoleSeed.cs
DepartmentSeed.cs
```

---

# Krok 5 — DepartmentSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\DepartmentSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class DepartmentSeed
{
    public static IReadOnlyList<Department> Data => new List<Department>
    {
        new()
        {
            DepartmentId = 1,
            Code = "CARD",
            Name = "Kardiologia",
            Description = "Oddział kardiologiczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 2,
            Code = "ORTH",
            Name = "Ortopedia",
            Description = "Oddział ortopedyczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 3,
            Code = "NEUR",
            Name = "Neurologia",
            Description = "Oddział neurologiczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 4,
            Code = "EMER",
            Name = "Izba Przyjęć",
            Description = "Izba przyjęć i obsługa nagłych przypadków",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            DepartmentId = 5,
            Code = "PED",
            Name = "Pediatria",
            Description = "Oddział pediatryczny",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    };
}
```

---

## Dlaczego jawne ID?

W danych seedowanych przez EF Core warto jawnie ustawić identyfikatory.

Dzięki temu później możemy jednoznacznie pisać:

```text
DepartmentId = 1 -> Kardiologia
DepartmentId = 2 -> Ortopedia
DepartmentId = 3 -> Neurologia
DepartmentId = 4 -> Izba Przyjęć
DepartmentId = 5 -> Pediatria
```

To bardzo ułatwi seed pacjentów i przypisań użytkowników.

---

# Krok 6 — ApplicationRoleSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\ApplicationRoleSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class ApplicationRoleSeed
{
    public static IReadOnlyList<ApplicationRole> Data => new List<ApplicationRole>
    {
        new()
        {
            ApplicationRoleId = 1,
            Code = "Doctor",
            Name = "Lekarz",
            Description = "Użytkownik medyczny odpowiedzialny za diagnozę i leczenie pacjentów",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 2,
            Code = "Nurse",
            Name = "Pielęgniarka",
            Description = "Użytkownik medyczny z ograniczonym dostępem do dokumentacji pacjenta",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 3,
            Code = "Registration",
            Name = "Rejestracja",
            Description = "Użytkownik odpowiedzialny za obsługę rejestracji pacjentów",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 4,
            Code = "DepartmentManager",
            Name = "Kierownik oddziału",
            Description = "Użytkownik zarządzający danym oddziałem",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 5,
            Code = "Auditor",
            Name = "Audytor",
            Description = "Użytkownik odpowiedzialny za kontrolę i analizę zdarzeń audytowych",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            ApplicationRoleId = 6,
            Code = "ITAdministrator",
            Name = "Administrator IT",
            Description = "Administrator techniczny bez domyślnego dostępu biznesowego do danych medycznych",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    };
}
```

---

# Krok 7 — podłączenie seeda w konfiguracjach EF Core

Seed przez `HasData` najlepiej podłączyć w konfiguracjach encji.

---

## DepartmentConfiguration

Otwórz plik:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\DepartmentConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure`, przed zamykającą klamrą, dodaj:

```csharp
builder.HasData(DepartmentSeed.Data);
```

Czyli końcówka klasy powinna zawierać coś takiego:

```csharp
builder.HasIndex(x => x.Name)
    .HasDatabaseName("IX_Departments_Name");

builder.HasData(DepartmentSeed.Data);
```

---

## ApplicationRoleConfiguration

Otwórz plik:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationRoleConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure`, przed zamykającą klamrą, dodaj:

```csharp
builder.HasData(ApplicationRoleSeed.Data);
```

Czyli końcówka klasy powinna wyglądać tak:

```csharp
builder.HasIndex(x => x.Code)
    .IsUnique()
    .HasDatabaseName("UQ_ApplicationRoles_Code");

builder.HasData(ApplicationRoleSeed.Data);
```

---

# Krok 8 — build po dodaniu seed

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

Jeżeli pojawią się błędy, sprawdź:

```text
namespace
using
literówki w nazwach klas
średniki
klamry
```

---

# Krok 9 — utworzenie migracji SeedDictionaryData

Utwórz migrację:

```powershell
dotnet ef migrations add SeedDictionaryData `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output-dir Data\Migrations
```

## Oczekiwany efekt

Powinna powstać nowa migracja:

```text
*_SeedDictionaryData.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Migrations
```

---

# Krok 10 — sprawdzenie migracji

Otwórz plik migracji:

```text
src\HospitalAccessControl.Infrastructure\Data\Migrations\*_SeedDictionaryData.cs
```

Powinieneś zobaczyć instrukcje podobne do:

```csharp
migrationBuilder.InsertData(
    schema: "dictionary",
    table: "Departments",
    columns: ...
);
```

oraz:

```csharp
migrationBuilder.InsertData(
    schema: "dictionary",
    table: "ApplicationRoles",
    columns: ...
);
```

Jeżeli migracja jest pusta, sprawdź, czy dodałeś:

```csharp
builder.HasData(DepartmentSeed.Data);
builder.HasData(ApplicationRoleSeed.Data);
```

w odpowiednich konfiguracjach.

---

# Krok 11 — update bazy

Wykonaj:

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

## Co robi ta komenda?

Wprowadza dane słownikowe do istniejącej bazy:

```text
HospitalAccessControlDb_Dev
```

Dodaje wpis migracji do:

```text
dbo.__EFMigrationsHistory
```

---

# Krok 12 — weryfikacja danych w SQL Server

## Wariant A — default instance

Sprawdź oddziały:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, Code, Name, IsActive FROM dictionary.Departments ORDER BY DepartmentId;"
```

Sprawdź role:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationRoleId, Code, Name, IsActive FROM dictionary.ApplicationRoles ORDER BY ApplicationRoleId;"
```

## Wariant B — SQL Express

Sprawdź oddziały:

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, Code, Name, IsActive FROM dictionary.Departments ORDER BY DepartmentId;"
```

Sprawdź role:

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationRoleId, Code, Name, IsActive FROM dictionary.ApplicationRoles ORDER BY ApplicationRoleId;"
```

---

# Krok 13 — oczekiwane wyniki

## Oddziały

Powinieneś zobaczyć:

```text
1  CARD  Kardiologia
2  ORTH  Ortopedia
3  NEUR  Neurologia
4  EMER  Izba Przyjęć
5  PED   Pediatria
```

## Role

Powinieneś zobaczyć:

```text
1  Doctor             Lekarz
2  Nurse              Pielęgniarka
3  Registration       Rejestracja
4  DepartmentManager  Kierownik oddziału
5  Auditor            Audytor
6  ITAdministrator    Administrator IT
```

---

# Krok 14 — weryfikacja historii migracji

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;"
```

Powinieneś zobaczyć minimum dwie migracje:

```text
InitialCreate
SeedDictionaryData
```

---

# Krok 15 — wygenerowanie skryptu SQL dla migracji

Dobrą praktyką jest generowanie skryptów SQL do dokumentacji.

Utwórz katalog, jeśli jeszcze nie istnieje:

```powershell
New-Item -ItemType Directory -Force .\sql\generated
```

Wygeneruj skrypt od początku do najnowszej migracji:

```powershell
dotnet ef migrations script `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output .\sql\generated\002_SeedDictionaryData.sql
```

Sprawdź:

```powershell
Get-ChildItem .\sql\generated
```

Powinieneś mieć:

```text
001_InitialCreate.sql
002_SeedDictionaryData.sql
```

Uwaga:

Ten skrypt może zawierać pełne przejście od zera do aktualnej wersji.  
Jeżeli później będziemy potrzebowali skrypt tylko pomiędzy migracjami, użyjemy wariantu z `from` i `to`.

---

# Krok 16 — opcjonalne testowe zapytanie kontrolne

Możesz sprawdzić liczbę rekordów:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT 'Departments' AS TableName, COUNT(*) AS RowCount FROM dictionary.Departments UNION ALL SELECT 'ApplicationRoles', COUNT(*) FROM dictionary.ApplicationRoles;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT 'Departments' AS TableName, COUNT(*) AS RowCount FROM dictionary.Departments UNION ALL SELECT 'ApplicationRoles', COUNT(*) FROM dictionary.ApplicationRoles;"
```

Oczekiwany wynik:

```text
Departments        5
ApplicationRoles   6
```

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
git commit -m "DAY005 Seed dictionary data"
```

---

# Kontrola końcowa DAY005

Lista kontrolna:

```text
[ ] Utworzono katalog Data\Seed
[ ] Utworzono DepartmentSeed.cs
[ ] Utworzono ApplicationRoleSeed.cs
[ ] Dodano builder.HasData dla Department
[ ] Dodano builder.HasData dla ApplicationRole
[ ] dotnet build kończy się sukcesem
[ ] Utworzono migrację SeedDictionaryData
[ ] database update zakończył się sukcesem
[ ] dictionary.Departments zawiera 5 rekordów
[ ] dictionary.ApplicationRoles zawiera 6 rekordów
[ ] __EFMigrationsHistory zawiera SeedDictionaryData
[ ] Wygenerowano sql\generated\002_SeedDictionaryData.sql
```

---

# Najczęstsze problemy

## Problem 1 — migracja jest pusta

Objaw:

```text
Migration generated with no operations
```

albo w pliku migracji nie ma `InsertData`.

Sprawdź, czy dodałeś:

```csharp
builder.HasData(DepartmentSeed.Data);
builder.HasData(ApplicationRoleSeed.Data);
```

w plikach:

```text
DepartmentConfiguration.cs
ApplicationRoleConfiguration.cs
```

Sprawdź też, czy dodałeś:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

---

## Problem 2 — błąd z DateTimeKind

EF Core przy `HasData` wymaga stałych, przewidywalnych wartości.

Dlatego używamy:

```csharp
new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
```

Nie używaj w seedzie:

```csharp
DateTime.UtcNow
DateTime.Now
```

bo EF Core będzie generował zmiany w migracjach przy kolejnych uruchomieniach.

---

## Problem 3 — konflikt kluczy głównych

Objaw:

```text
Cannot insert duplicate key
```

Najczęstsza przyczyna:

- dane zostały już ręcznie dodane do tabel,
- migracja była uruchamiana kilka razy po ręcznych zmianach,
- baza DEV jest w niespójnym stanie.

Jeżeli to tylko środowisko DEV, najprościej wyczyścić bazę i odtworzyć migracje:

### Default instance

```powershell
sqlcmd -S localhost -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

### SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

Potem:

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

---

## Problem 4 — błąd connection string

Objaw:

```text
Connection string 'HospitalAccessControlDb' was not found.
```

Sprawdź plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
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

## Problem 5 — SQL Server nie działa

Sprawdź usługę.

### Default instance

```powershell
Get-Service MSSQLSERVER
```

### SQL Express

```powershell
Get-Service MSSQL`$SQLEXPRESS
```

Jeżeli usługa nie działa:

```powershell
Start-Service MSSQLSERVER
```

albo dla SQL Express:

```powershell
Start-Service MSSQL`$SQLEXPRESS
```

---

# Efekt końcowy DAY005

Po zakończeniu DAY005 masz w bazie pierwsze dane słownikowe:

```text
dictionary.Departments
dictionary.ApplicationRoles
```

Czyli fundament pod dalsze dane:

```text
użytkownicy aplikacyjni,
przypisania użytkowników do oddziałów,
przypisania użytkowników do ról,
pacjenci testowi,
dokumentacja medyczna.
```

Następny etap:

```text
DAY006 — Seed użytkowników, dostępów i pacjentów
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach piątego etapu implementacji przygotowano dane słownikowe wymagane do dalszego działania systemu. Do bazy `HospitalAccessControlDb_Dev` dodano podstawowe oddziały szpitalne oraz role aplikacyjne. Dane zostały zaimplementowane z wykorzystaniem mechanizmu `HasData` w Entity Framework Core i zapisane w osobnych klasach seedujących. Następnie utworzono migrację `SeedDictionaryData`, zastosowano ją do lokalnej bazy danych oraz zweryfikowano poprawność danych bezpośrednio w SQL Server.
