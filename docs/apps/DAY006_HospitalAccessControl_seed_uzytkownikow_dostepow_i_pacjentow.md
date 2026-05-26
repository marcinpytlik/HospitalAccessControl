# DAY006 — HospitalAccessControl  
## Seed użytkowników, dostępów, pacjentów i wpisów medycznych

## Cel dnia

Celem DAY006 jest dodanie danych testowych, które pozwolą później sprawdzić działanie:

```text
RBAC
Row-Level Security
symulowanego użytkownika DEV
listy pacjentów
dostępu według oddziału
```

Po zakończeniu tego dnia powinieneś mieć dane w tabelach:

```text
security.ApplicationUsers
security.UserDepartmentAccess
security.UserRoleAssignments
medical.Patients
medical.MedicalRecords
```

W bazie powinny już istnieć słowniki z DAY005:

```text
dictionary.Departments
dictionary.ApplicationRoles
```

---

## Założenia

Projekt jest już po:

```text
DAY001 — solution i projekty
DAY002 — encje domenowe
DAY003 — DbContext i konfiguracje EF Core
DAY004 — pierwsza migracja i baza DEV
DAY005 — seed danych słownikowych
```

Powinna istnieć lokalna baza:

```text
HospitalAccessControlDb_Dev
```

oraz dane:

```text
5 oddziałów
6 ról aplikacyjnych
```

---

## Co dzisiaj dodajemy?

Dodamy:

```text
12 użytkowników testowych
przypisania użytkowników do ról
przypisania użytkowników do oddziałów
40 pacjentów testowych
wpisy medyczne dla pacjentów
```

Dane będą zgodne z projektem środowiska Active Directory, ale na tym etapie nie potrzebujemy prawdziwego AD.

Użytkownicy będą zapisani jako tekstowe loginy domenowe, np.:

```text
HOSPITAL\doctor.cardio
HOSPITAL\doctor.ortho
HOSPITAL\nurse.cardio
```

---

# 1. Plan danych testowych

## Użytkownicy

| Id | DomainLogin | Rola | Oddział |
|---:|---|---|---|
| 1 | `HOSPITAL\doctor.cardio` | Doctor | Kardiologia |
| 2 | `HOSPITAL\doctor.ortho` | Doctor | Ortopedia |
| 3 | `HOSPITAL\doctor.neuro` | Doctor | Neurologia |
| 4 | `HOSPITAL\nurse.cardio` | Nurse | Kardiologia |
| 5 | `HOSPITAL\nurse.ortho` | Nurse | Ortopedia |
| 6 | `HOSPITAL\nurse.ped` | Nurse | Pediatria |
| 7 | `HOSPITAL\registration.user` | Registration | brak konkretnego oddziału |
| 8 | `HOSPITAL\registration.emer` | Registration | Izba Przyjęć |
| 9 | `HOSPITAL\manager.cardio` | DepartmentManager | Kardiologia |
| 10 | `HOSPITAL\manager.ortho` | DepartmentManager | Ortopedia |
| 11 | `HOSPITAL\auditor.user` | Auditor | brak oddziału |
| 12 | `HOSPITAL\it.admin` | ITAdministrator | brak oddziału |

---

## Oddziały

Z DAY005 mamy:

| DepartmentId | Code | Name |
|---:|---|---|
| 1 | `CARD` | Kardiologia |
| 2 | `ORTH` | Ortopedia |
| 3 | `NEUR` | Neurologia |
| 4 | `EMER` | Izba Przyjęć |
| 5 | `PED` | Pediatria |

---

## Role

Z DAY005 mamy:

| ApplicationRoleId | Code |
|---:|---|
| 1 | `Doctor` |
| 2 | `Nurse` |
| 3 | `Registration` |
| 4 | `DepartmentManager` |
| 5 | `Auditor` |
| 6 | `ITAdministrator` |

---

## Pacjenci

Dodamy:

