# DAY009 — HospitalAccessControl  
## Row-Level Security w SQL Server na podstawie SESSION_CONTEXT

## Cel dnia

Celem DAY009 jest wdrożenie mechanizmu **Row-Level Security** w SQL Server.

Po zakończeniu tego dnia SQL Server będzie filtrował dane pacjentów na podstawie aktualnego użytkownika zapisanego w:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

To oznacza, że użytkownik:

```text
HOSPITAL\doctor.cardio
```

powinien widzieć tylko pacjentów z Kardiologii, a użytkownik:

```text
HOSPITAL\doctor.ortho
```

powinien widzieć tylko pacjentów z Ortopedii.

---

## Efekt końcowy DAY009

Po zakończeniu dnia powinieneś mieć:

- schemat `security` używany do funkcji RLS,
- funkcję predicate:

```text
security.fn_rls_department_access
```

- politykę bezpieczeństwa:

```text
security.HospitalDepartmentSecurityPolicy
```

- RLS włączony dla tabel:

```text
medical.Patients
medical.MedicalRecords
```

- skrypty SQL zapisane w katalogu repo,
- testy ręczne pokazujące różnice między użytkownikami,
- potwierdzenie, że użytkownik bez dostępu do oddziału nie widzi pacjentów.

---

## Dlaczego RLS jest ważne?

Aplikacja może filtrować dane w kodzie, ale to nie wystarcza jako główne zabezpieczenie.

W tym projekcie najważniejsze jest to, że dane są chronione na poziomie bazy danych.

Czyli nawet jeżeli ktoś wykona zapytanie bezpośrednio w SQL Server:

```sql
SELECT *
FROM medical.Patients;
```

to SQL Server i tak zwróci tylko te rekordy, które użytkownik powinien zobaczyć.

To jest bardzo mocny element pracy inżynierskiej.

---

# 1. Jak działa RLS w tym projekcie?

Mechanizm działa według schematu:

```text
SESSION_CONTEXT(N'CurrentUser')
        ↓
security.ApplicationUsers.DomainLogin
        ↓
security.UserDepartmentAccess.ApplicationUserId
        ↓
security.UserDepartmentAccess.DepartmentId
        ↓
medical.Patients.DepartmentId
```

Jeżeli aktualny użytkownik ma aktywny dostęp do oddziału, rekord jest widoczny.

Jeżeli nie ma aktywnego dostępu, rekord jest niewidoczny.

---

# 2. Przykład działania

## Użytkownik kardiologii

```text
SESSION_CONTEXT(N'CurrentUser') = HOSPITAL\doctor.cardio
```

Ten użytkownik ma dostęp do:

```text
DepartmentId = 1
CARD — Kardiologia
```

Wynik:

```text
widzi tylko pacjentów z DepartmentId = 1
```

---

## Użytkownik ortopedii

```text
SESSION_CONTEXT(N'CurrentUser') = HOSPITAL\doctor.ortho
```

Ten użytkownik ma dostęp do:

```text
DepartmentId = 2
ORTH — Ortopedia
```

Wynik:

```text
widzi tylko pacjentów z DepartmentId = 2
```

---

## Administrator IT

```text
SESSION_CONTEXT(N'CurrentUser') = HOSPITAL\it.admin
```

Ten użytkownik nie ma przypisania do oddziału.

Wynik:

```text
nie widzi pacjentów
```

To jest celowe i bardzo ważne:

> Administrator techniczny nie powinien automatycznie mieć dostępu biznesowego do danych medycznych.

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

Sprawdź build po DAY008:

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

# Krok 2 — sprawdzenie danych wymaganych przez RLS

RLS potrzebuje danych w tabelach:

