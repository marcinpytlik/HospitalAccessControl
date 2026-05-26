# DAY002 — HospitalAccessControl  
## Encje domenowe i minimalny model danych aplikacji

## Cel dnia

Celem DAY002 jest przygotowanie pierwszej wersji warstwy domenowej aplikacji **HospitalAccessControl**.

Po zakończeniu tego dnia powinieneś mieć:

- katalog `Entities`,
- katalog `Common`,
- podstawowe klasy domenowe,
- minimalny model potrzebny do bazy danych,
- klasy gotowe do późniejszego mapowania przez EF Core,
- działający `dotnet build`.

Na tym etapie nadal **nie tworzymy jeszcze bazy danych** i **nie konfigurujemy EF Core**.  
Robimy tylko czysty model domenowy.

---

## Założenia

Projekt jest już utworzony po DAY001.

Zakładamy, że istnieje katalog:

```text
C:\Projects\HospitalAccessControl
```

oraz projekty:

```text
src\HospitalAccessControl.Domain
src\HospitalAccessControl.Application
src\HospitalAccessControl.Infrastructure
src\HospitalAccessControl.Web
tests\HospitalAccessControl.Tests
tests\HospitalAccessControl.IntegrationTests
```

Na DAY002 pracujemy głównie w projekcie:

```text
src\HospitalAccessControl.Domain
```

---

## Co dzisiaj tworzymy?

Minimalny zestaw encji:

```text
Department
ApplicationRole
ApplicationUser
UserDepartmentAccess
UserRoleAssignment
Patient
MedicalRecord
AccessLog
```

To jest pierwszy działający model, który później pozwoli zbudować scenariusz:

```text
użytkownik domenowy
    ↓
rola
    ↓
oddział
    ↓
pacjent
    ↓
Row-Level Security
```

---

## Dlaczego nie tworzymy od razu wszystkiego?

Docelowo baza będzie miała więcej tabel, na przykład:

```text
PatientAdmission
PatientNote
SecurityEvent
ApplicationSetting
SchemaVersion
PatientStatus
MedicalRecordType
Gender
AccessActionType
SecurityEventType
```

Ale na początek nie komplikujemy.

Pierwszy cel aplikacji to:

```text
pokazać listę pacjentów ograniczoną przez oddział użytkownika
```

Do tego wystarczą:

```text
Department
Patient
ApplicationUser
UserDepartmentAccess
ApplicationRole
UserRoleAssignment
MedicalRecord
AccessLog
```

---

# Krok 1 — przejście do katalogu projektu

Otwórz PowerShell i przejdź do katalogu projektu:

```powershell
Set-Location C:\Projects\HospitalAccessControl
```

Sprawdź, czy jesteś w dobrym miejscu:

```powershell
Get-Location
```

Oczekiwany wynik:

```text
C:\Projects\HospitalAccessControl
```

Sprawdź solution:

```powershell
Get-ChildItem *.slnx
```

Powinieneś zobaczyć:

```text
HospitalAccessControl.slnx
```

---

# Krok 2 — utworzenie katalogów w projekcie Domain

Tworzymy katalogi:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Domain\Entities
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Domain\Common
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Domain
```

Powinieneś zobaczyć między innymi:

```text
Common
Entities
HospitalAccessControl.Domain.csproj
```

---

# Krok 3 — usunięcie domyślnej klasy Class1.cs

Po utworzeniu projektu `classlib` zwykle istnieje plik:

```text
Class1.cs
```

Usuwamy go, bo nie będzie potrzebny:

```powershell
Remove-Item .\src\HospitalAccessControl.Domain\Class1.cs -ErrorAction SilentlyContinue
```

---

# Krok 4 — utworzenie plików encji

Tworzymy puste pliki klas:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\Department.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\ApplicationRole.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\ApplicationUser.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\UserDepartmentAccess.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\UserRoleAssignment.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\Patient.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\MedicalRecord.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Entities\AccessLog.cs
```

Sprawdź pliki:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Domain\Entities
```

---

# Krok 5 — utworzenie klasy bazowej audytu

Tworzymy plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Domain\Common\AuditableEntity.cs
```

## Zawartość pliku `AuditableEntity.cs`

Wklej do pliku:

```csharp
namespace HospitalAccessControl.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
```

## Dlaczego ta klasa jest potrzebna?

Większość tabel biznesowych będzie miała pola:

```text
CreatedAt
CreatedBy
UpdatedAt
UpdatedBy
```

