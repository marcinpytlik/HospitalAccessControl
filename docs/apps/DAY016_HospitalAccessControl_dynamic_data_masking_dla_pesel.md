# DAY016 — HospitalAccessControl  
## Dynamic Data Masking dla PESEL

## Cel dnia

Celem DAY016 jest dodanie mechanizmu **Dynamic Data Masking** dla kolumny PESEL w tabeli pacjentów.

Do tej pory chroniliśmy rekordy przez:

```text
Row-Level Security
```

Teraz dodajemy ochronę kolumny:

```text
medical.Patients.Pesel
```

---

## Efekt końcowy DAY016

Po zakończeniu dnia powinieneś mieć:

- skrypt SQL dodający maskę na kolumnę `Pesel`,
- test użytkownikiem bez uprawnienia `UNMASK`,
- test użytkownikiem z uprawnieniem `UNMASK`,
- opis ograniczeń DDM,
- gotowy materiał do pracy inżynierskiej.

---

# 1. RLS a DDM

## RLS

Odpowiada na pytanie:

```text
które rekordy użytkownik może zobaczyć?
```

## DDM

Odpowiada na pytanie:

```text
jakie wartości kolumn mają być zamaskowane?
```

Przykład:

```text
PESEL 90010100001
```

może być pokazany jako:

```text
XXXXXXX0001
```

---

# Krok 1 — katalog i pliki SQL

```powershell
Set-Location C:\Projects\HospitalAccessControl

New-Item -ItemType Directory -Force .\sql\08_audit
New-Item -ItemType Directory -Force .\sql\09_tests

New-Item -ItemType File -Force .\sql\08_audit\18_create_dynamic_data_masking.sql
New-Item -ItemType File -Force .\sql\09_tests\22_test_dynamic_data_masking.sql
```

---

# Krok 2 — skrypt dodający maskę

Otwórz:

```text
sql\08_audit\18_create_dynamic_data_masking.sql
```

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY016
    Dynamic Data Masking dla kolumny PESEL.
*/

IF NOT EXISTS
(
    SELECT 1
    FROM sys.masked_columns
    WHERE object_id = OBJECT_ID(N'medical.Patients')
      AND name = N'Pesel'
      AND is_masked = 1
)
BEGIN
    ALTER TABLE medical.Patients
    ALTER COLUMN Pesel
        ADD MASKED WITH (FUNCTION = 'partial(0,"XXXXXXX",4)');
END
GO

SELECT
    c.name,
    c.is_masked,
    c.masking_function
FROM sys.masked_columns AS c
WHERE c.object_id = OBJECT_ID(N'medical.Patients');
GO
```

---

# Krok 3 — uruchomienie skryptu

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\08_audit\18_create_dynamic_data_masking.sql
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\08_audit\18_create_dynamic_data_masking.sql
```

Oczekiwany wynik:

```text
Pesel  1  partial(0,"XXXXXXX",4)
```

---

# Krok 4 — sprawdzenie sys.masked_columns

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT c.name, c.is_masked, c.masking_function FROM sys.masked_columns c WHERE c.object_id = OBJECT_ID(N'medical.Patients');"
```

---

# Krok 5 — ważna uwaga o testowaniu

Jeżeli testujesz jako:

```text
sysadmin
db_owner
właściciel bazy
```

możesz widzieć pełny PESEL.

DDM działa dla użytkowników, którzy nie mają:

```sql
UNMASK
```

Dlatego tworzymy testowego użytkownika SQL.

---

# Krok 6 — testowy login SQL

Otwórz:

```text
sql\09_tests\22_test_dynamic_data_masking.sql
```

Wklej:

```sql
USE [master];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.sql_logins
    WHERE name = N'hac_mask_test'
)
BEGIN
    CREATE LOGIN hac_mask_test
    WITH PASSWORD = 'Str0ng!Password123',
         CHECK_POLICY = OFF;
END
GO

USE [HospitalAccessControlDb_Dev];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'hac_mask_test'
)
BEGIN
    CREATE USER hac_mask_test FOR LOGIN hac_mask_test;
END
GO

GRANT SELECT ON medical.Patients TO hac_mask_test;
GRANT SELECT ON dictionary.Departments TO hac_mask_test;
GO

EXECUTE AS USER = 'hac_mask_test';

SELECT TOP (10)
    PatientId,
    MedicalNumber,
    FirstName,
    LastName,
    Pesel
FROM medical.Patients
ORDER BY PatientId;

REVERT;
GO
```

---

# Krok 7 — uruchomienie testu

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\22_test_dynamic_data_masking.sql
```

Oczekiwany wynik:

```text
PESEL zamaskowany
```

---

# Krok 8 — test UNMASK

Nadaj uprawnienie:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "GRANT UNMASK TO hac_mask_test;"
```

Wykonaj test ponownie:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\22_test_dynamic_data_masking.sql
```

Oczekiwany wynik:

```text
PESEL jawny
```

Odbierz uprawnienie:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "REVOKE UNMASK TO hac_mask_test;"
```

---

# Krok 9 — wpływ na aplikację

Jeżeli aplikacja działa jako użytkownik z wysokimi uprawnieniami, może widzieć pełny PESEL.

Docelowo aplikacja powinna działać na koncie runtime:

```text
HOSPITAL\svc_hac_app
```

bez uprawnienia:

```sql
UNMASK
```

---

# Krok 10 — opis do dokumentacji

Dodaj do dokumentacji:

```text
Dynamic Data Masking nie jest szyfrowaniem. Mechanizm ten ogranicza widoczność wartości kolumn w wynikach zapytań dla użytkowników bez uprawnienia UNMASK. DDM nie zastępuje Row-Level Security, ale stanowi dodatkową warstwę ograniczenia ekspozycji danych wrażliwych.
```

---

# Krok 11 — commit

```powershell
git status
git add .
git commit -m "DAY016 Add dynamic data masking for PESEL"
```

---

# Kontrola końcowa DAY016

```text
[ ] Utworzono 18_create_dynamic_data_masking.sql
[ ] Dodano maskę na medical.Patients.Pesel
[ ] sys.masked_columns pokazuje is_masked = 1
[ ] Utworzono login hac_mask_test
[ ] Użytkownik bez UNMASK widzi PESEL zamaskowany
[ ] Użytkownik z UNMASK widzi PESEL jawny
[ ] Opisano ograniczenia DDM
```

---

# Najczęstsze problemy

## Problem 1 — PESEL nadal jawny

Prawdopodobnie testujesz jako `sysadmin` albo użytkownik z `UNMASK`.

## Problem 2 — błąd przy ALTER COLUMN

Sprawdź definicję kolumny `Pesel` i czy maska nie jest już dodana.

## Problem 3 — aplikacja nadal widzi PESEL jawny

To zależy od konta połączenia SQL. Poprawimy to przy least privilege.

---

# Efekt końcowy DAY016

Masz dwa mechanizmy ochrony:

```text
RLS -> ogranicza rekordy
DDM -> maskuje kolumny
```

Następny etap:

```text
DAY017 — SQL Server Audit
```
