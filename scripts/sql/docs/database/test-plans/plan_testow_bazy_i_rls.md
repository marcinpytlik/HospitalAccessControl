# Plan testów bazy danych i RLS

## Test 1 — lekarz kardiologii

### Konto

```text
HOSPITAL\doctor.cardio
```

### Oczekiwany wynik

- widzi tylko pacjentów z Kardiologii,
- nie widzi pacjentów z Ortopedii,
- nie widzi pacjentów z Neurologii,
- nie widzi pacjentów z Pediatrii.

---

## Test 2 — lekarz ortopedii

### Konto

```text
HOSPITAL\doctor.ortho
```

### Oczekiwany wynik

- widzi tylko pacjentów z Ortopedii,
- nie widzi pacjentów z Kardiologii.

---

## Test 3 — pielęgniarka kardiologii

### Konto

```text
HOSPITAL\nurse.cardio
```

### Oczekiwany wynik

- widzi pacjentów z Kardiologii,
- ma ograniczony dostęp do dokumentacji medycznej.

---

## Test 4 — rejestracja

### Konto

```text
HOSPITAL\registration.user
```

### Oczekiwany wynik

- widzi tylko zakres danych podstawowych,
- nie widzi diagnozy i leczenia,
- PESEL może być zamaskowany.

---

## Test 5 — audytor

### Konto

```text
HOSPITAL\auditor.user
```

### Oczekiwany wynik

- ma dostęp do danych audytowych,
- nie edytuje danych pacjentów.

---

## Test 6 — administrator IT

### Konto

```text
HOSPITAL\it.admin
```

### Oczekiwany wynik

- brak domyślnego dostępu biznesowego do danych pacjentów.

---

## Test 7 — bezpośrednie zapytanie do SQL Server

### Cel

Sprawdzenie, czy RLS działa również poza aplikacją.

### Oczekiwany wynik

Użytkownik wykonujący zapytanie bezpośrednio w SQL Server widzi tylko rekordy zgodne z przypisaniem do oddziału.
