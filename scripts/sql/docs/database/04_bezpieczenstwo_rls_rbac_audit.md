# Projekt bezpieczeństwa bazy danych

## 1. Row-Level Security

### Tabele objęte RLS

```text
medical.Patients
medical.PatientAdmissions
medical.MedicalRecords
medical.PatientNotes
```

### Główna zasada

Użytkownik widzi rekord, jeżeli aktualny login domenowy znajduje się w tabeli `security.ApplicationUsers` i ma aktywny wpis w `security.UserDepartmentAccess` dla `DepartmentId` zgodnego z rekordem.

### Logika działania

```text
SUSER_SNAME() / ORIGINAL_LOGIN()
        ↓
security.ApplicationUsers.DomainLogin
        ↓
security.UserDepartmentAccess.ApplicationUserId
        ↓
security.UserDepartmentAccess.DepartmentId
        ↓
medical.<tabela>.DepartmentId
```

### Specjalne założenia

- `Auditor` może mieć dostęp do audytu, ale niekoniecznie do pełnych danych medycznych.
- `ITAdministrator` nie powinien domyślnie widzieć danych medycznych.
- Administrator techniczny nie jest automatycznie użytkownikiem danych medycznych.

---

## 2. Role bazodanowe

Proponowane role:

```text
db_hac_doctor
db_hac_nurse
db_hac_registration
db_hac_department_manager
db_hac_auditor
db_hac_app_runtime
db_hac_migration
db_hac_monitoring
db_hac_backup
```

## 3. Mapowanie AD → SQL

| Grupa AD | Rola bazodanowa |
|---|---|
| `GG_SQL_HospitalAccessControl_Doctor` | `db_hac_doctor` |
| `GG_SQL_HospitalAccessControl_Nurse` | `db_hac_nurse` |
| `GG_SQL_HospitalAccessControl_Registration` | `db_hac_registration` |
| `GG_SQL_HospitalAccessControl_Manager` | `db_hac_department_manager` |
| `GG_SQL_HospitalAccessControl_Auditor` | `db_hac_auditor` |
| `GG_SQL_HospitalAccessControl_Runtime` | `db_hac_app_runtime` |
| `GG_SQL_HospitalAccessControl_Migration` | `db_hac_migration` |
| `GG_SQL_HospitalAccessControl_Monitoring` | `db_hac_monitoring` |
| `GG_SQL_HospitalAccessControl_Backup` | `db_hac_backup` |

---

## 4. Macierz dostępu do tabel

| Rola | Patients | MedicalRecords | PatientAdmissions | Audit | Security |
|---|---|---|---|---|---|
| `db_hac_doctor` | SELECT | SELECT/INSERT | SELECT | INSERT własny log | brak |
| `db_hac_nurse` | SELECT | SELECT ograniczony | SELECT | INSERT własny log | brak |
| `db_hac_registration` | SELECT ograniczony | brak | SELECT ograniczony | INSERT własny log | brak |
| `db_hac_department_manager` | SELECT | SELECT | SELECT | SELECT raportowy | brak |
| `db_hac_auditor` | SELECT metadane lub brak | brak | brak | SELECT | SELECT ograniczony |
| `db_hac_app_runtime` | EXECUTE/SELECT według potrzeb | EXECUTE/SELECT | EXECUTE/SELECT | INSERT | brak |
| `db_hac_migration` | DDL | DDL | DDL | DDL | DDL |
| `db_hac_monitoring` | brak | brak | brak | SELECT metadane | brak |
| `db_hac_backup` | backup | backup | backup | backup | brak |

---

## 5. Dynamic Data Masking

### Pola do maskowania

```text
medical.Patients.Pesel
medical.Patients.DateOfBirth
```

Opcjonalnie:

```text
medical.Patients.LastName
```

### Założenia

| Rola | PESEL |
|---|---|
| Doctor | pełny |
| Nurse | częściowo widoczny |
| Registration | częściowo widoczny |
| Auditor | zamaskowany lub brak |
| ITAdministrator | brak |

---

## 6. Audyt

### Audyt aplikacyjny

Tabela:

```text
audit.AccessLog
```

Rejestruje:

- kto,
- kiedy,
- jaką akcję wykonał,
- na jakim pacjencie,
- z jakiej aplikacji,
- z jakiego hosta,
- czy operacja była udana.

### SQL Server Audit

SQL Server Audit powinien rejestrować:

- `SELECT` na tabelach medycznych,
- `EXECUTE` na procedurach,
- zmiany uprawnień,
- zmiany ról,
- nieudane próby dostępu,
- działania administracyjne.
