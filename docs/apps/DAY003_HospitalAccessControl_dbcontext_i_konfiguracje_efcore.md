# DAY003 — HospitalAccessControl  
## DbContext, konfiguracje EF Core i rejestracja Infrastructure

## Cel dnia

Celem DAY003 jest przygotowanie warstwy **Infrastructure** pod obsługę bazy danych przez **Entity Framework Core**.

Po zakończeniu tego dnia powinieneś mieć:

- katalog `Data`,
- katalog `Data/Configurations`,
- klasę `HospitalAccessControlDbContext`,
- osobne konfiguracje EF Core dla encji,
- mapowanie encji na schematy SQL,
- relacje między encjami,
- indeksy i ograniczenia podstawowe,
- klasę rejestrującą Infrastructure w dependency injection,
- wpis `ConnectionStrings` w konfiguracji aplikacji,
- działający `dotnet build`.

Na tym etapie **nie tworzymy jeszcze migracji**.  
Migracja i utworzenie bazy będą w DAY004.

---

## Założenia

Projekt jest już po DAY001 i DAY002.

Powinieneś mieć:

```text
C:\Projects\HospitalAccessControl
```

oraz encje domenowe:

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

Na DAY003 pracujemy głównie w projektach:

```text
src\HospitalAccessControl.Infrastructure
src\HospitalAccessControl.Web
```

---

## Co dzisiaj tworzymy?

W projekcie Infrastructure:

```text
Data/
├── HospitalAccessControlDbContext.cs
└── Configurations/
    ├── DepartmentConfiguration.cs
    ├── ApplicationRoleConfiguration.cs
    ├── ApplicationUserConfiguration.cs
    ├── UserDepartmentAccessConfiguration.cs
    ├── UserRoleAssignmentConfiguration.cs
    ├── PatientConfiguration.cs
    ├── MedicalRecordConfiguration.cs
    └── AccessLogConfiguration.cs

DependencyInjection/
└── ServiceCollectionExtensions.cs
```

---

## Mapowanie encji na schematy SQL

| Encja | Schemat | Tabela |
|---|---|---|
| `Department` | `dictionary` | `Departments` |
| `ApplicationRole` | `dictionary` | `ApplicationRoles` |
| `ApplicationUser` | `security` | `ApplicationUsers` |
| `UserDepartmentAccess` | `security` | `UserDepartmentAccess` |
| `UserRoleAssignment` | `security` | `UserRoleAssignments` |
| `Patient` | `medical` | `Patients` |
| `MedicalRecord` | `medical` | `MedicalRecords` |
| `AccessLog` | `audit` | `AccessLog` |

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

Sprawdź build po DAY002:

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

# Krok 2 — utworzenie katalogów Infrastructure

Tworzymy katalogi:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Data
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\DependencyInjection
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure
```

Powinieneś zobaczyć między innymi:

```text
Data
DependencyInjection
HospitalAccessControl.Infrastructure.csproj
```

---

# Krok 3 — usunięcie domyślnej klasy Class1.cs

Jeśli istnieje domyślny plik:

```text
Class1.cs
```

usuń go:

```powershell
Remove-Item .\src\HospitalAccessControl.Infrastructure\Class1.cs -ErrorAction SilentlyContinue
```

---

# Krok 4 — utworzenie plików DbContext i konfiguracji

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\HospitalAccessControlDbContext.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\DepartmentConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationRoleConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationUserConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\UserDepartmentAccessConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\UserRoleAssignmentConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\PatientConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\MedicalRecordConfiguration.cs
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Data\Configurations\AccessLogConfiguration.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Data\Configurations
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\DependencyInjection
```

---

# Krok 5 — DbContext

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\HospitalAccessControlDbContext.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Data;

