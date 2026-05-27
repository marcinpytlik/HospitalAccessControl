# DAY024 — HospitalAccessControl
    ## Scenariusz demo i checklista końcowa

    ## Cel dnia

    Przygotowanie końcowego scenariusza demonstracji projektu i checklisty przed obroną.

    Ten etap kontynuuje styl warsztatowy z poprzednich dni: najpierw robimy jedną konkretną zmianę, potem build, test i dopiero kolejny krok.

    ---

    ## Efekt końcowy DAY024

    Po zakończeniu tego dnia projekt będzie mieć:

    ```text
    Scenariusz demo i checklista końcowa
    build bez błędów
    testy ręczne lub automatyczne dla nowego zakresu
    artefakty SQL/kodu zapisane w repo
    ```

    ---

    ## Założenia

    Projekt jest po DAY012 i ma już:

    ```text
    CurrentUserService
    SESSION_CONTEXT
    Row-Level Security
    lista pacjentów
    szczegóły pacjenta
    audyt dostępu
    ```

    ---

    # Krok 0 — przejście do katalogu projektu

    ```powershell
    Set-Location C:\Projects\HospitalAccessControl
    dotnet build
    ```

    Oczekiwany wynik:

    ```text
    Build succeeded.
    ```

    ---

    # Krok 1 — Opisać scenariusze dla `doctor.cardio`, `doctor.ortho`, `it.admin`, `auditor.user`.

Opis wykonania:

```text
Opisać scenariusze dla `doctor.cardio`, `doctor.ortho`, `it.admin`, `auditor.user`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 2 — Przygotować zapytania SQL do pokazania RLS, DDM i audytu.

Opis wykonania:

```text
Przygotować zapytania SQL do pokazania RLS, DDM i audytu.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 3 — Przygotować listę screenshotów.

Opis wykonania:

```text
Przygotować listę screenshotów.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 4 — Przygotować checklistę techniczną.

Opis wykonania:

```text
Przygotować checklistę techniczną.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 5 — Zebrać artefakty testowe.

Opis wykonania:

```text
Zebrać artefakty testowe.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.


    ---

    # Fragmenty kodu / SQL

    ## RLS demo

```csharp
EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio';
SELECT DepartmentId, COUNT(*) FROM medical.Patients GROUP BY DepartmentId;
```

## audit demo

```csharp
SELECT TOP (20) AccessLogId, DomainLogin, RequestedPatientId, WasSuccessful FROM audit.AccessLog ORDER BY AccessLogId DESC;
```

## screenshots

```csharp
strona główna
/Patients
/Patients/Details/1
/Audit
SQL Server Audit
DDM
```


    ---

    # Komendy

    ```powershell
    dotnet build
dotnet test
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "SELECT name, is_enabled FROM sys.security_policies;"
    ```

    ---

    # Testy ręczne

    - Demo przechodzi bez błędów.
- Każdy użytkownik pokazuje inny zakres danych.
- Nieudane próby są widoczne w audycie.

    ---

    # Kontrola końcowa DAY024

    ```text
    [ ] Opisać scenariusze dla doctor.cardio, doctor.ortho, it.admin, auditor.user.
[ ] Przygotować zapytania SQL do pokazania RLS, DDM i audytu.
[ ] Przygotować listę screenshotów.
[ ] Przygotować checklistę techniczną.
[ ] Zebrać artefakty testowe.
[ ] dotnet build kończy się sukcesem
[ ] zmiany są zapisane w Git
    ```

    ---

    # Najczęstsze problemy

    ## Problem 1 — build nie przechodzi

    Sprawdź namespace, usingi oraz czy projekt Infrastructure ma referencję do Application i Domain.

    ## Problem 2 — DI nie znajduje serwisu

    Sprawdź, czy dodałeś rejestrację w:

    ```text
    ServiceCollectionExtensions.cs
    ```

    ## Problem 3 — wynik w aplikacji różni się od SQL

    Sprawdź, czy działa `SESSION_CONTEXT`, czy RLS jest włączony i czy aplikacja łączy się z właściwą bazą.

    ---

    # Commit Git

    ```powershell
    git status
    git add .
    git commit -m "DAY024 Scenariusz demo i checklista końcowa"
    ```

    ---

    # Krótkie podsumowanie dla dokumentacji pracy

    W ramach DAY024 rozszerzono projekt HospitalAccessControl o zakres: **Scenariusz demo i checklista końcowa**. Zmiana wzmacnia demonstracyjny model kontroli dostępu do danych medycznych oparty o Active Directory, role aplikacyjne, SESSION_CONTEXT, Row-Level Security, Dynamic Data Masking oraz audyt.

    Następny etap:

    ```text
    DAY025 — Dokumentacja techniczna końcowa
    ```
