# DAY025 — Dokumentacja techniczna końcowa

## Architektura

```text
HospitalAccessControl.Web
  -> HospitalAccessControl.Application
  -> HospitalAccessControl.Infrastructure
  -> HospitalAccessControl.Domain
  -> SQL Server
```

## Kluczowe mechanizmy

```text
CurrentUserService
SessionContextConnectionInterceptor
Row-Level Security
Dynamic Data Masking
AuditService / audit.AccessLog
SQL Server Audit
Windows Authentication
Role aplikacyjne
Least privilege
```

## Najważniejsze strony

```text
/
/Patients
/Patients/Details/{id}
/Patients/AddRecord/{id}
/Audit
/MyAccess
```

## Najważniejsze tabele

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

## Komendy końcowe

```powershell
dotnet build
dotnet test
```

## Skrypty SQL końcowe

```text
sql/07_rls/16_create_rls_function.sql
sql/07_rls/17_create_rls_policy.sql
sql/08_audit/18_create_dynamic_data_masking.sql
sql/08_audit/19_create_sql_audit.sql
sql/02_security/13_create_database_roles.sql
sql/02_security/14_create_ad_logins_and_users.sql
sql/02_security/15_grant_permissions.sql
```