```text
security.ApplicationUsers
security.UserDepartmentAccess
medical.Patients
medical.MedicalRecords
```

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UsersCount FROM security.ApplicationUsers;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS DepartmentAccessCount FROM security.UserDepartmentAccess;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS PatientsCount FROM medical.Patients;"
```

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS MedicalRecordsCount FROM medical.MedicalRecords;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS UsersCount FROM security.ApplicationUsers;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS DepartmentAccessCount FROM security.UserDepartmentAccess;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS PatientsCount FROM medical.Patients;"
```

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT COUNT(*) AS MedicalRecordsCount FROM medical.MedicalRecords;"
```

Oczekiwany wynik:

```text
UsersCount = 12
DepartmentAccessCount = 9
PatientsCount = 40
MedicalRecordsCount = 40
```

Jeżeli te dane się nie zgadzają, wróć do DAY006.

---

# Krok 3 — utworzenie katalogów SQL dla RLS

Tworzymy katalogi:

```powershell
New-Item -ItemType Directory -Force .\sql\07_rls
New-Item -ItemType Directory -Force .\sql\09_tests
```

Sprawdź:

```powershell
Get-ChildItem .\sql
```

---

# Krok 4 — utworzenie plików SQL

Tworzymy trzy pliki:

```powershell
New-Item -ItemType File -Force .\sql\07_rls\16_create_rls_function.sql
New-Item -ItemType File -Force .\sql\07_rls\17_create_rls_policy.sql
New-Item -ItemType File -Force .\sql\09_tests\20_test_rls_access.sql
```

Sprawdź:

```powershell
Get-ChildItem .\sql\07_rls
Get-ChildItem .\sql\09_tests
```

---

# Krok 5 — skrypt funkcji RLS

## Plik

```text
sql\07_rls\16_create_rls_function.sql
```

## Zawartość

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Row-Level Security predicate function

    Funkcja sprawdza, czy aktualny użytkownik aplikacyjny zapisany
    w SESSION_CONTEXT(N'CurrentUser') ma aktywny dostęp do oddziału.
*/

CREATE OR ALTER FUNCTION security.fn_rls_department_access
(
    @DepartmentId int
)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
(
    SELECT 1 AS fn_access_result
    WHERE EXISTS
    (
        SELECT 1
        FROM security.ApplicationUsers AS au
        INNER JOIN security.UserDepartmentAccess AS uda
            ON uda.ApplicationUserId = au.ApplicationUserId
        WHERE
            au.DomainLogin = CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser'))
            AND au.IsActive = 1
            AND uda.IsActive = 1
            AND uda.DepartmentId = @DepartmentId
            AND uda.ValidFrom <= SYSUTCDATETIME()
            AND (uda.ValidTo IS NULL OR uda.ValidTo >= SYSUTCDATETIME())
    )
);
GO
```

---

## Wyjaśnienie funkcji

Funkcja przyjmuje:

```sql
@DepartmentId
```

czyli oddział rekordu, który SQL Server chce zwrócić.

Następnie sprawdza:

```text
czy użytkownik z SESSION_CONTEXT istnieje,
czy jest aktywny,
czy ma aktywny dostęp do danego oddziału,
czy dostęp jest ważny czasowo.
```

Jeżeli warunki są spełnione, funkcja zwraca:

```sql
SELECT 1
```

Jeżeli nie, nie zwraca nic.

Dla RLS oznacza to:

```text
zwrócono 1 -> rekord widoczny
brak wyniku -> rekord niewidoczny
```

---

# Krok 6 — ważna uwaga o SCHEMABINDING

Funkcja predicate używana przez RLS powinna być utworzona z:

```sql
WITH SCHEMABINDING
```

Dlaczego?

- SQL Server wymaga stabilnej definicji obiektów używanych przez RLS,
- zabezpiecza przed przypadkową zmianą tabel zależnych,
- poprawia spójność polityki bezpieczeństwa.

---

# Krok 7 — skrypt polityki RLS

## Plik

```text
sql\07_rls\17_create_rls_policy.sql
```

## Zawartość

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Row-Level Security policy

    Polityka RLS nakłada filtr na tabele medyczne.