| Oddział | DepartmentId | Liczba pacjentów |
|---|---:|---:|
| Kardiologia | 1 | 10 |
| Ortopedia | 2 | 10 |
| Neurologia | 3 | 10 |
| Izba Przyjęć | 4 | 5 |
| Pediatria | 5 | 5 |

Razem:

```text
40 pacjentów testowych
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

# Krok 2 — sprawdzenie danych z DAY005

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, Code, Name FROM dictionary.Departments ORDER BY DepartmentId;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationRoleId, Code, Name FROM dictionary.ApplicationRoles ORDER BY ApplicationRoleId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, Code, Name FROM dictionary.Departments ORDER BY DepartmentId;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT ApplicationRoleId, Code, Name FROM dictionary.ApplicationRoles ORDER BY ApplicationRoleId;"
```

Jeżeli te zapytania nie zwracają danych, wróć do DAY005.

---

# Krok 3 — utworzenie plików seed

Tworzymy pliki:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\ApplicationUserSeed.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\UserDepartmentAccessSeed.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\UserRoleAssignmentSeed.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\PatientSeed.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Seed\MedicalRecordSeed.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Seed
```

Powinieneś zobaczyć między innymi:

```text
ApplicationUserSeed.cs
UserDepartmentAccessSeed.cs
UserRoleAssignmentSeed.cs
PatientSeed.cs
MedicalRecordSeed.cs
```

---

# Krok 4 — ApplicationUserSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\ApplicationUserSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class ApplicationUserSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<ApplicationUser> Data => new List<ApplicationUser>
    {
        new()
        {
            ApplicationUserId = 1,
            DomainLogin = @"HOSPITAL\doctor.cardio",
            SamAccountName = "doctor.cardio",
            DisplayName = "Jan Kardiolog",
            Email = "doctor.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 2,
            DomainLogin = @"HOSPITAL\doctor.ortho",
            SamAccountName = "doctor.ortho",
            DisplayName = "Marek Ortopeda",
            Email = "doctor.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 3,
            DomainLogin = @"HOSPITAL\doctor.neuro",
            SamAccountName = "doctor.neuro",
            DisplayName = "Anna Neurolog",
            Email = "doctor.neuro@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 4,
            DomainLogin = @"HOSPITAL\nurse.cardio",
            SamAccountName = "nurse.cardio",
            DisplayName = "Ewa Pielęgniarka Kardiologia",
            Email = "nurse.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 5,
            DomainLogin = @"HOSPITAL\nurse.ortho",
            SamAccountName = "nurse.ortho",
            DisplayName = "Katarzyna Pielęgniarka Ortopedia",
            Email = "nurse.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 6,
            DomainLogin = @"HOSPITAL\nurse.ped",
            SamAccountName = "nurse.ped",
            DisplayName = "Magdalena Pielęgniarka Pediatria",
            Email = "nurse.ped@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 7,
            DomainLogin = @"HOSPITAL\registration.user",
            SamAccountName = "registration.user",
            DisplayName = "Karolina Rejestracja",
            Email = "registration.user@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 8,
            DomainLogin = @"HOSPITAL\registration.emer",
            SamAccountName = "registration.emer",
            DisplayName = "Tomasz Rejestracja Izba Przyjęć",
            Email = "registration.emer@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 9,
            DomainLogin = @"HOSPITAL\manager.cardio",
            SamAccountName = "manager.cardio",
            DisplayName = "Piotr Kierownik Kardiologii",
            Email = "manager.cardio@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 10,
            DomainLogin = @"HOSPITAL\manager.ortho",
            SamAccountName = "manager.ortho",
            DisplayName = "Agnieszka Kierownik Ortopedii",
            Email = "manager.ortho@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 11,
            DomainLogin = @"HOSPITAL\auditor.user",
            SamAccountName = "auditor.user",
            DisplayName = "Alicja Audytor",
            Email = "auditor.user@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        },
        new()
        {
            ApplicationUserId = 12,
            DomainLogin = @"HOSPITAL\it.admin",
            SamAccountName = "it.admin",
            DisplayName = "Adam Administrator IT",
            Email = "it.admin@hospital.local",
            IsActive = true,
            CreatedAt = CreatedAt
        }
    };
}
```

