# Harmonogram prac — HospitalAccessControl

## Temat projektu

**Projekt i implementacja modelu kontroli dostępu do danych medycznych z wykorzystaniem Active Directory, RBAC oraz Row-Level Security w Microsoft SQL Server**

## Założenie ogólne

Projekt zostanie zrealizowany etapowo. Najpierw przygotowane zostanie środowisko laboratoryjne, następnie usługi domenowe Active Directory, model użytkowników i grup, środowisko SQL Server, baza danych, mechanizmy bezpieczeństwa oraz aplikacja demonstracyjna w technologii .NET 10. Ostatnim etapem będą testy działania, analiza bezpieczeństwa oraz przygotowanie dokumentacji końcowej.

---

## Etap 1 — Analiza wymagań i zakresu pracy

### Cel etapu

Określenie, jaki problem rozwiązuje projekt oraz jakie mechanizmy bezpieczeństwa mają zostać wykorzystane.

### Zakres prac

1. Określenie celu pracy.
2. Zdefiniowanie zakresu funkcjonalnego systemu.
3. Określenie ról użytkowników.
4. Określenie oddziałów szpitalnych.
5. Wybór technologii.
6. Przygotowanie założeń bezpieczeństwa.

### Wynik etapu

- opis celu pracy,
- opis zakresu projektu,
- lista ról użytkowników,
- lista oddziałów,
- wybór stacku technologicznego.

### Planowany czas

**1 tydzień**

---

## Etap 2 — Projekt architektury środowiska laboratoryjnego

### Cel etapu

Zaprojektowanie środowiska serwerowego, w którym będzie realizowany projekt.

### Zakres prac

1. Przygotowanie listy maszyn wirtualnych.
2. Zaprojektowanie adresacji IP.
3. Określenie ról serwerów.
4. Zaprojektowanie domeny Active Directory.
5. Przygotowanie schematu komunikacji między maszynami.

### Maszyny laboratoryjne

| Maszyna | Adres IP | Przeznaczenie |
|---|---:|---|
| `DC01` | `192.168.50.10` | Active Directory Domain Services + DNS |
| `SQL01` | `192.168.50.20` | SQL Server 2022 Developer Edition |
| `APP01` | `192.168.50.30` | IIS + .NET 10 + aplikacja |
| `CLIENT01` | `192.168.50.40` | Windows 11, klient testowy |

### Wynik etapu

- diagram środowiska,
- tabela maszyn,
- tabela adresacji IP,
- opis przeznaczenia każdej maszyny.

### Planowany czas

**1 tydzień**

---

## Etap 3 — Przygotowanie Active Directory

### Cel etapu

Instalacja i konfiguracja domeny Active Directory dla środowiska laboratoryjnego.

### Zakres prac

1. Instalacja roli Active Directory Domain Services.
2. Utworzenie domeny `hospital.local`.
3. Instalacja i konfiguracja DNS.
4. Utworzenie struktury OU.
5. Utworzenie kont usługowych.
6. Utworzenie grup bezpieczeństwa.
7. Utworzenie użytkowników testowych.
8. Dołączenie serwerów i klienta do domeny.

### Wynik etapu

- działająca domena `hospital.local`,
- utworzona struktura OU,
- utworzone konta usługowe,
- utworzone grupy ról i oddziałów,
- utworzeni użytkownicy testowi,
- `SQL01`, `APP01` i `CLIENT01` dołączone do domeny.

### Planowany czas

**1–2 tygodnie**

---

## Etap 4 — Projekt modelu RBAC

### Cel etapu

Zaprojektowanie modelu kontroli dostępu opartego na rolach użytkowników i przypisaniu do oddziałów.

### Zakres prac

1. Zdefiniowanie ról biznesowych.
2. Zdefiniowanie grup oddziałowych.
3. Zdefiniowanie grup dostępowych do SQL Server.
4. Zdefiniowanie grup dostępowych do aplikacji.
5. Przygotowanie zasad zagnieżdżania grup.
6. Przygotowanie macierzy uprawnień.

### Role użytkowników

- `Doctor`
- `Nurse`
- `Registration`
- `DepartmentManager`
- `Auditor`
- `ITAdministrator`

### Oddziały

- `Cardiology`
- `Orthopedics`
- `Neurology`
- `Emergency`
- `Pediatrics`

### Wynik etapu

- model RBAC,
- macierz uprawnień,
- opis grup AD,
- opis zagnieżdżeń grup,
- opis mapowania ról na dostęp do danych.

### Planowany czas

**1 tydzień**

---

## Etap 5 — Przygotowanie SQL Server

### Cel etapu

Instalacja i konfiguracja serwera SQL Server 2022 Developer Edition.

### Zakres prac