*/

IF EXISTS
(
    SELECT 1
    FROM sys.security_policies
    WHERE name = N'HospitalDepartmentSecurityPolicy'
      AND SCHEMA_NAME(schema_id) = N'security'
)
BEGIN
    DROP SECURITY POLICY security.HospitalDepartmentSecurityPolicy;
END
GO

CREATE SECURITY POLICY security.HospitalDepartmentSecurityPolicy
ADD FILTER PREDICATE security.fn_rls_department_access(DepartmentId)
ON medical.Patients,
ADD FILTER PREDICATE security.fn_rls_department_access(DepartmentId)
ON medical.MedicalRecords
WITH (STATE = ON);
GO
```

---

## Co robi ta polityka?

Polityka dodaje filtr do tabel:

```text
medical.Patients
medical.MedicalRecords
```

Każdy SELECT z tych tabel będzie filtrowany przez funkcję:

```text
security.fn_rls_department_access
```

Jeżeli rekord ma `DepartmentId = 1`, użytkownik musi mieć dostęp do `DepartmentId = 1`.

Jeżeli rekord ma `DepartmentId = 2`, użytkownik musi mieć dostęp do `DepartmentId = 2`.

---

# Krok 8 — skrypt testów RLS

## Plik

```text
sql\09_tests\20_test_rls_access.sql
```

## Zawartość

Wklej:

```sql
USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Testy Row-Level Security oparte o SESSION_CONTEXT
*/

PRINT '========================================';
PRINT 'TEST 1: HOSPITAL\doctor.cardio';
PRINT 'Oczekiwany wynik: tylko CARD / DepartmentId = 1';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.cardio';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;

SELECT TOP (20)
    p.PatientId,
    p.MedicalNumber,
    p.FirstName,
    p.LastName,
    p.DepartmentId
FROM medical.Patients AS p
ORDER BY p.PatientId;
GO

PRINT '========================================';
PRINT 'TEST 2: HOSPITAL\doctor.ortho';
PRINT 'Oczekiwany wynik: tylko ORTH / DepartmentId = 2';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.ortho';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;

SELECT TOP (20)
    p.PatientId,
    p.MedicalNumber,
    p.FirstName,
    p.LastName,
    p.DepartmentId
FROM medical.Patients AS p
ORDER BY p.PatientId;
GO

PRINT '========================================';
PRINT 'TEST 3: HOSPITAL\doctor.neuro';
PRINT 'Oczekiwany wynik: tylko NEUR / DepartmentId = 3';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.neuro';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;
GO

PRINT '========================================';
PRINT 'TEST 4: HOSPITAL\nurse.ped';
PRINT 'Oczekiwany wynik: tylko PED / DepartmentId = 5';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\nurse.ped';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;
GO

PRINT '========================================';
PRINT 'TEST 5: HOSPITAL\it.admin';
PRINT 'Oczekiwany wynik: brak pacjentów';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\it.admin';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    COUNT(*) AS PatientsVisibleForItAdmin
FROM medical.Patients;
GO

PRINT '========================================';
PRINT 'TEST 6: brak SESSION_CONTEXT';
PRINT 'Oczekiwany wynik: brak pacjentów';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = NULL;

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    COUNT(*) AS PatientsVisibleWithoutContext
FROM medical.Patients;
GO
```

---

# Krok 9 — uruchomienie funkcji RLS

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\07_rls\16_create_rls_function.sql
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\07_rls\16_create_rls_function.sql
```

Oczekiwany wynik:

```text
Commands completed successfully.
```

---

# Krok 10 — uruchomienie polityki RLS

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\07_rls\17_create_rls_policy.sql
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\07_rls\17_create_rls_policy.sql
```

Oczekiwany wynik:

```text
Commands completed successfully.
```

---

# Krok 11 — sprawdzenie, czy polityka istnieje

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT SCHEMA_NAME(schema_id) AS SchemaName, name, is_enabled FROM sys.security_policies WHERE name = N'HospitalDepartmentSecurityPolicy';"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT SCHEMA_NAME(schema_id) AS SchemaName, name, is_enabled FROM sys.security_policies WHERE name = N'HospitalDepartmentSecurityPolicy';"
```

