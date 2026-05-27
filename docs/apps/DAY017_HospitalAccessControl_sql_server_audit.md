# DAY017 — HospitalAccessControl  
## SQL Server Audit — audyt po stronie SQL Server

## Cel dnia

Celem DAY017 jest dodanie natywnego mechanizmu **SQL Server Audit**.

Do tej pory audytowaliśmy zdarzenia w aplikacji:

```text
audit.AccessLog
```

Teraz dodamy audyt techniczny po stronie SQL Server, który może rejestrować:

```text
SELECT na tabelach medycznych
zmiany uprawnień
zmiany członkostwa w rolach
zmiany obiektów zabezpieczeń
```

---

## Efekt końcowy DAY017

Po zakończeniu dnia powinieneś mieć:

- katalog audytu na dysku,
- `SERVER AUDIT`,
- `DATABASE AUDIT SPECIFICATION`,
- test generowania zdarzeń,
- test odczytu plików `.sqlaudit`,
- gotowy opis różnicy między audytem aplikacyjnym i SQL Server Audit.

---

# Krok 1 — przygotowanie katalogów

```powershell
Set-Location C:\Projects\HospitalAccessControl

New-Item -ItemType Directory -Force .\sql\08_audit
New-Item -ItemType Directory -Force .\sql\09_tests
New-Item -ItemType Directory -Force C:\SqlAudit\HospitalAccessControl
```

---

# Krok 2 — utworzenie skryptów

```powershell
New-Item -ItemType File -Force .\sql\08_audit\19_create_sql_audit.sql
New-Item -ItemType File -Force .\sql\09_tests\23_test_sql_audit.sql
```

---

# Krok 3 — skrypt tworzący audyt

Otwórz:

```text
sql\08_audit\19_create_sql_audit.sql
```

Wklej:

```sql
USE [master];
GO

IF EXISTS
(
    SELECT 1
    FROM sys.server_file_audits
    WHERE name = N'HospitalAccessControl_ServerAudit'
)
BEGIN
    ALTER SERVER AUDIT HospitalAccessControl_ServerAudit
    WITH (STATE = OFF);

    DROP SERVER AUDIT HospitalAccessControl_ServerAudit;
END
GO

CREATE SERVER AUDIT HospitalAccessControl_ServerAudit
TO FILE
(
    FILEPATH = N'C:\SqlAudit\HospitalAccessControl\',
    MAXSIZE = 100 MB,
    MAX_ROLLOVER_FILES = 10,
    RESERVE_DISK_SPACE = OFF
)
WITH
(
    QUEUE_DELAY = 1000,
    ON_FAILURE = CONTINUE
);
GO

ALTER SERVER AUDIT HospitalAccessControl_ServerAudit
WITH (STATE = ON);
GO

USE [HospitalAccessControlDb_Dev];
GO

IF EXISTS
(
    SELECT 1
    FROM sys.database_audit_specifications
    WHERE name = N'HospitalAccessControl_DatabaseAuditSpec'
)
BEGIN
    ALTER DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec
    WITH (STATE = OFF);

    DROP DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec;
END
GO

CREATE DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec
FOR SERVER AUDIT HospitalAccessControl_ServerAudit
ADD (SELECT ON OBJECT::medical.Patients BY public),
ADD (SELECT ON OBJECT::medical.MedicalRecords BY public),
ADD (DATABASE_PERMISSION_CHANGE_GROUP),
ADD (DATABASE_ROLE_MEMBER_CHANGE_GROUP),
ADD (SCHEMA_OBJECT_PERMISSION_CHANGE_GROUP)
WITH (STATE = ON);
GO
```

---

# Krok 4 — uruchomienie

```powershell
sqlcmd -S localhost -E -i .\sql\08_audit\19_create_sql_audit.sql
```

SQL Express:

```powershell
sqlcmd -S .\SQLEXPRESS -E -i .\sql\08_audit\19_create_sql_audit.sql
```

---

# Krok 5 — weryfikacja

```powershell
sqlcmd -S localhost -E -Q "SELECT name, is_state_enabled FROM sys.server_file_audits WHERE name = N'HospitalAccessControl_ServerAudit';"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT name, is_state_enabled FROM sys.database_audit_specifications WHERE name = N'HospitalAccessControl_DatabaseAuditSpec';"
```

Oczekiwane:

```text
is_state_enabled = 1
```

---

# Krok 6 — wygenerowanie zdarzeń

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT TOP (5) PatientId, MedicalNumber FROM medical.Patients;"
```

---

# Krok 7 — odczyt audytu

Otwórz:

```text
sql\09_tests\23_test_sql_audit.sql
```

Wklej:

```sql
SELECT TOP (100)
    event_time,
    action_id,
    succeeded,
    server_principal_name,
    database_name,
    schema_name,
    object_name,
    statement
FROM sys.fn_get_audit_file
(
    'C:\SqlAudit\HospitalAccessControl\*.sqlaudit',
    DEFAULT,
    DEFAULT
)
ORDER BY event_time DESC;
GO
```

Uruchom:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\23_test_sql_audit.sql
```

---

# Krok 8 — test zmiany uprawnień

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "GRANT SELECT ON dictionary.Departments TO public; REVOKE SELECT ON dictionary.Departments TO public;"
```

Ponownie odczytaj audit file.

---

# Krok 9 — opis do dokumentacji

```text
SQL Server Audit rejestruje zdarzenia na poziomie silnika bazy danych i działa niezależnie od logiki aplikacji. W projekcie wykorzystano go jako dodatkową warstwę audytu technicznego, uzupełniającą audyt aplikacyjny zapisujący zdarzenia biznesowe w tabeli audit.AccessLog.
```

---

# Krok 10 — commit

```powershell
git status
git add .
git commit -m "DAY017 Add SQL Server Audit"
```

---

# Kontrola końcowa DAY017

```text
[ ] Utworzono katalog C:\SqlAudit\HospitalAccessControl
[ ] Utworzono SERVER AUDIT
[ ] Utworzono DATABASE AUDIT SPECIFICATION
[ ] Audyt jest włączony
[ ] SELECT z Patients generuje zdarzenie
[ ] fn_get_audit_file pokazuje wpisy
[ ] Zmiany uprawnień są rejestrowane
```

---

# Najczęstsze problemy

## Brak plików audytu

Sprawdź uprawnienia SQL Server service account do katalogu.

## Brak wpisów

Wykonaj SELECT po utworzeniu audytu i poczekaj chwilę.

## Za dużo danych

W produkcji audyt trzeba projektować selektywnie.

---

# Efekt końcowy DAY017

Masz audyt aplikacyjny i techniczny.

Następny etap:

```text
DAY018 — Windows Authentication
```