public sealed class HospitalAccessControlDbContext : DbContext
{
    public HospitalAccessControlDbContext(DbContextOptions<HospitalAccessControlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<UserDepartmentAccess> UserDepartmentAccesses => Set<UserDepartmentAccess>();

    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalAccessControlDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
```

## Co robi DbContext?

`HospitalAccessControlDbContext` jest główną klasą dostępu do bazy danych.  
W kolejnych dniach będzie odpowiadać za:

- tworzenie migracji,
- mapowanie encji na tabele,
- relacje,
- zapytania EF Core,
- zapis audytu aplikacyjnego,
- dostęp do pacjentów i dokumentacji.

---

# Krok 6 — DepartmentConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\DepartmentConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments", "dictionary");

        builder.HasKey(x => x.DepartmentId);

        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("UQ_Departments_Code");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Departments_Name");
    }
}
```

---

# Krok 7 — ApplicationRoleConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationRoleConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("ApplicationRoles", "dictionary");

        builder.HasKey(x => x.ApplicationRoleId);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationRoles_Code");
    }
}
```

---

# Krok 8 — ApplicationUserConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\ApplicationUserConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers", "security");

        builder.HasKey(x => x.ApplicationUserId);

        builder.Property(x => x.DomainLogin)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SamAccountName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.DomainLogin)
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationUsers_DomainLogin");

        builder.HasIndex(x => x.SamAccountName)
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationUsers_SamAccountName");
    }
}
```

---

# Krok 9 — UserDepartmentAccessConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\UserDepartmentAccessConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class UserDepartmentAccessConfiguration : IEntityTypeConfiguration<UserDepartmentAccess>
{
    public void Configure(EntityTypeBuilder<UserDepartmentAccess> builder)
    {
        builder.ToTable("UserDepartmentAccess", "security");

        builder.HasKey(x => x.UserDepartmentAccessId);

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.ValidTo);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.DepartmentAccesses)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.UserDepartmentAccesses)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationUserId)
            .HasDatabaseName("IX_UserDepartmentAccess_ApplicationUserId");

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_UserDepartmentAccess_DepartmentId");

        builder.HasIndex(x => new { x.ApplicationUserId, x.DepartmentId, x.IsActive })
            .HasDatabaseName("IX_UserDepartmentAccess_User_Department_Active");
    }
}
```

---

# Krok 10 — UserRoleAssignmentConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\UserRoleAssignmentConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoleAssignments", "security");

        builder.HasKey(x => x.UserRoleAssignmentId);

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.ValidTo);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.RoleAssignments)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApplicationRole)
            .WithMany(x => x.UserRoleAssignments)
            .HasForeignKey(x => x.ApplicationRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationUserId)
            .HasDatabaseName("IX_UserRoleAssignments_ApplicationUserId");

        builder.HasIndex(x => x.ApplicationRoleId)
            .HasDatabaseName("IX_UserRoleAssignments_ApplicationRoleId");

        builder.HasIndex(x => new { x.ApplicationUserId, x.ApplicationRoleId, x.IsActive })
            .HasDatabaseName("IX_UserRoleAssignments_User_Role_Active");
    }
}
```

---

# Krok 11 — PatientConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\PatientConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients", "medical");

        builder.HasKey(x => x.PatientId);

        builder.Property(x => x.MedicalNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Pesel)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.GenderCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.PatientStatusCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Patients)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_Patients_DepartmentId");

        builder.HasIndex(x => x.MedicalNumber)
            .IsUnique()
            .HasDatabaseName("UQ_Patients_MedicalNumber");

        builder.HasIndex(x => new { x.LastName, x.FirstName })
            .HasDatabaseName("IX_Patients_LastName_FirstName");

        builder.HasIndex(x => x.Pesel)
            .HasDatabaseName("IX_Patients_Pesel");

        builder.HasIndex(x => x.PatientStatusCode)
            .HasDatabaseName("IX_Patients_Status");
    }
}
```

---

# Krok 12 — MedicalRecordConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\MedicalRecordConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecords", "medical");

        builder.HasKey(x => x.MedicalRecordId);

        builder.Property(x => x.RecordTypeCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Diagnosis)
            .HasMaxLength(4000);

        builder.Property(x => x.Treatment)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.MedicalRecords)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.MedicalRecords)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_MedicalRecords_PatientId");

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_MedicalRecords_DepartmentId");

        builder.HasIndex(x => x.RecordTypeCode)
            .HasDatabaseName("IX_MedicalRecords_RecordTypeCode");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_MedicalRecords_CreatedAt");
    }
}
```