Oczekiwany wynik:

```text
security  HospitalDepartmentSecurityPolicy  1
```

---

# Krok 12 — sprawdzenie predicate

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT OBJECT_SCHEMA_NAME(object_id) AS SchemaName, OBJECT_NAME(object_id) AS PolicyName, target_object_id, OBJECT_SCHEMA_NAME(target_object_id) AS TargetSchema, OBJECT_NAME(target_object_id) AS TargetTable FROM sys.security_predicates;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "SELECT OBJECT_SCHEMA_NAME(object_id) AS SchemaName, OBJECT_NAME(object_id) AS PolicyName, target_object_id, OBJECT_SCHEMA_NAME(target_object_id) AS TargetSchema, OBJECT_NAME(target_object_id) AS TargetTable FROM sys.security_predicates;"
```

Oczekiwany wynik powinien zawierać:

```text
medical  Patients
medical  MedicalRecords
```

---

# Krok 13 — uruchomienie testów RLS

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\20_test_rls_access.sql
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\20_test_rls_access.sql
```

---

# Krok 14 — oczekiwane wyniki testów

## TEST 1 — doctor.cardio

Oczekiwany wynik:

```text
DepartmentId = 1
DepartmentCode = CARD
PatientsCount = 10
```

Nie powinno być oddziałów:

```text
ORTH
NEUR
EMER
PED
```

---

## TEST 2 — doctor.ortho

Oczekiwany wynik:

```text
DepartmentId = 2
DepartmentCode = ORTH
PatientsCount = 10
```

---

## TEST 3 — doctor.neuro

Oczekiwany wynik:

```text
DepartmentId = 3
DepartmentCode = NEUR
PatientsCount = 10
```

---

## TEST 4 — nurse.ped

Oczekiwany wynik:

```text
DepartmentId = 5
DepartmentCode = PED
PatientsCount = 5
```

---

## TEST 5 — it.admin

Oczekiwany wynik:

```text
PatientsVisibleForItAdmin = 0
```

---

## TEST 6 — brak SESSION_CONTEXT

Oczekiwany wynik:

```text
PatientsVisibleWithoutContext = 0
```

---

# Krok 15 — test szybki dla jednego użytkownika

Jeżeli chcesz szybko sprawdzić jednego użytkownika:

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS PatientsCount FROM medical.Patients GROUP BY DepartmentId ORDER BY DepartmentId;"
```

Oczekiwany wynik:

```text
DepartmentId = 1
PatientsCount = 10
```

---

# Krok 16 — test MedicalRecords

RLS powinien działać również dla dokumentacji medycznej.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS RecordsCount FROM medical.MedicalRecords GROUP BY DepartmentId ORDER BY DepartmentId;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) AS RecordsCount FROM medical.MedicalRecords GROUP BY DepartmentId ORDER BY DepartmentId;"
```

Oczekiwany wynik:

```text
DepartmentId = 1
RecordsCount = 10
```

---

# Krok 17 — bardzo ważny test: administrator IT

