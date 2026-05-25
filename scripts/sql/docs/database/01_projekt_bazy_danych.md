# Projekt bazy danych — HospitalAccessControlDb

## 1. Nazwa bazy danych

```text
HospitalAccessControlDb
```

## 2. Cel bazy danych

Baza danych ma przechowywać dane demonstracyjnego systemu szpitalnego oraz umożliwiać testowanie mechanizmów bezpieczeństwa:

- dostępu do danych pacjentów według oddziałów,
- kontroli dostępu według ról użytkowników,
- Row-Level Security,
- Dynamic Data Masking,
- SQL Server Audit,
- audytu aplikacyjnego,
- mapowania użytkowników domenowych na role i oddziały.

Główne założenie:

> Użytkownik widzi tylko te dane pacjentów, do których ma dostęp na podstawie przypisania do oddziału oraz roli biznesowej.

---

## 3. Schematy SQL

Proponowane schematy:

| Schemat | Przeznaczenie |
|---|---|
| `dictionary` | Dane słownikowe i referencyjne |
| `medical` | Dane pacjentów i dokumentacja medyczna |
| `security` | Mapowanie użytkowników, ról i oddziałów |
| `audit` | Audyt aplikacyjny i zdarzenia bezpieczeństwa |
| `app` | Ustawienia aplikacyjne i wersje schematu |

---

## 4. Lista tabel

### Schemat `dictionary`

```text
dictionary.Departments
dictionary.ApplicationRoles
dictionary.PatientStatuses
dictionary.MedicalRecordTypes
dictionary.Genders
dictionary.AccessActionTypes
dictionary.SecurityEventTypes
```

### Schemat `medical`

```text
medical.Patients
medical.PatientAdmissions
medical.MedicalRecords
medical.PatientNotes
```

### Schemat `security`

```text
security.ApplicationUsers
security.UserDepartmentAccess
security.UserRoleAssignments
```

### Schemat `audit`

```text
audit.AccessLog
audit.SecurityEvents
```

### Schemat `app`

```text
app.ApplicationSettings
app.SchemaVersions
```

---

## 5. Najważniejsze decyzje projektowe

### Decyzja 1 — `DepartmentId` w tabelach medycznych

Kolumna `DepartmentId` znajduje się nie tylko w tabeli `medical.Patients`, ale także w:

- `medical.PatientAdmissions`,
- `medical.MedicalRecords`,
- `medical.PatientNotes`.

Dzięki temu polityki Row-Level Security są prostsze, czytelniejsze i łatwiejsze do testowania.

### Decyzja 2 — użytkownicy w bazie nie zastępują Active Directory

Tabela `security.ApplicationUsers` nie służy do logowania. Logowanie odbywa się przez:

```text
Active Directory + Windows Authentication
```

Tabela `security.ApplicationUsers` służy do mapowania:

```text
login domenowy -> role -> oddziały -> Row-Level Security
```

### Decyzja 3 — administrator IT nie widzi automatycznie danych medycznych

Administrator techniczny może zarządzać infrastrukturą, ale nie powinien automatycznie mieć dostępu biznesowego do danych pacjentów.

### Decyzja 4 — RLS w bazie, nie tylko filtrowanie w aplikacji

Aplikacja może filtrować widoki, ale główne ograniczenie danych jest wymuszane przez SQL Server. Dzięki temu także zapytania wykonywane poza aplikacją podlegają ograniczeniom RLS.

---

## 6. Minimalna wersja bazy do pierwszej implementacji

Minimalny działający model:

```text
dictionary.Departments
dictionary.ApplicationRoles
medical.Patients
medical.MedicalRecords
security.ApplicationUsers
security.UserDepartmentAccess
security.UserRoleAssignments
audit.AccessLog
```

Ten zestaw pozwala pokazać:

- RBAC,
- RLS,
- pacjentów według oddziału,
- role użytkowników,
- audyt dostępu.

---

## 7. Docelowy efekt

Po wdrożeniu projektu:

```text
HOSPITAL\doctor.cardio
```

widzi tylko pacjentów z Kardiologii, a:

```text
HOSPITAL\doctor.ortho
```

widzi tylko pacjentów z Ortopedii.

Użytkownik:

```text
HOSPITAL\it.admin
```

nie ma domyślnego dostępu biznesowego do danych medycznych.
