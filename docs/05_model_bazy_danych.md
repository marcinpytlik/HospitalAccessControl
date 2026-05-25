# Model bazy danych

## Baza

```text
HospitalAccessControlDb
```

## Schematy

- `dictionary`
- `medical`
- `security`
- `audit`
- `app`

## Tabele

### Schemat `dictionary`

- `dictionary.Departments`
- `dictionary.Roles`
- `dictionary.PatientStatuses`
- `dictionary.MedicalRecordTypes`

### Schemat `medical`

- `medical.Patients`
- `medical.PatientAdmissions`
- `medical.MedicalRecords`
- `medical.PatientNotes`

### Schemat `security`

- `security.ApplicationUsers`
- `security.UserDepartmentAccess`
- `security.UserRoleAssignments`

### Schemat `audit`

- `audit.AccessLog`
- `audit.SecurityEvents`

## Diagram ERD

```mermaid
erDiagram
    DEPARTMENTS ||--o{ PATIENTS : has
    DEPARTMENTS ||--o{ PATIENT_ADMISSIONS : has
    DEPARTMENTS ||--o{ MEDICAL_RECORDS : has
    DEPARTMENTS ||--o{ USER_DEPARTMENT_ACCESS : grants

    PATIENTS ||--o{ PATIENT_ADMISSIONS : has
    PATIENTS ||--o{ MEDICAL_RECORDS : has
    PATIENTS ||--o{ PATIENT_NOTES : has
    PATIENTS ||--o{ ACCESS_LOG : audited

    APPLICATION_USERS ||--o{ USER_DEPARTMENT_ACCESS : assigned
    APPLICATION_USERS ||--o{ USER_ROLE_ASSIGNMENTS : has

    APPLICATION_ROLES ||--o{ USER_ROLE_ASSIGNMENTS : assigned
```

## Założenie RLS

Mechanizm Row-Level Security filtruje dane według `DepartmentId` oraz mapowania aktualnego użytkownika w tabeli `security.UserDepartmentAccess`.

```text
Aktualny użytkownik domenowy
  ↓
security.ApplicationUsers
  ↓
security.UserDepartmentAccess
  ↓
DepartmentId
  ↓
medical.Patients.DepartmentId
```
