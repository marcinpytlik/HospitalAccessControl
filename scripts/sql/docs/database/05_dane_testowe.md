# Dane testowe

## 1. Oddziały

| Code | Name |
|---|---|
| `CARD` | Kardiologia |
| `ORTH` | Ortopedia |
| `NEUR` | Neurologia |
| `EMER` | Izba Przyjęć |
| `PED` | Pediatria |

## 2. Role

| Code | Name |
|---|---|
| `Doctor` | Lekarz |
| `Nurse` | Pielęgniarka |
| `Registration` | Rejestracja |
| `DepartmentManager` | Kierownik oddziału |
| `Auditor` | Audytor |
| `ITAdministrator` | Administrator IT |

## 3. Użytkownicy

```text
HOSPITAL\doctor.cardio
HOSPITAL\doctor.ortho
HOSPITAL\doctor.neuro
HOSPITAL\nurse.cardio
HOSPITAL\nurse.ortho
HOSPITAL\nurse.ped
HOSPITAL\registration.user
HOSPITAL\registration.emer
HOSPITAL\manager.cardio
HOSPITAL\manager.ortho
HOSPITAL\auditor.user
HOSPITAL\it.admin
```

## 4. Pacjenci

Proponowana liczba pacjentów testowych:

| Oddział | Liczba pacjentów |
|---|---:|
| Kardiologia | 10 |
| Ortopedia | 10 |
| Neurologia | 10 |
| Izba Przyjęć | 5 |
| Pediatria | 5 |

Razem: **40 pacjentów testowych**.

## 5. Przypisania użytkowników do oddziałów

| Użytkownik | Oddział |
|---|---|
| `doctor.cardio` | Kardiologia |
| `doctor.ortho` | Ortopedia |
| `doctor.neuro` | Neurologia |
| `nurse.cardio` | Kardiologia |
| `nurse.ortho` | Ortopedia |
| `nurse.ped` | Pediatria |
| `registration.emer` | Izba Przyjęć |
| `manager.cardio` | Kardiologia |
| `manager.ortho` | Ortopedia |

## 6. Przypisania użytkowników do ról

| Użytkownik | Rola |
|---|---|
| `doctor.cardio` | Doctor |
| `doctor.ortho` | Doctor |
| `doctor.neuro` | Doctor |
| `nurse.cardio` | Nurse |
| `nurse.ortho` | Nurse |
| `nurse.ped` | Nurse |
| `registration.user` | Registration |
| `registration.emer` | Registration |
| `manager.cardio` | DepartmentManager |
| `manager.ortho` | DepartmentManager |
| `auditor.user` | Auditor |
| `it.admin` | ITAdministrator |