Dzięki klasie bazowej nie powtarzamy tych właściwości w każdej encji.

---

# Krok 6 — encja Department

## Plik

```text
src\HospitalAccessControl.Domain\Entities\Department.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class Department
{
    public int DepartmentId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public ICollection<UserDepartmentAccess> UserDepartmentAccesses { get; set; } = new List<UserDepartmentAccess>();
}
```

## Znaczenie encji

`Department` reprezentuje oddział szpitalny, np.:

```text
CARD — Kardiologia
ORTH — Ortopedia
NEUR — Neurologia
EMER — Izba Przyjęć
PED — Pediatria
```

Kolumna `DepartmentId` będzie później kluczowa dla Row-Level Security.

---

# Krok 7 — encja ApplicationRole

## Plik

```text
src\HospitalAccessControl.Domain\Entities\ApplicationRole.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class ApplicationRole
{
    public int ApplicationRoleId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
}
```

## Przykładowe role

```text
Doctor
Nurse
Registration
DepartmentManager
Auditor
ITAdministrator
```

---

# Krok 8 — encja ApplicationUser

## Plik

```text
src\HospitalAccessControl.Domain\Entities\ApplicationUser.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class ApplicationUser
{
    public int ApplicationUserId { get; set; }

    public string DomainLogin { get; set; } = string.Empty;

    public string SamAccountName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<UserDepartmentAccess> DepartmentAccesses { get; set; } = new List<UserDepartmentAccess>();

    public ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
}
```

## Ważne

Ta tabela **nie służy do logowania**.

Logowanie będzie realizowane przez:

```text
Active Directory / Windows Authentication
```

Encja `ApplicationUser` służy do mapowania użytkownika domenowego na role i oddziały.

Przykład:

```text
HOSPITAL\doctor.cardio
```

---

# Krok 9 — encja UserDepartmentAccess

## Plik

```text
src\HospitalAccessControl.Domain\Entities\UserDepartmentAccess.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class UserDepartmentAccess
{
    public int UserDepartmentAccessId { get; set; }

    public int ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
```

## Znaczenie dla RLS

To jest jedna z najważniejszych encji projektu.

Przykład:

```text
doctor.cardio -> Kardiologia
doctor.ortho  -> Ortopedia
nurse.cardio  -> Kardiologia
```

RLS będzie sprawdzał, czy aktualny użytkownik ma aktywny wpis w tej tabeli dla danego oddziału.

---

# Krok 10 — encja UserRoleAssignment

## Plik

```text
src\HospitalAccessControl.Domain\Entities\UserRoleAssignment.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class UserRoleAssignment
{
    public int UserRoleAssignmentId { get; set; }

    public int ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int ApplicationRoleId { get; set; }

    public ApplicationRole ApplicationRole { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
```

## Przykład

```text
doctor.cardio -> Doctor
nurse.cardio  -> Nurse
auditor.user  -> Auditor
it.admin      -> ITAdministrator
```

---

# Krok 11 — encja Patient

## Plik

```text
src\HospitalAccessControl.Domain\Entities\Patient.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Common;

namespace HospitalAccessControl.Domain.Entities;

public sealed class Patient : AuditableEntity
{
    public int PatientId { get; set; }

    public string MedicalNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Pesel { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string GenderCode { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public string PatientStatusCode { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
```

## Dlaczego `GenderCode` i `PatientStatusCode` jako string?

Na pierwszym etapie upraszczamy model.

Zamiast od razu tworzyć osobne tabele:

```text
Genders
PatientStatuses
```

używamy kodów:

```text
M
F
ACTIVE
DISCHARGED
```

Później możemy to rozbudować do pełnych tabel słownikowych.

## Znaczenie dla RLS

Kolumna:

```text
DepartmentId
```

będzie filtrowana przez Row-Level Security.

---

# Krok 12 — encja MedicalRecord

## Plik

```text
src\HospitalAccessControl.Domain\Entities\MedicalRecord.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Common;

namespace HospitalAccessControl.Domain.Entities;

public sealed class MedicalRecord : AuditableEntity
{
    public int MedicalRecordId { get; set; }

    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public string RecordTypeCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public bool IsDeleted { get; set; }
}
```

## Dlaczego `DepartmentId` jest też tutaj?

Teoretycznie można odczytać oddział przez pacjenta.

Ale dla RLS lepiej mieć `DepartmentId` bezpośrednio w tabeli `MedicalRecords`.

