# DAY021 — HospitalAccessControl  
## Panel użytkownika: moje role i oddziały

## Cel dnia

Celem DAY021 jest: **strona /MyAccess pokazująca role oraz oddziały aktualnego użytkownika**.

Ten etap domyka ważny fragment projektu HospitalAccessControl i powinien być wykonany dopiero po przejściu DAY000–DAY012 oraz po poprawkach DAY013–DAY018.

---

## Efekt końcowy DAY021

Po zakończeniu tego dnia powinieneś mieć:

```text
działający zakres: strona /MyAccess pokazująca role oraz oddziały aktualnego użytkownika
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

# Krok 3 — wykonanie zakresu DAY021

Zakres techniczny:

```text
strona /MyAccess pokazująca role oraz oddziały aktualnego użytkownika
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
New-Item -ItemType Directory -Force .\docs\day021
New-Item -ItemType File -Force .\docs\day021\README.md
```

Wpisz do README:

```markdown
# DAY021

## Zakres
strona /MyAccess pokazująca role oraz oddziały aktualnego użytkownika

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
git commit -m "DAY021 Panel użytkownika: moje role i oddziały"
```

---

# Kontrola końcowa DAY021

```text
[ ] dotnet build działa
[ ] aplikacja uruchamia się
[ ] zakres dnia został wykonany: strona /MyAccess pokazująca role oraz oddziały aktualnego użytkownika
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

# Efekt końcowy DAY021

Projekt jest rozszerzony o:

```text
Panel użytkownika: moje role i oddziały
```

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach etapu DAY021 rozbudowano projekt HospitalAccessControl o obszar: Panel użytkownika: moje role i oddziały. Zakres ten uzupełnia demonstrację kontroli dostępu, audytu oraz bezpieczeństwa danych medycznych w środowisku .NET i SQL Server.
