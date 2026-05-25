# Aplikacja .NET 10

## Nazwa aplikacji

```text
HospitalAccessControl
```

## Stack

- .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core 10
- SQL Server 2022
- Windows Authentication
- IIS

## Solution

```text
HospitalAccessControl.slnx

src/
├── HospitalAccessControl.Domain
├── HospitalAccessControl.Application
├── HospitalAccessControl.Infrastructure
└── HospitalAccessControl.Web

tests/
├── HospitalAccessControl.Tests
└── HospitalAccessControl.IntegrationTests
```

## Projekty

### `HospitalAccessControl.Domain`

Encje domenowe:

- `Department`
- `Patient`
- `PatientAdmission`
- `MedicalRecord`
- `PatientNote`
- `ApplicationUser`
- `ApplicationRole`
- `UserDepartmentAccess`
- `UserRoleAssignment`
- `AccessLog`
- `SecurityEvent`

### `HospitalAccessControl.Application`

Usługi i interfejsy:

- `ICurrentUserService`
- `IPatientReadService`
- `IMedicalRecordService`
- `IAuditService`
- `ISecurityPolicyService`

DTO:

- `PatientListItemDto`
- `PatientDetailsDto`
- `MedicalRecordDto`
- `CreateMedicalRecordDto`
- `CurrentUserDto`
- `AccessLogDto`

### `HospitalAccessControl.Infrastructure`

- `HospitalAccessControlDbContext`
- konfiguracje encji EF Core,
- repozytoria,
- integracja z SQL Server.

### `HospitalAccessControl.Web`

Razor Pages:

```text
Pages/
├── Index.cshtml
├── AccessDenied.cshtml
├── Patients/
│   ├── Index.cshtml
│   └── Details.cshtml
├── MedicalRecords/
│   ├── Index.cshtml
│   ├── Details.cshtml
│   └── Create.cshtml
├── Audit/
│   ├── Index.cshtml
│   └── Details.cshtml
└── Admin/
    ├── Users.cshtml
    ├── Roles.cshtml
    └── Departments.cshtml
```
