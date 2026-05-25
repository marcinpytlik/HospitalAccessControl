# Proponowana kolejność implementacji SQL

## Kolejność plików

```text
01_create_database.sql
02_create_schemas.sql
03_create_dictionary_tables.sql
04_create_medical_tables.sql
05_create_security_tables.sql
06_create_audit_tables.sql
07_create_app_tables.sql
08_create_constraints.sql
09_create_indexes.sql
10_seed_dictionary_data.sql
11_seed_users_and_access.sql
12_seed_patients.sql
13_create_database_roles.sql
14_create_ad_logins_and_users.sql
15_grant_permissions.sql
16_create_rls_function.sql
17_create_rls_policy.sql
18_create_dynamic_data_masking.sql
19_create_sql_audit.sql
20_test_rls_access.sql
```

## Etap minimalny

Do pierwszej wersji wystarczy:

```text
01_create_database.sql
02_create_schemas.sql
03_create_dictionary_tables.sql
04_create_medical_tables.sql
05_create_security_tables.sql
06_create_audit_tables.sql
10_seed_dictionary_data.sql
11_seed_users_and_access.sql
12_seed_patients.sql
13_create_database_roles.sql
14_create_ad_logins_and_users.sql
15_grant_permissions.sql
16_create_rls_function.sql
17_create_rls_policy.sql
20_test_rls_access.sql
```

## Etap rozszerzony

W kolejnym kroku można dodać:

```text
18_create_dynamic_data_masking.sql
19_create_sql_audit.sql
```