---

# Krok 5 — UserRoleAssignmentSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\UserRoleAssignmentSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class UserRoleAssignmentSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<UserRoleAssignment> Data => new List<UserRoleAssignment>
    {
        new() { UserRoleAssignmentId = 1, ApplicationUserId = 1, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 2, ApplicationUserId = 2, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 3, ApplicationUserId = 3, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 4, ApplicationUserId = 4, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 5, ApplicationUserId = 5, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 6, ApplicationUserId = 6, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 7, ApplicationUserId = 7, ApplicationRoleId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 8, ApplicationUserId = 8, ApplicationRoleId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 9, ApplicationUserId = 9, ApplicationRoleId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 10, ApplicationUserId = 10, ApplicationRoleId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 11, ApplicationUserId = 11, ApplicationRoleId = 5, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 12, ApplicationUserId = 12, ApplicationRoleId = 6, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" }
    };
}
```

---

# Krok 6 — UserDepartmentAccessSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\UserDepartmentAccessSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class UserDepartmentAccessSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<UserDepartmentAccess> Data => new List<UserDepartmentAccess>
    {
        new() { UserDepartmentAccessId = 1, ApplicationUserId = 1, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 2, ApplicationUserId = 2, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 3, ApplicationUserId = 3, DepartmentId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 4, ApplicationUserId = 4, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 5, ApplicationUserId = 5, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 6, ApplicationUserId = 6, DepartmentId = 5, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 7, ApplicationUserId = 8, DepartmentId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 8, ApplicationUserId = 9, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 9, ApplicationUserId = 10, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" }
    };
}
```

## Ważne

Użytkownicy:

```text
registration.user
auditor.user
it.admin
```

celowo nie dostają bezpośredniego dostępu do oddziałów.

To pomoże później pokazać, że:

```text
brak przypisania do oddziału = brak widoczności pacjentów przez RLS
```

---

# Krok 7 — PatientSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\PatientSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class PatientSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<Patient> Data
    {
        get
        {
            var patients = new List<Patient>();
            var id = 1;

            AddPatients(patients, ref id, departmentId: 1, prefix: "CARD", count: 10, lastNamePrefix: "Kardiologiczny");
            AddPatients(patients, ref id, departmentId: 2, prefix: "ORTH", count: 10, lastNamePrefix: "Ortopedyczny");
            AddPatients(patients, ref id, departmentId: 3, prefix: "NEUR", count: 10, lastNamePrefix: "Neurologiczny");
            AddPatients(patients, ref id, departmentId: 4, prefix: "EMER", count: 5, lastNamePrefix: "Nagły");
            AddPatients(patients, ref id, departmentId: 5, prefix: "PED", count: 5, lastNamePrefix: "Pediatryczny");

            return patients;
        }
    }

    private static void AddPatients(
        List<Patient> patients,
        ref int id,
        int departmentId,
        string prefix,
        int count,
        string lastNamePrefix)
    {
        for (var i = 1; i <= count; i++)
        {
            patients.Add(new Patient
            {
                PatientId = id,
                MedicalNumber = $"{prefix}-{i:000}",
                FirstName = $"Pacjent{i:00}",
                LastName = $"{lastNamePrefix}{i:00}",
                Pesel = $"900101{id:00000}",
                DateOfBirth = new DateOnly(1990, 1, 1).AddDays(id),
                GenderCode = id % 2 == 0 ? "F" : "M",
                DepartmentId = departmentId,
                PatientStatusCode = "ACTIVE",
                CreatedAt = CreatedAt,
                CreatedBy = "seed",
                IsDeleted = false
            });

            id++;
        }
    }
}
```

## Uwaga o PESEL

To są dane sztuczne, techniczne, przeznaczone wyłącznie do laboratorium.

Nie używamy prawdziwych danych osobowych.

---

# Krok 8 — MedicalRecordSeed

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Seed\MedicalRecordSeed.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class MedicalRecordSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 2, 8, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<MedicalRecord> Data
    {
        get
        {
            var records = new List<MedicalRecord>();
            var id = 1;

            foreach (var patient in PatientSeed.Data)
            {
                records.Add(new MedicalRecord
                {
                    MedicalRecordId = id,
                    PatientId = patient.PatientId,
                    DepartmentId = patient.DepartmentId,
                    RecordTypeCode = "OBSERVATION",
                    Title = $"Pierwsza obserwacja pacjenta {patient.MedicalNumber}",
                    Description = $"Testowy wpis medyczny dla pacjenta {patient.MedicalNumber}.",
                    Diagnosis = patient.DepartmentId switch
                    {
                        1 => "Kontrola kardiologiczna",
                        2 => "Kontrola ortopedyczna",
                        3 => "Kontrola neurologiczna",
                        4 => "Ocena stanu w izbie przyjęć",
                        5 => "Kontrola pediatryczna",
                        _ => "Obserwacja ogólna"
                    },
                    Treatment = "Zalecenia testowe do demonstracji systemu.",
                    CreatedAt = CreatedAt,
                    CreatedBy = "seed",
                    IsDeleted = false
                });

                id++;
            }

            return records;
        }
    }
}
```