1. Instalacja SQL Server 2022 Developer Edition na `SQL01`.
2. Konfiguracja usług SQL Server.
3. Ustawienie kont usługowych.
4. Konfiguracja podstawowych ustawień instancji.
5. Utworzenie bazy `HospitalAccessControlDb`.
6. Utworzenie loginów dla grup Active Directory.
7. Utworzenie użytkowników bazodanowych.
8. Utworzenie ról bazodanowych.

### Wynik etapu

- działający SQL Server 2022,
- utworzona baza `HospitalAccessControlDb`,
- utworzone loginy dla grup AD,
- utworzone role bazodanowe,
- przygotowany fundament pod RLS i audyt.

### Planowany czas

**1 tydzień**

---

## Etap 6 — Projekt i implementacja bazy danych

### Cel etapu

Utworzenie struktury bazy danych dla demonstracyjnego systemu szpitalnego.

### Zakres prac

1. Utworzenie schematów SQL.
2. Utworzenie tabel słownikowych.
3. Utworzenie tabel medycznych.
4. Utworzenie tabel bezpieczeństwa.
5. Utworzenie tabel audytowych.
6. Przygotowanie relacji między tabelami.
7. Przygotowanie danych testowych.

### Główne schematy

- `dictionary`
- `medical`
- `security`
- `audit`
- `app`

### Główne tabele

- `dictionary.Departments`
- `dictionary.Roles`
- `medical.Patients`
- `medical.PatientAdmissions`
- `medical.MedicalRecords`
- `medical.PatientNotes`
- `security.ApplicationUsers`
- `security.UserDepartmentAccess`
- `security.UserRoleAssignments`
- `audit.AccessLog`
- `audit.SecurityEvents`

### Wynik etapu

- gotowy model bazy danych,
- utworzone relacje,
- wprowadzone dane testowe,
- przygotowana baza do testów RLS.

### Planowany czas

**1–2 tygodnie**

---

## Etap 7 — Implementacja mechanizmów bezpieczeństwa SQL Server

### Cel etapu

Wdrożenie mechanizmów zabezpieczających dostęp do danych pacjentów.

### Zakres prac

1. Implementacja Row-Level Security.
2. Implementacja funkcji filtrującej dane po oddziale.
3. Implementacja `SECURITY POLICY`.
4. Implementacja ról bazodanowych.
5. Nadanie minimalnych uprawnień.
6. Wdrożenie Dynamic Data Masking.
7. Wdrożenie SQL Server Audit.
8. Opcjonalnie wdrożenie TDE.

### Mechanizmy bezpieczeństwa

- Windows Authentication
- RBAC
- Database roles
- Row-Level Security
- Dynamic Data Masking
- SQL Server Audit
- TDE

### Wynik etapu

- dane pacjentów filtrowane według oddziału,
- dostęp ograniczony według roli,
- dane wrażliwe częściowo maskowane,
- dostęp do danych rejestrowany w audycie.

### Planowany czas

**1–2 tygodnie**

---

## Etap 8 — Projekt aplikacji .NET 10

### Cel etapu

Zaprojektowanie aplikacji demonstracyjnej pokazującej działanie kontroli dostępu.

### Zakres prac

1. Przygotowanie struktury solution.
2. Utworzenie projektów warstwowych.
3. Zdefiniowanie encji domenowych.
4. Zdefiniowanie DTO.
5. Zdefiniowanie usług aplikacyjnych.
6. Zdefiniowanie interfejsów.
7. Zaprojektowanie widoków Razor Pages.

### Struktura solution

- `HospitalAccessControl.Domain`
- `HospitalAccessControl.Application`
- `HospitalAccessControl.Infrastructure`
- `HospitalAccessControl.Web`
- `HospitalAccessControl.Tests`

### Wynik etapu

- opis architektury aplikacji,
- lista projektów,
- lista klas,
- lista encji,
- lista usług,
- projekt interfejsu użytkownika.

### Planowany czas

**1 tydzień**

---

## Etap 9 — Implementacja aplikacji .NET 10

### Cel etapu

Zbudowanie aplikacji demonstracyjnej działającej z Windows Authentication i SQL Server.

### Zakres prac

1. Utworzenie solution .NET 10.
2. Implementacja encji domenowych.
3. Implementacja `DbContext`.
4. Implementacja konfiguracji EF Core.
5. Implementacja usług aplikacyjnych.
6. Implementacja Razor Pages.
7. Konfiguracja Windows Authentication.
8. Konfiguracja połączenia z SQL Server.
9. Publikacja aplikacji na `APP01`.

### Główne funkcje aplikacji

- lista pacjentów,
- szczegóły pacjenta,
- lista wpisów medycznych,
- dodanie wpisu medycznego przez lekarza,
- panel audytu,
- strona odmowy dostępu.

### Wynik etapu

- działająca aplikacja `HospitalAccessControl.Web`,
- aplikacja opublikowana na `APP01`,
- użytkownicy domenowi mogą testować dostęp z `CLIENT01`.