Ten test jest jednym z najważniejszych w pracy.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\it.admin'; SELECT COUNT(*) AS PatientsCount FROM medical.Patients; SELECT COUNT(*) AS RecordsCount FROM medical.MedicalRecords;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\it.admin'; SELECT COUNT(*) AS PatientsCount FROM medical.Patients; SELECT COUNT(*) AS RecordsCount FROM medical.MedicalRecords;"
```

Oczekiwany wynik:

```text
PatientsCount = 0
RecordsCount = 0
```

To pokazuje, że administrator techniczny nie ma automatycznego dostępu biznesowego do danych medycznych.

---

# Krok 18 — test z aplikacją po DAY008

W DAY008 na stronie głównej pokazaliśmy:

```text
SESSION_CONTEXT('CurrentUser')
```

Teraz możesz sprawdzić, czy po włączeniu RLS aplikacja nadal startuje.

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Wejdź na stronę główną.

Powinieneś nadal widzieć:

```text
HOSPITAL\doctor.cardio
```

Na razie strona główna nie pobiera jeszcze pacjentów, więc RLS nie wpływa na UI.  
To zmieni się w DAY010, gdy dodamy listę pacjentów.

---

# Krok 19 — zapis wyników testów do pliku

Możesz zapisać wynik testów do pliku tekstowego.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\20_test_rls_access.sql -o .\sql\09_tests\20_test_rls_access_results.txt
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -i .\sql\09_tests\20_test_rls_access.sql -o .\sql\09_tests\20_test_rls_access_results.txt
```

Sprawdź plik:

```powershell
Get-Content .\sql\09_tests\20_test_rls_access_results.txt
```

Ten plik będzie dobrym artefaktem do dokumentacji pracy.

---

# Krok 20 — build końcowy

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

# Krok 21 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY009 Add row-level security based on session context"
```

---

# Kontrola końcowa DAY009

Lista kontrolna:

```text
[ ] Sprawdzono, że dane z DAY006 istnieją
[ ] Utworzono katalog sql\07_rls
[ ] Utworzono katalog sql\09_tests
[ ] Utworzono 16_create_rls_function.sql
[ ] Utworzono 17_create_rls_policy.sql
[ ] Utworzono 20_test_rls_access.sql
[ ] Utworzono funkcję security.fn_rls_department_access
[ ] Utworzono politykę security.HospitalDepartmentSecurityPolicy
[ ] Polityka RLS jest włączona
[ ] RLS działa na medical.Patients
[ ] RLS działa na medical.MedicalRecords
[ ] doctor.cardio widzi tylko CARD
[ ] doctor.ortho widzi tylko ORTH
[ ] doctor.neuro widzi tylko NEUR
[ ] nurse.ped widzi tylko PED
[ ] it.admin nie widzi pacjentów
[ ] brak SESSION_CONTEXT nie widzi pacjentów
[ ] Wyniki testów zapisano opcjonalnie do pliku
[ ] dotnet build kończy się sukcesem
```

---

# Najczęstsze problemy

## Problem 1 — błąd przy tworzeniu funkcji RLS

Objaw:

```text
Cannot schema bind function because name is invalid for schema binding
```

Najczęstsza przyczyna:

- w funkcji użyto obiektów bez pełnego schematu,
- tabela została wskazana bez `security.`,
- funkcja nie ma `WITH SCHEMABINDING`.

Sprawdź, czy w funkcji masz pełne nazwy:

```sql
security.ApplicationUsers
security.UserDepartmentAccess
```

oraz:

```sql
WITH SCHEMABINDING
```

---

## Problem 2 — RLS zwraca zero rekordów dla wszystkich

Sprawdź, czy `SESSION_CONTEXT` jest ustawiony:

```sql
SELECT SESSION_CONTEXT(N'CurrentUser');
```

Sprawdź, czy wartość dokładnie odpowiada `DomainLogin`:

```sql
SELECT DomainLogin
FROM security.ApplicationUsers;
```

Przykład poprawnej zgodności:

```text
SESSION_CONTEXT: HOSPITAL\doctor.cardio
DomainLogin:     HOSPITAL\doctor.cardio
```

---

## Problem 3 — literówka w loginie

Jeżeli ustawisz:

```text
HOSPITAL/doctor.cardio
```

albo:

```text
hospital\doctor.cardio
```

zamiast:

```text
HOSPITAL\doctor.cardio
```

to RLS może nie znaleźć użytkownika.

Na potrzeby demo trzymaj dokładne wartości z seeda.

---

## Problem 4 — błąd przy DROP SECURITY POLICY

Jeżeli polityka istnieje i próbujesz zmienić funkcję, SQL Server może blokować zmianę.

Kolejność powinna być:

```text
DROP SECURITY POLICY
ALTER FUNCTION albo CREATE OR ALTER FUNCTION
CREATE SECURITY POLICY
```

W naszym skrypcie polityki mamy `DROP SECURITY POLICY`, ale jeżeli chcesz zmienić funkcję, najpierw wyłącz lub usuń politykę.

Pomocniczo:

```sql
DROP SECURITY POLICY IF EXISTS security.HospitalDepartmentSecurityPolicy;
GO
```

---

## Problem 5 — użytkownik it.admin widzi dane

Jeżeli `it.admin` widzi pacjentów, sprawdź:

```sql
SELECT *
FROM security.UserDepartmentAccess uda
INNER JOIN security.ApplicationUsers au
    ON au.ApplicationUserId = uda.ApplicationUserId
