# Model RBAC

## Role biznesowe

- `Doctor`
- `Nurse`
- `Registration`
- `DepartmentManager`
- `Auditor`
- `ITAdministrator`

## Grupy ról AD

```text
GG_HOSP_ROLE_Doctor
GG_HOSP_ROLE_Nurse
GG_HOSP_ROLE_Registration
GG_HOSP_ROLE_DepartmentManager
GG_HOSP_ROLE_Auditor
GG_HOSP_ROLE_ITAdministrator
```

## Grupy oddziałów

```text
GG_HOSP_DEPT_Cardiology
GG_HOSP_DEPT_Orthopedics
GG_HOSP_DEPT_Neurology
GG_HOSP_DEPT_Emergency
GG_HOSP_DEPT_Pediatrics
```

## Grupy dostępowe SQL

```text
GG_SQL_HospitalAccessControl_Doctor
GG_SQL_HospitalAccessControl_Nurse
GG_SQL_HospitalAccessControl_Registration
GG_SQL_HospitalAccessControl_Manager
GG_SQL_HospitalAccessControl_Auditor
GG_SQL_HospitalAccessControl_Admin
GG_SQL_HospitalAccessControl_Runtime
GG_SQL_HospitalAccessControl_Migration
```

## Zasada mapowania

```text
Użytkownik
  ↓
Grupa roli biznesowej
  ↓
Grupa dostępowa SQL / aplikacyjna
  ↓
Login SQL Server dla grupy AD
  ↓
User w bazie
  ↓
Rola bazodanowa
  ↓
Uprawnienia
```

## Macierz uprawnień

| Rola | Pacjenci | Dokumentacja medyczna | Audyt | Administracja |
|---|---|---|---|---|
| Doctor | odczyt własny oddział | odczyt/dodawanie | brak | brak |
| Nurse | odczyt własny oddział | ograniczony odczyt | brak | brak |
| Registration | dane podstawowe | brak | brak | brak |
| DepartmentManager | odczyt własny oddział | odczyt własny oddział | raporty oddziałowe | brak |
| Auditor | metadane | brak lub zanonimizowane | odczyt | brak |
| ITAdministrator | brak domyślnie | brak domyślnie | technicznie ograniczony | administracja techniczna |