---

# Krok 13 — AccessLogConfiguration

## Plik

```text
src\HospitalAccessControl.Infrastructure\Data\Configurations\AccessLogConfiguration.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLog", "audit");

        builder.HasKey(x => x.AccessLogId);

        builder.Property(x => x.DomainLogin)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ActionCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ObjectName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.AccessDate)
            .IsRequired();

        builder.Property(x => x.ClientHost)
            .HasMaxLength(256);

        builder.Property(x => x.ApplicationName)
            .HasMaxLength(256);

        builder.Property(x => x.WasSuccessful)
            .IsRequired();

        builder.Property(x => x.AdditionalInfo)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.DomainLogin)
            .HasDatabaseName("IX_AccessLog_DomainLogin");

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_AccessLog_PatientId");

        builder.HasIndex(x => x.AccessDate)
            .HasDatabaseName("IX_AccessLog_AccessDate");

        builder.HasIndex(x => x.ActionCode)
            .HasDatabaseName("IX_AccessLog_ActionCode");

        builder.HasIndex(x => x.WasSuccessful)
            .HasDatabaseName("IX_AccessLog_WasSuccessful");
    }
}
```

---

# Krok 14 — rejestracja Infrastructure w DI

## Plik

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

## Zawartość

```csharp
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalAccessControl.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HospitalAccessControlDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'HospitalAccessControlDb' was not found.");
        }

        services.AddDbContext<HospitalAccessControlDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
```

## Co robi ta klasa?

Dzięki niej projekt `Web` będzie mógł w prosty sposób dodać warstwę Infrastructure:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

---

# Krok 15 — connection string w Web

## Plik

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Jeżeli plik istnieje, uzupełnij go.  
Jeżeli nie istnieje, utwórz go.

## Wariant dla lokalnego SQL Server Developer / default instance

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Wariant dla SQL Express

Jeżeli używasz SQL Express, użyj:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=.\\SQLEXPRESS;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Na razie tylko zapisujemy connection string.  
Bazy jeszcze nie tworzymy.

---

# Krok 16 — podłączenie Infrastructure w Program.cs

## Plik

```text
src\HospitalAccessControl.Web\Program.cs
```

Dodaj na górze pliku:

```csharp
using HospitalAccessControl.Infrastructure.DependencyInjection;
```

Następnie znajdź fragment:

```csharp
builder.Services.AddRazorPages();
```

I pod nim dodaj:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

Docelowy fragment powinien wyglądać tak:

```csharp
using HospitalAccessControl.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
```

Reszta pliku może zostać taka, jak wygenerował ją template Razor Pages.

---

# Krok 17 — build

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

# Krok 18 — uruchomienie aplikacji

Uruchom aplikację:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Na tym etapie aplikacja może próbować utworzyć `DbContext`, ale nie powinna jeszcze wykonywać zapytań do bazy.

Jeżeli strona startowa się uruchamia, jest dobrze.

Zatrzymanie:

```text
CTRL + C
```

---

# Krok 19 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY003 Add EF Core DbContext and entity configurations"
```

---

# Kontrola końcowa DAY003

Lista kontrolna:

```text
[ ] Usunięto Class1.cs z Infrastructure
[ ] Utworzono katalog Data
[ ] Utworzono katalog Data\Configurations
[ ] Utworzono katalog DependencyInjection
[ ] Utworzono HospitalAccessControlDbContext
[ ] Utworzono DepartmentConfiguration
[ ] Utworzono ApplicationRoleConfiguration
[ ] Utworzono ApplicationUserConfiguration
[ ] Utworzono UserDepartmentAccessConfiguration
[ ] Utworzono UserRoleAssignmentConfiguration
[ ] Utworzono PatientConfiguration
[ ] Utworzono MedicalRecordConfiguration
[ ] Utworzono AccessLogConfiguration
[ ] Utworzono ServiceCollectionExtensions
[ ] Dodano connection string w appsettings.Development.json
[ ] Dodano AddInfrastructure w Program.cs
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — brak paczek EF Core