---

# Krok 9 — podłączenie seeda w konfiguracjach EF Core

Teraz trzeba dodać `HasData` do konfiguracji.

---

## ApplicationUserConfiguration

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationUserConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure` dodaj:

```csharp
builder.HasData(ApplicationUserSeed.Data);
```

---

## UserRoleAssignmentConfiguration

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\UserRoleAssignmentConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure` dodaj:

```csharp
builder.HasData(UserRoleAssignmentSeed.Data);
```

---

## UserDepartmentAccessConfiguration

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\UserDepartmentAccessConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure` dodaj:

```csharp
builder.HasData(UserDepartmentAccessSeed.Data);
```

---

## PatientConfiguration

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\PatientConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure` dodaj:

```csharp
builder.HasData(PatientSeed.Data);
```

---

## MedicalRecordConfiguration

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\MedicalRecordConfiguration.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

Na końcu metody `Configure` dodaj:

```csharp
builder.HasData(MedicalRecordSeed.Data);
```

---

# Krok 10 — build po dodaniu seed

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

# Krok 11 — utworzenie migracji SeedUsersAccessAndPatients

Wykonaj:

```powershell
dotnet ef migrations add SeedUsersAccessAndPatients `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output-dir Data\Migrations
```

Sprawdź migrację:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Migrations
```

Powinieneś zobaczyć migrację:

```text
*_SeedUsersAccessAndPatients.cs
```

---

# Krok 12 — sprawdzenie migracji

Otwórz plik migracji:

```text
src\HospitalAccessControl.Infrastructure\Data\Migrations\*_SeedUsersAccessAndPatients.cs
```

Powinieneś zobaczyć `InsertData` dla tabel:

```text
security.ApplicationUsers
security.UserRoleAssignments
security.UserDepartmentAccess
medical.Patients
medical.MedicalRecords
```

Jeżeli migracja jest pusta, sprawdź `HasData`.

---

# Krok 13 — update bazy

Wykonaj:

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

