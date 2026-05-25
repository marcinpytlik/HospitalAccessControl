# Projekt tabel — HospitalAccessControlDb

## 1. Tabele słownikowe

## `dictionary.Departments`

Tabela przechowuje oddziały szpitalne.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `DepartmentId` | int | Klucz główny |
| `Code` | string | Kod oddziału |
| `Name` | string | Nazwa oddziału |
| `Description` | string | Opis oddziału |
| `IsActive` | bool | Czy oddział aktywny |
| `CreatedAt` | datetime | Data utworzenia |

Przykładowe dane:

| Code | Name |
|---|---|
| `CARD` | Kardiologia |
| `ORTH` | Ortopedia |
| `NEUR` | Neurologia |
| `EMER` | Izba Przyjęć |
| `PED` | Pediatria |

Znaczenie dla RLS:

> `DepartmentId` jest kluczową kolumną używaną do filtrowania danych pacjentów.

---

## `dictionary.ApplicationRoles`

Tabela przechowuje role aplikacyjne.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `ApplicationRoleId` | int | Klucz główny |
| `Code` | string | Kod roli |
| `Name` | string | Nazwa roli |
| `Description` | string | Opis roli |
| `IsActive` | bool | Czy rola aktywna |
| `CreatedAt` | datetime | Data utworzenia |

Przykładowe dane:

| Code | Name |
|---|---|
| `Doctor` | Lekarz |
| `Nurse` | Pielęgniarka |
| `Registration` | Rejestracja |
| `DepartmentManager` | Kierownik oddziału |
| `Auditor` | Audytor |
| `ITAdministrator` | Administrator IT |

---

## `dictionary.PatientStatuses`

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `PatientStatusId` | int | Klucz główny |
| `Code` | string | Kod statusu |
| `Name` | string | Nazwa statusu |
| `IsActive` | bool | Czy aktywny |

Przykładowe dane:

| Code | Name |
|---|---|
| `ACTIVE` | Aktywny |
| `DISCHARGED` | Wypisany |
| `TRANSFERRED` | Przeniesiony |
| `ARCHIVED` | Archiwalny |

---

## `dictionary.MedicalRecordTypes`

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `MedicalRecordTypeId` | int | Klucz główny |
| `Code` | string | Kod typu |
| `Name` | string | Nazwa typu |
| `Description` | string | Opis |
| `IsActive` | bool | Czy aktywny |

Przykładowe dane:

| Code | Name |
|---|---|
| `DIAGNOSIS` | Diagnoza |
| `TREATMENT` | Leczenie |
| `NURSE_NOTE` | Notatka pielęgniarska |
| `DISCHARGE` | Wypis |
| `OBSERVATION` | Obserwacja |

---

## `dictionary.Genders`

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `GenderId` | int | Klucz główny |
| `Code` | string | Kod |
| `Name` | string | Nazwa |

Przykładowe dane:

| Code | Name |
|---|---|
| `F` | Kobieta |
| `M` | Mężczyzna |
| `U` | Nieokreślono |

---

## `dictionary.AccessActionTypes`

Typy akcji audytowych.

| Code | Name |
|---|---|
| `ViewPatientList` | Wyświetlenie listy pacjentów |
| `ViewPatientDetails` | Wyświetlenie szczegółów pacjenta |
| `ViewMedicalRecord` | Wyświetlenie dokumentacji |
| `CreateMedicalRecord` | Dodanie wpisu medycznego |
| `AccessDenied` | Odmowa dostępu |
| `ExportData` | Eksport danych |

---

## `dictionary.SecurityEventTypes`

Typy zdarzeń bezpieczeństwa.

| Code | Name |
|---|---|
| `UnauthorizedAccessAttempt` | Próba nieuprawnionego dostępu |
| `RoleMismatch` | Niezgodność roli |
| `DepartmentAccessDenied` | Brak dostępu do oddziału |
| `AuditViewOpened` | Otwarcie panelu audytu |
| `PermissionChanged` | Zmiana uprawnień |

---

# 2. Tabele medyczne

## `medical.Patients`

Główna tabela pacjentów.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `PatientId` | int | Klucz główny |
| `MedicalNumber` | string | Numer pacjenta w systemie |
| `FirstName` | string | Imię |
| `LastName` | string | Nazwisko |
| `Pesel` | string | PESEL |
| `DateOfBirth` | date | Data urodzenia |
| `GenderId` | int | FK do `dictionary.Genders` |
| `DepartmentId` | int | FK do `dictionary.Departments` |
| `PatientStatusId` | int | FK do `dictionary.PatientStatuses` |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Login użytkownika tworzącego |
| `UpdatedAt` | datetime nullable | Data modyfikacji |
| `UpdatedBy` | string nullable | Login użytkownika modyfikującego |
| `IsDeleted` | bool | Flaga soft delete |

Klucze:

```text
PK: PatientId
FK: DepartmentId -> dictionary.Departments
FK: GenderId -> dictionary.Genders
FK: PatientStatusId -> dictionary.PatientStatuses
```

Indeksy:

```text
IX_Patients_DepartmentId
IX_Patients_MedicalNumber
IX_Patients_LastName_FirstName
IX_Patients_Pesel
IX_Patients_Status
```

---

## `medical.PatientAdmissions`