Dzięki temu polityka bezpieczeństwa jest prostsza i czytelniejsza.

---

# Krok 13 — encja AccessLog

## Plik

```text
src\HospitalAccessControl.Domain\Entities\AccessLog.cs
```

## Zawartość

```csharp
namespace HospitalAccessControl.Domain.Entities;

public sealed class AccessLog
{
    public long AccessLogId { get; set; }

    public string DomainLogin { get; set; } = string.Empty;

    public int? PatientId { get; set; }

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

## Przykładowe akcje

```text
ViewPatientList
ViewPatientDetails
ViewMedicalRecord
CreateMedicalRecord
AccessDenied
```

---

# Krok 14 — build projektu

Po wklejeniu wszystkich klas wykonaj:

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

# Krok 15 — opcjonalnie: sprawdzenie plików

```powershell
Get-ChildItem .\src\HospitalAccessControl.Domain\Entities
Get-ChildItem .\src\HospitalAccessControl.Domain\Common
```

Powinieneś mieć:

```text
Entities
├── AccessLog.cs
├── ApplicationRole.cs
├── ApplicationUser.cs
├── Department.cs
├── MedicalRecord.cs
├── Patient.cs
├── UserDepartmentAccess.cs
└── UserRoleAssignment.cs

Common
└── AuditableEntity.cs
```

---

# Krok 16 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY002 Add initial domain entities"
```

---

# Kontrola końcowa DAY002

Lista kontrolna:

```text
[ ] Usunięto domyślny plik Class1.cs
[ ] Utworzono katalog Entities
[ ] Utworzono katalog Common
[ ] Utworzono AuditableEntity
[ ] Utworzono Department
[ ] Utworzono ApplicationRole
[ ] Utworzono ApplicationUser
[ ] Utworzono UserDepartmentAccess
[ ] Utworzono UserRoleAssignment
[ ] Utworzono Patient
[ ] Utworzono MedicalRecord
[ ] Utworzono AccessLog
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — błąd namespace

Objaw:

```text
The type or namespace name 'Common' does not exist
```

Sprawdź, czy w pliku `AuditableEntity.cs` masz:

```csharp
namespace HospitalAccessControl.Domain.Common;
```

oraz czy w encjach `Patient` i `MedicalRecord` masz:

```csharp
using HospitalAccessControl.Domain.Common;
```

---

## Problem 2 — błąd DateOnly

Objaw:

```text
The type or namespace name 'DateOnly' could not be found
```

Najczęstsza przyczyna:

- projekt nie używa `net10.0`,
- SDK jest za stare.

Sprawdź:

```powershell
dotnet --list-sdks
```

oraz plik:

```text
src\HospitalAccessControl.Domain\HospitalAccessControl.Domain.csproj
```

Powinno być:

```xml
<TargetFramework>net10.0</TargetFramework>
```

---

## Problem 3 — nullability warning

Możesz dostać ostrzeżenia dotyczące pól referencyjnych.

W naszych encjach używamy:

```csharp
= null!;
```

dla właściwości nawigacyjnych, które ustawi EF Core.

Przykład:

```csharp
public Department Department { get; set; } = null!;
```

To jest poprawne na tym etapie.

---

## Problem 4 — literówka w nazwie pliku lub klasy

Jeżeli build pokazuje błąd, sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Domain\Entities
```

Nazwy plików powinny odpowiadać nazwom klas.

---

# Efekt końcowy DAY002

Po zakończeniu DAY002 masz pierwszą wersję modelu domenowego:

```text
Department
ApplicationRole
ApplicationUser
UserDepartmentAccess
UserRoleAssignment
Patient
MedicalRecord
AccessLog
```

To wystarczy, aby w DAY003 przejść do:

```text
DbContext
konfiguracje EF Core
mapowanie schematów SQL
mapowanie relacji
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach drugiego etapu implementacji przygotowano warstwę domenową aplikacji. Utworzono podstawowe encje reprezentujące oddziały szpitalne, użytkowników aplikacyjnych, role, przypisania użytkowników do oddziałów i ról, pacjentów, dokumentację medyczną oraz audyt dostępu. Model domenowy został zaprojektowany tak, aby wspierał późniejszą implementację mechanizmu Row-Level Security w SQL Server, gdzie kluczową rolę pełni przypisanie użytkownika do oddziału oraz kolumna `DepartmentId` w tabelach medycznych.