# Krok 14 — weryfikacja danych w SQL Server

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UsersCount FROM security.ApplicationUsers;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UserRoleAssignmentsCount FROM security.UserRoleAssignments;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UserDepartmentAccessCount FROM security.UserDepartmentAccess;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS MedicalRecordsCount FROM medical.MedicalRecords;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UsersCount FROM security.ApplicationUsers;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UserRoleAssignmentsCount FROM security.UserRoleAssignments;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UserDepartmentAccessCount FROM security.UserDepartmentAccess;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS MedicalRecordsCount FROM medical.MedicalRecords;"
```

---

# Krok 15 — oczekiwane wyniki

## Użytkownicy

```text
UsersCount = 12
```

## Role użytkowników

```text
UserRoleAssignmentsCount = 12
```

## Dostępy do oddziałów

```text
UserDepartmentAccessCount = 9
```

## Pacjenci

```text
DepartmentId  PatientsCount
1             10
2             10
3             10
4             5
5             5
```

## Wpisy medyczne

```text
MedicalRecordsCount = 40
```

---

# Krok 16 — zapytanie kontrolne: użytkownik, rola i oddział

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT u.DomainLogin, r.Code AS RoleCode, d.Code AS DepartmentCode FROM security.ApplicationUsers u LEFT JOIN security.UserRoleAssignments ura ON ura.ApplicationUserId = u.ApplicationUserId LEFT JOIN dictionary.ApplicationRoles r ON r.ApplicationRoleId = ura.ApplicationRoleId LEFT JOIN security.UserDepartmentAccess uda ON uda.ApplicationUserId = u.ApplicationUserId LEFT JOIN dictionary.Departments d ON d.DepartmentId = uda.DepartmentId ORDER BY u.ApplicationUserId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT u.DomainLogin, r.Code AS RoleCode, d.Code AS DepartmentCode FROM security.ApplicationUsers u LEFT JOIN security.UserRoleAssignments ura ON ura.ApplicationUserId = u.ApplicationUserId LEFT JOIN dictionary.ApplicationRoles r ON r.ApplicationRoleId = ura.ApplicationRoleId LEFT JOIN security.UserDepartmentAccess uda ON uda.ApplicationUserId = u.ApplicationUserId LEFT JOIN dictionary.Departments d ON d.DepartmentId = uda.DepartmentId ORDER BY u.ApplicationUserId;"
```

Oczekiwany przykład:

```text
HOSPITAL\doctor.cardio       Doctor             CARD
HOSPITAL\doctor.ortho        Doctor             ORTH
HOSPITAL\doctor.neuro        Doctor             NEUR
HOSPITAL\nurse.cardio        Nurse              CARD
HOSPITAL\nurse.ortho         Nurse              ORTH
HOSPITAL\nurse.ped           Nurse              PED
HOSPITAL\registration.user   Registration       NULL
HOSPITAL\registration.emer   Registration       EMER
HOSPITAL\manager.cardio      DepartmentManager  CARD
HOSPITAL\manager.ortho       DepartmentManager  ORTH
HOSPITAL\auditor.user        Auditor            NULL
HOSPITAL\it.admin            ITAdministrator    NULL
```

---

# Krok 17 — wygenerowanie skryptu SQL z migracji

Utwórz katalog, jeśli nie istnieje:

```powershell
New-Item -ItemType Directory -Force .\sql\generated
```

Wygeneruj skrypt:

```powershell
dotnet ef migrations script `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output .\sql\generated\003_SeedUsersAccessAndPatients.sql
```

Sprawdź:

```powershell
Get-ChildItem .\sql\generated
```

Powinieneś mieć:

```text
001_InitialCreate.sql
002_SeedDictionaryData.sql
003_SeedUsersAccessAndPatients.sql
```

---

# Krok 18 — build końcowy

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

# Krok 19 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY006 Seed users access and patients"
```

---

# Kontrola końcowa DAY006

Lista kontrolna:

```text
[ ] Utworzono ApplicationUserSeed.cs
[ ] Utworzono UserRoleAssignmentSeed.cs
[ ] Utworzono UserDepartmentAccessSeed.cs
[ ] Utworzono PatientSeed.cs
[ ] Utworzono MedicalRecordSeed.cs
[ ] Dodano HasData dla ApplicationUser
[ ] Dodano HasData dla UserRoleAssignment
[ ] Dodano HasData dla UserDepartmentAccess
[ ] Dodano HasData dla Patient
[ ] Dodano HasData dla MedicalRecord
[ ] dotnet build kończy się sukcesem
[ ] Utworzono migrację SeedUsersAccessAndPatients
[ ] database update zakończył się sukcesem
[ ] security.ApplicationUsers zawiera 12 rekordów
[ ] security.UserRoleAssignments zawiera 12 rekordów
[ ] security.UserDepartmentAccess zawiera 9 rekordów
[ ] medical.Patients zawiera 40 rekordów
[ ] medical.MedicalRecords zawiera 40 rekordów
[ ] Wygenerowano sql\generated\003_SeedUsersAccessAndPatients.sql
```