Objaw:

```text
The type or namespace name 'EntityFrameworkCore' does not exist in the namespace 'Microsoft'
```

Rozwiązanie:

Sprawdź, czy paczki są dodane do Infrastructure:

```powershell
dotnet list .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj package
```

Jeżeli brakuje, dodaj:

```powershell
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add .\src\HospitalAccessControl.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

---

## Problem 2 — Web nie widzi AddInfrastructure

Objaw:

```text
The name 'AddInfrastructure' does not exist in the current context
```

Albo:

```text
IServiceCollection does not contain a definition for AddInfrastructure
```

Sprawdź, czy w `Program.cs` masz:

```csharp
using HospitalAccessControl.Infrastructure.DependencyInjection;
```

Sprawdź też, czy projekt Web ma referencję do Infrastructure:

```powershell
dotnet list .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference
```

Jeżeli nie ma, dodaj:

```powershell
dotnet add .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj reference .\src\HospitalAccessControl.Infrastructure\HospitalAccessControl.Infrastructure.csproj
```

---

## Problem 3 — błąd connection string

Objaw:

```text
Connection string 'HospitalAccessControlDb' was not found.
```

Sprawdź plik:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Powinien zawierać:

```json
{
  "ConnectionStrings": {
    "HospitalAccessControlDb": "Server=localhost;Database=HospitalAccessControlDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Upewnij się, że aplikację uruchamiasz w środowisku Development.

Możesz sprawdzić:

```powershell
$env:ASPNETCORE_ENVIRONMENT
```

Jeżeli trzeba, ustaw:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

---

## Problem 4 — błąd relacji EF Core

Objaw może być podobny do:

```text
Unable to determine the relationship represented by navigation
```

Najczęstsza przyczyna:

- literówka w właściwości nawigacyjnej,
- inna nazwa kolekcji w encji,
- brak właściwości FK.

Sprawdź szczególnie:

```text
Department.Patients
Department.MedicalRecords
Department.UserDepartmentAccesses

ApplicationUser.DepartmentAccesses
ApplicationUser.RoleAssignments

ApplicationRole.UserRoleAssignments

Patient.MedicalRecords
```

---

## Problem 5 — błąd typu DateOnly

Jeżeli EF Core albo build zgłasza problem z `DateOnly`, sprawdź:

```powershell
dotnet --list-sdks
```

oraz:

```text
<TargetFramework>net10.0</TargetFramework>
```

w projektach.

---

# Efekt końcowy DAY003

Po zakończeniu DAY003 masz przygotowaną warstwę Infrastructure:

```text
HospitalAccessControlDbContext
konfiguracje EF Core
mapowanie schematów SQL
mapowanie relacji
connection string
rejestracja DI
```

Projekt powinien się budować.

Następny etap:

```text
DAY004 — pierwsza migracja i utworzenie bazy HospitalAccessControlDb_Dev
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach trzeciego etapu implementacji przygotowano warstwę Infrastructure odpowiedzialną za dostęp do danych. Utworzono klasę `HospitalAccessControlDbContext` oraz osobne klasy konfiguracji encji zgodne z podejściem Fluent API w Entity Framework Core. Encje domenowe zostały odwzorowane na logiczne schematy SQL: `dictionary`, `security`, `medical` oraz `audit`. Zdefiniowano relacje między użytkownikami, rolami, oddziałami, pacjentami i wpisami medycznymi. Przygotowano również rejestrację warstwy Infrastructure w kontenerze dependency injection aplikacji webowej.
