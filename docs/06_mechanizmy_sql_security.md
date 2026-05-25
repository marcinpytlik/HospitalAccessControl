# Mechanizmy bezpieczeństwa SQL Server

## Mechanizmy

- Windows Authentication
- Loginy dla grup Active Directory
- Database roles
- Row-Level Security
- Dynamic Data Masking
- SQL Server Audit
- TDE jako rozszerzenie opcjonalne

## Row-Level Security

RLS powinno ograniczać widoczność danych pacjentów na podstawie oddziału.

Główna zasada:

> Użytkownik widzi tylko rekordy, dla których istnieje aktywne przypisanie użytkownika do oddziału.

## Dynamic Data Masking

Przykładowe pola do maskowania:

- `medical.Patients.Pesel`
- `medical.Patients.DateOfBirth`
- wybrane dane osobowe dla ról pomocniczych

## SQL Server Audit

Audyt powinien rejestrować:

- dostęp do tabel medycznych,
- wykonanie procedur,
- zmiany uprawnień,
- nieudane próby dostępu,
- użycie kont administracyjnych.

## Zasada najmniejszych uprawnień

Konto aplikacyjne `svc_hac_app` powinno mieć tylko uprawnienia wymagane do działania aplikacji.

Konto migracyjne `svc_hac_migr` powinno mieć uprawnienia do zmian schematu bazy, ale nie powinno być używane jako konto runtime.
