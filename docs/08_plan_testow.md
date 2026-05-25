# Plan testów

## Test 1 — lekarz kardiologii

### Konto

```text
HOSPITAL\doctor.cardio
```

### Oczekiwany wynik

- widzi tylko pacjentów kardiologii,
- nie widzi pacjentów ortopedii i neurologii,
- może odczytać dokumentację medyczną,
- może dodać wpis medyczny.

## Test 2 — lekarz ortopedii

### Konto

```text
HOSPITAL\doctor.ortho
```

### Oczekiwany wynik

- widzi tylko pacjentów ortopedii,
- nie widzi pacjentów kardiologii.

## Test 3 — pielęgniarka

### Konto

```text
HOSPITAL
urse.cardio
```

### Oczekiwany wynik

- widzi pacjentów swojego oddziału,
- nie może dodać diagnozy lekarskiej.

## Test 4 — rejestracja

### Konto

```text
HOSPITALegistration.user
```

### Oczekiwany wynik

- widzi dane podstawowe pacjenta,
- nie widzi diagnozy i leczenia,
- PESEL jest zamaskowany zgodnie z założeniami.

## Test 5 — audytor

### Konto

```text
HOSPITALuditor.user
```

### Oczekiwany wynik

- widzi panel audytu,
- nie edytuje danych medycznych.

## Test 6 — administrator IT

### Konto

```text
HOSPITAL\it.admin
```

### Oczekiwany wynik

- brak domyślnego dostępu biznesowego do danych pacjentów.

## Test 7 — bezpośrednie zapytanie do SQL Server

### Cel

Potwierdzenie, że RLS działa również poza aplikacją.

### Oczekiwany wynik

Użytkownik wykonujący zapytanie bezpośrednio do SQL Server widzi tylko dane zgodne z przypisaniem do oddziału.