WHERE au.DomainLogin = N'HOSPITAL\it.admin';
```

Oczekiwany wynik:

```text
brak rekordów
```

Jeżeli rekord istnieje, użytkownik dostał dostęp do oddziału.

---

## Problem 6 — właściciel bazy albo sysadmin widzi wszystko

RLS działa również dla wielu zapytań, ale osoby z bardzo wysokimi uprawnieniami administracyjnymi mogą mieć możliwości obchodzenia lub zmiany zabezpieczeń.

W pracy warto wyraźnie napisać:

> RLS ogranicza dostęp użytkowników aplikacyjnych i technicznych zgodnie z polityką bezpieczeństwa, ale administratorzy z uprawnieniami sysadmin lub właściciela bazy mogą zmieniać polityki bezpieczeństwa. Dlatego zasada najmniejszych uprawnień i ograniczenie dostępu administracyjnego są częścią projektu bezpieczeństwa.

---

# Jak wyłączyć RLS tymczasowo?

Jeżeli chcesz chwilowo wyłączyć politykę:

```sql
ALTER SECURITY POLICY security.HospitalDepartmentSecurityPolicy
WITH (STATE = OFF);
GO
```

Włączyć ponownie:

```sql
ALTER SECURITY POLICY security.HospitalDepartmentSecurityPolicy
WITH (STATE = ON);
GO
```

Sprawdzić stan:

```sql
SELECT
    SCHEMA_NAME(schema_id) AS SchemaName,
    name,
    is_enabled
FROM sys.security_policies
WHERE name = N'HospitalDepartmentSecurityPolicy';
```

---

# Efekt końcowy DAY009

Po zakończeniu DAY009 baza danych aktywnie filtruje dane pacjentów i wpisów medycznych na podstawie:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

To oznacza, że mechanizm bezpieczeństwa działa już na poziomie SQL Server.

Mamy gotowy fundament pod:

```text
DAY010 — Lista pacjentów w Razor Pages
```

W DAY010 dodamy stronę:

```text
/Patients
```

i wtedy aplikacja pokaże praktyczny efekt RLS:

```text
doctor.cardio widzi tylko Kardiologię
doctor.ortho widzi tylko Ortopedię
it.admin nie widzi pacjentów
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach dziewiątego etapu implementacji wdrożono mechanizm Row-Level Security w SQL Server. Przygotowano funkcję predicate `security.fn_rls_department_access`, która sprawdza aktywne przypisanie aktualnego użytkownika do oddziału na podstawie wartości zapisanej w `SESSION_CONTEXT(N'CurrentUser')`. Następnie utworzono politykę bezpieczeństwa `security.HospitalDepartmentSecurityPolicy`, obejmującą tabele `medical.Patients` oraz `medical.MedicalRecords`. Przeprowadzone testy potwierdziły, że użytkownicy widzą wyłącznie dane pacjentów przypisanych do swoich oddziałów, a użytkownik bez przypisania do oddziału nie widzi danych medycznych.
