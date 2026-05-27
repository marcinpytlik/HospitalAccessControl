# DAY024 — HospitalAccessControl  
## Scenariusz demo i checklista końcowa

## Cel dnia

Celem DAY024 jest: **pełny scenariusz demonstracji projektu**.

Ten etap domyka ważny fragment projektu HospitalAccessControl i powinien być wykonany dopiero po przejściu DAY000–DAY012 oraz po poprawkach DAY013–DAY018.

---

## Efekt końcowy DAY024

Po zakończeniu tego dnia powinieneś mieć:

```text
działający zakres: pełny scenariusz demonstracji projektu
komendy kontrolne
testy ręczne
artefakty do dokumentacji
commit Git
```

---

# Krok 1 — przejście do katalogu projektu

```powershell
Set-Location C:\Projects\HospitalAccessControl
dotnet build
```

---

# Krok 2 — weryfikacja punktu startowego

Sprawdź działanie aplikacji:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Sprawdź strony:

```text
/
 /Patients
 /Patients/Details/1
 /Audit
```

---

# Krok 3 — wykonanie zakresu DAY024

Zakres techniczny:

```text
pełny scenariusz demonstracji projektu
```

Pracuj według warstw:

```text
Application -> DTO/interfejsy
Infrastructure -> implementacje/SQL/EF
Web -> Razor Pages/UI
SQL -> skrypty i testy
docs -> notatka dokumentacyjna
```

---

# Krok 4 — pliki do utworzenia

Utwórz katalog dokumentacyjny dnia:

```powershell
New-Item -ItemType Directory -Force .\docs\day024
New-Item -ItemType File -Force .\docs\day024\README.md
```

Wpisz do README:

```markdown
# DAY024

## Zakres
pełny scenariusz demonstracji projektu

## Zmienione elementy
- Application
- Infrastructure
- Web
- SQL / konfiguracja

## Testy
- dotnet build
- test ręczny
- test SQL
```

---

# Krok 5 — testy SQL

Użyj podstawowych zapytań kontrolnych:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT TOP (10) AccessLogId, DomainLogin, PatientId, RequestedPatientId, WasSuccessful, AccessDate FROM audit.AccessLog ORDER BY AccessLogId DESC;"
```

Sprawdź RLS:

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT DepartmentId, COUNT(*) FROM medical.Patients GROUP BY DepartmentId;"
```

---

# Krok 6 — testy użytkowników

Testuj co najmniej:

```text
HOSPITAL\doctor.cardio
HOSPITAL\doctor.ortho
HOSPITAL\auditor.user
HOSPITAL\it.admin
HOSPITAL\unknown.user
```

---

# Krok 7 — build końcowy

```powershell
dotnet build
```

Jeżeli masz testy:

```powershell
dotnet test
```

---

# Krok 8 — commit Git

```powershell
git status
git add .
git commit -m "DAY024 Scenariusz demo i checklista końcowa"
```

---

# Kontrola końcowa DAY024

```text
[ ] dotnet build działa
[ ] aplikacja uruchamia się
[ ] zakres dnia został wykonany: pełny scenariusz demonstracji projektu
[ ] wykonano testy użytkowników
[ ] wykonano testy SQL
[ ] dokumentacja dnia istnieje
[ ] commit wykonany
```

---

# Najczęstsze problemy

## Problem 1 — aplikacja nie startuje

Uruchom:

```powershell
dotnet build
```

i popraw pierwszy błąd z listy.

## Problem 2 — RLS pokazuje złe dane

Sprawdź:

```sql
SELECT SESSION_CONTEXT(N'CurrentUser');
```

oraz:

```sql
SELECT name, is_enabled FROM sys.security_policies;
```

## Problem 3 — brak danych testowych

Wróć do DAY005 i DAY006, a potem wykonaj migracje.

---

# Efekt końcowy DAY024

Projekt jest rozszerzony o:

```text
Scenariusz demo i checklista końcowa
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach etapu DAY024 rozbudowano projekt HospitalAccessControl o obszar: Scenariusz demo i checklista końcowa. Zakres ten uzupełnia demonstrację kontroli dostępu, audytu oraz bezpieczeństwa danych medycznych w środowisku .NET i SQL Server.