---

# Najczęstsze problemy

## Problem 1 — migracja jest pusta

Objaw:

```text
Migration generated with no operations
```

albo plik migracji nie zawiera `InsertData`.

Sprawdź, czy dodałeś `HasData` w konfiguracjach:

```csharp
builder.HasData(ApplicationUserSeed.Data);
builder.HasData(UserRoleAssignmentSeed.Data);
builder.HasData(UserDepartmentAccessSeed.Data);
builder.HasData(PatientSeed.Data);
builder.HasData(MedicalRecordSeed.Data);
```

Sprawdź też, czy dodałeś:

```csharp
using HospitalAccessControl.Infrastructure.Data.Seed;
```

---

## Problem 2 — błąd klucza obcego

Objaw:

```text
The INSERT statement conflicted with the FOREIGN KEY constraint
```

Najczęstsza przyczyna:

- `DepartmentId` nie istnieje,
- `ApplicationRoleId` nie istnieje,
- `ApplicationUserId` nie istnieje,
- migracje są w złej kolejności.

Sprawdź, czy wcześniej wykonałeś DAY005 i czy masz dane:

```sql
SELECT * FROM dictionary.Departments;
SELECT * FROM dictionary.ApplicationRoles;
```

---

## Problem 3 — błąd z `DateOnly`

Jeżeli build zgłasza problem z `DateOnly`, sprawdź framework:

```powershell
dotnet --list-sdks
```

oraz plik `.csproj`:

```xml
<TargetFramework>net10.0</TargetFramework>
```

---

## Problem 4 — PESEL ma złą długość

W encji `Patient` właściwość `Pesel` ma maksymalną długość 11 znaków.

Seed generuje:

```csharp
Pesel = $"900101{id:00000}"
```

To daje 11 znaków:

```text
90010100001
```

Jeżeli zmienisz format, pilnuj długości.

---

## Problem 5 — conflict duplicate key

Jeżeli baza DEV jest w niespójnym stanie, możesz ją wyczyścić.

### Default instance

```powershell
sqlcmd -S localhost -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

### SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "DROP DATABASE IF EXISTS HospitalAccessControlDb_Dev;"
```

Potem odtwórz wszystkie migracje:

```powershell
dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
```

---

# Efekt końcowy DAY006

Po zakończeniu DAY006 masz bazę z kompletem danych startowych:

```text
5 oddziałów
6 ról
12 użytkowników
9 przypisań użytkowników do oddziałów
12 przypisań użytkowników do ról
40 pacjentów
40 wpisów medycznych
```

To jest fundament pod kolejny krok:

```text
DAY007 — CurrentUserService DEV
```

Od DAY007 aplikacja zacznie rozpoznawać symulowanego użytkownika, np.:

```text
HOSPITAL\doctor.cardio
```

a potem w DAY008 i DAY009 podłączymy:

```text
SESSION_CONTEXT
Row-Level Security
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach szóstego etapu implementacji przygotowano komplet danych testowych potrzebnych do weryfikacji modelu RBAC i przyszłego mechanizmu Row-Level Security. Do bazy dodano użytkowników aplikacyjnych odpowiadających planowanym kontom domenowym Active Directory, przypisania użytkowników do ról aplikacyjnych oraz przypisania wybranych użytkowników do oddziałów szpitalnych. Dodano również zestaw 40 pacjentów testowych rozmieszczonych pomiędzy pięć oddziałów oraz podstawowe wpisy medyczne. Dane zostały przygotowane z użyciem mechanizmu `HasData` w Entity Framework Core i zapisane w osobnych klasach seedujących.