### Planowany czas

**2–3 tygodnie**

---

## Etap 10 — Testy funkcjonalne i bezpieczeństwa

### Cel etapu

Sprawdzenie, czy mechanizmy RBAC i RLS działają zgodnie z założeniami.

### Zakres prac

1. Test lekarza z kardiologii.
2. Test lekarza z ortopedii.
3. Test pielęgniarki.
4. Test rejestracji.
5. Test audytora.
6. Test administratora IT.
7. Test próby dostępu do danych innego oddziału.
8. Test działania Dynamic Data Masking.
9. Test działania audytu.
10. Test bezpośredniego zapytania do SQL Server poza aplikacją.

### Przykładowe scenariusze

- `doctor.cardio` widzi tylko pacjentów kardiologii.
- `doctor.ortho` widzi tylko pacjentów ortopedii.
- `registration.user` nie widzi diagnozy i leczenia.
- `auditor.user` widzi logi audytowe.
- `it.admin` nie ma domyślnego dostępu biznesowego do danych pacjentów.

### Wynik etapu

- tabela testów,
- wyniki testów,
- zrzuty ekranu,
- opis wykrytych ograniczeń,
- potwierdzenie działania RLS.

### Planowany czas

**1–2 tygodnie**

---

## Etap 11 — Analiza wyników i ograniczeń rozwiązania

### Cel etapu

Ocena skuteczności zastosowanych mechanizmów bezpieczeństwa.

### Zakres prac

1. Analiza działania RBAC.
2. Analiza działania RLS.
3. Analiza zalet modelu opartego o AD.
4. Analiza ograniczeń RLS.
5. Analiza ryzyk administracyjnych.
6. Analiza wpływu modelu na utrzymanie systemu.
7. Wskazanie możliwych rozszerzeń.

### Wynik etapu

- rozdział analityczny pracy,
- opis zalet,
- opis ograniczeń,
- opis potencjalnych usprawnień.

### Planowany czas

**1 tydzień**

---

## Etap 12 — Dokumentacja końcowa pracy

### Cel etapu

Przygotowanie kompletnej dokumentacji technicznej oraz treści pracy inżynierskiej.

### Zakres prac

1. Opis problemu.
2. Opis wykorzystanych technologii.
3. Opis architektury.
4. Opis środowiska laboratoryjnego.
5. Opis Active Directory.
6. Opis modelu RBAC.
7. Opis bazy danych.
8. Opis mechanizmów SQL Server.
9. Opis aplikacji .NET 10.
10. Opis testów.
11. Wnioski końcowe.

### Wynik etapu

- gotowa praca inżynierska,
- załączniki techniczne,
- skrypty PowerShell,
- skrypty T-SQL,
- diagramy,
- zrzuty ekranu,
- wyniki testów.

### Planowany czas

**2–3 tygodnie**

---

# Harmonogram zbiorczy

| Etap | Nazwa etapu | Czas |
|---:|---|---:|
| 1 | Analiza wymagań i zakresu pracy | 1 tydzień |
| 2 | Projekt architektury środowiska laboratoryjnego | 1 tydzień |
| 3 | Przygotowanie Active Directory | 1–2 tygodnie |
| 4 | Projekt modelu RBAC | 1 tydzień |
| 5 | Przygotowanie SQL Server | 1 tydzień |
| 6 | Projekt i implementacja bazy danych | 1–2 tygodnie |
| 7 | Implementacja mechanizmów bezpieczeństwa SQL Server | 1–2 tygodnie |
| 8 | Projekt aplikacji .NET 10 | 1 tydzień |
| 9 | Implementacja aplikacji .NET 10 | 2–3 tygodnie |
| 10 | Testy funkcjonalne i bezpieczeństwa | 1–2 tygodnie |
| 11 | Analiza wyników i ograniczeń | 1 tydzień |
| 12 | Dokumentacja końcowa | 2–3 tygodnie |

## Łączny czas realizacji

**Około 14–20 tygodni**, czyli realnie **4–5 miesięcy spokojnej pracy**.

---

# Kamienie milowe

| Kamień milowy | Opis | Efekt |
|---|---|---|
| M1 | Gotowy projekt środowiska | Diagram, lista maszyn, adresacja |
| M2 | Działająca domena AD | `hospital.local`, OU, grupy, użytkownicy |
| M3 | Działający SQL Server | `SQL01`, baza, loginy, role |
| M4 | Gotowy model danych | Tabele, relacje, dane testowe |
| M5 | Działający RLS | Filtrowanie pacjentów po oddziale |
| M6 | Działająca aplikacja | Razor Pages, Windows Authentication |
| M7 | Zakończone testy | Tabela testów i wyniki |
| M8 | Gotowa dokumentacja | Praca, skrypty, diagramy |
| M9 | Gotowa demonstracja | Scenariusz pokazowy na obronę |