Tabela pobytów i przyjęć pacjenta.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `PatientAdmissionId` | int | Klucz główny |
| `PatientId` | int | FK do pacjenta |
| `DepartmentId` | int | FK do oddziału |
| `AdmissionDate` | datetime | Data przyjęcia |
| `DischargeDate` | datetime nullable | Data wypisu |
| `AdmissionReason` | string | Powód przyjęcia |
| `Status` | string | Status pobytu |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Użytkownik tworzący |

---

## `medical.MedicalRecords`

Tabela dokumentacji medycznej.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `MedicalRecordId` | int | Klucz główny |
| `PatientId` | int | FK do pacjenta |
| `DepartmentId` | int | FK do oddziału |
| `MedicalRecordTypeId` | int | FK do typu wpisu |
| `Title` | string | Tytuł wpisu |
| `Description` | string | Opis |
| `Diagnosis` | string nullable | Diagnoza |
| `Treatment` | string nullable | Leczenie |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Autor wpisu |
| `UpdatedAt` | datetime nullable | Data modyfikacji |
| `UpdatedBy` | string nullable | Autor modyfikacji |
| `IsDeleted` | bool | Flaga soft delete |

Uwaga:

> `DepartmentId` znajduje się w tabeli celowo, żeby uprościć politykę Row-Level Security.

---

## `medical.PatientNotes`

Tabela notatek pomocniczych.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `PatientNoteId` | int | Klucz główny |
| `PatientId` | int | FK do pacjenta |
| `DepartmentId` | int | FK do oddziału |
| `NoteText` | string | Treść notatki |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Autor |
| `IsDeleted` | bool | Flaga soft delete |

---

# 3. Tabele bezpieczeństwa

## `security.ApplicationUsers`

Tabela mapująca użytkowników domenowych na użytkowników aplikacyjnych.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `ApplicationUserId` | int | Klucz główny |
| `DomainLogin` | string | Login domenowy, np. `HOSPITAL\doctor.cardio` |
| `SamAccountName` | string | Nazwa konta, np. `doctor.cardio` |
| `DisplayName` | string | Nazwa wyświetlana |
| `Email` | string nullable | E-mail |
| `IsActive` | bool | Czy konto aktywne |
| `CreatedAt` | datetime | Data utworzenia |

Unikalność:

```text
UQ_ApplicationUsers_DomainLogin
UQ_ApplicationUsers_SamAccountName
```

---

## `security.UserDepartmentAccess`

Kluczowa tabela dla RLS.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `UserDepartmentAccessId` | int | Klucz główny |
| `ApplicationUserId` | int | FK do użytkownika |
| `DepartmentId` | int | FK do oddziału |
| `ValidFrom` | datetime | Od kiedy dostęp jest aktywny |
| `ValidTo` | datetime nullable | Do kiedy dostęp jest aktywny |
| `IsActive` | bool | Czy przypisanie aktywne |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Kto utworzył przypisanie |

Dostęp jest aktywny, gdy:

```text
IsActive = 1
ValidFrom <= aktualna data
ValidTo IS NULL albo ValidTo >= aktualna data
```

---

## `security.UserRoleAssignments`

Tabela przypisania użytkowników do ról aplikacyjnych.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `UserRoleAssignmentId` | int | Klucz główny |
| `ApplicationUserId` | int | FK do użytkownika |
| `ApplicationRoleId` | int | FK do roli |
| `ValidFrom` | datetime | Od kiedy rola aktywna |
| `ValidTo` | datetime nullable | Do kiedy rola aktywna |
| `IsActive` | bool | Czy przypisanie aktywne |
| `CreatedAt` | datetime | Data utworzenia |
| `CreatedBy` | string | Kto utworzył |

---

# 4. Tabele audytu

## `audit.AccessLog`

Tabela audytu aplikacyjnego.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `AccessLogId` | bigint | Klucz główny |
| `DomainLogin` | string | Login domenowy użytkownika |
| `PatientId` | int nullable | Pacjent, którego dotyczył dostęp |
| `ActionTypeId` | int | FK do `dictionary.AccessActionTypes` |
| `ObjectName` | string | Nazwa obiektu |
| `AccessDate` | datetime | Data dostępu |
| `ClientHost` | string nullable | Nazwa hosta |
| `ApplicationName` | string nullable | Nazwa aplikacji |
| `WasSuccessful` | bool | Czy operacja udana |
| `AdditionalInfo` | string nullable | Dodatkowe informacje |

---

## `audit.SecurityEvents`

Tabela zdarzeń bezpieczeństwa.

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `SecurityEventId` | bigint | Klucz główny |
| `DomainLogin` | string | Login użytkownika |
| `SecurityEventTypeId` | int | FK do typu zdarzenia |
| `EventDate` | datetime | Data zdarzenia |
| `Severity` | string | Poziom ważności |
| `Description` | string | Opis zdarzenia |
| `ClientHost` | string nullable | Host |
| `ApplicationName` | string nullable | Aplikacja |

---

# 5. Tabele aplikacyjne

## `app.ApplicationSettings`

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `ApplicationSettingId` | int | Klucz główny |
| `SettingKey` | string | Klucz ustawienia |
| `SettingValue` | string | Wartość |
| `Description` | string nullable | Opis |
| `UpdatedAt` | datetime nullable | Data aktualizacji |

## `app.SchemaVersions`

| Kolumna | Typ logiczny | Opis |
|---|---|---|
| `SchemaVersionId` | int | Klucz główny |
| `VersionNumber` | string | Numer wersji |
| `AppliedAt` | datetime | Data wdrożenia |
| `Description` | string | Opis zmiany |
