# Relacje i diagram ERD

## Relacje główne

```text
dictionary.Departments 1 --- * medical.Patients
dictionary.Departments 1 --- * medical.PatientAdmissions
dictionary.Departments 1 --- * medical.MedicalRecords
dictionary.Departments 1 --- * medical.PatientNotes
dictionary.Departments 1 --- * security.UserDepartmentAccess

dictionary.ApplicationRoles 1 --- * security.UserRoleAssignments

security.ApplicationUsers 1 --- * security.UserDepartmentAccess
security.ApplicationUsers 1 --- * security.UserRoleAssignments

medical.Patients 1 --- * medical.PatientAdmissions
medical.Patients 1 --- * medical.MedicalRecords
medical.Patients 1 --- * medical.PatientNotes
medical.Patients 1 --- * audit.AccessLog
```

## Diagram ERD — Mermaid

```mermaid
erDiagram
    DEPARTMENTS ||--o{ PATIENTS : has
    DEPARTMENTS ||--o{ PATIENT_ADMISSIONS : has
    DEPARTMENTS ||--o{ MEDICAL_RECORDS : has
    DEPARTMENTS ||--o{ PATIENT_NOTES : has
    DEPARTMENTS ||--o{ USER_DEPARTMENT_ACCESS : grants

    GENDERS ||--o{ PATIENTS : classifies
    PATIENT_STATUSES ||--o{ PATIENTS : classifies

    PATIENTS ||--o{ PATIENT_ADMISSIONS : has
    PATIENTS ||--o{ MEDICAL_RECORDS : has
    PATIENTS ||--o{ PATIENT_NOTES : has
    PATIENTS ||--o{ ACCESS_LOG : audited

    MEDICAL_RECORD_TYPES ||--o{ MEDICAL_RECORDS : classifies

    APPLICATION_USERS ||--o{ USER_DEPARTMENT_ACCESS : assigned
    APPLICATION_USERS ||--o{ USER_ROLE_ASSIGNMENTS : has

    APPLICATION_ROLES ||--o{ USER_ROLE_ASSIGNMENTS : assigned

    ACCESS_ACTION_TYPES ||--o{ ACCESS_LOG : classifies
    SECURITY_EVENT_TYPES ||--o{ SECURITY_EVENTS : classifies

    DEPARTMENTS {
        int DepartmentId PK
        string Code
        string Name
        bool IsActive
        datetime CreatedAt
    }

    PATIENTS {
        int PatientId PK
        string MedicalNumber
        string FirstName
        string LastName
        string Pesel
        date DateOfBirth
        int GenderId FK
        int DepartmentId FK
        int PatientStatusId FK
        datetime CreatedAt
        string CreatedBy
        bool IsDeleted
    }

    PATIENT_ADMISSIONS {
        int PatientAdmissionId PK
        int PatientId FK
        int DepartmentId FK
        datetime AdmissionDate
        datetime DischargeDate
        string AdmissionReason
        string Status
    }

    MEDICAL_RECORDS {
        int MedicalRecordId PK
        int PatientId FK
        int DepartmentId FK
        int MedicalRecordTypeId FK
        string Title
        string Diagnosis
        string Treatment
        datetime CreatedAt
        string CreatedBy
        bool IsDeleted
    }

    PATIENT_NOTES {
        int PatientNoteId PK
        int PatientId FK
        int DepartmentId FK
        string NoteText
        datetime CreatedAt
        string CreatedBy
        bool IsDeleted
    }

    APPLICATION_USERS {
        int ApplicationUserId PK
        string DomainLogin
        string SamAccountName
        string DisplayName
        string Email
        bool IsActive
    }

    APPLICATION_ROLES {
        int ApplicationRoleId PK
        string Code
        string Name
        bool IsActive
    }

    USER_DEPARTMENT_ACCESS {
        int UserDepartmentAccessId PK
        int ApplicationUserId FK
        int DepartmentId FK
        datetime ValidFrom
        datetime ValidTo
        bool IsActive
    }

    USER_ROLE_ASSIGNMENTS {
        int UserRoleAssignmentId PK
        int ApplicationUserId FK
        int ApplicationRoleId FK
        datetime ValidFrom
        datetime ValidTo
        bool IsActive
    }

    ACCESS_LOG {
        bigint AccessLogId PK
        string DomainLogin
        int PatientId FK
        int ActionTypeId FK
        datetime AccessDate
        string ClientHost
        bool WasSuccessful
    }

    SECURITY_EVENTS {
        bigint SecurityEventId PK
        string DomainLogin
        int SecurityEventTypeId FK
        datetime EventDate
        string Severity
        string Description
    }
```
