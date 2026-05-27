# DAY013 — HospitalAccessControl
    ## RequestedPatientId w audycie

    ## Cel dnia

    Rozbudowa audytu aplikacyjnego tak, aby nieudane próby wejścia do pacjenta zapisywały żądany identyfikator pacjenta nawet wtedy, gdy RLS ukryje rekord.

    Ten etap kontynuuje styl warsztatowy z poprzednich dni: najpierw robimy jedną konkretną zmianę, potem build, test i dopiero kolejny krok.

    ---

    ## Efekt końcowy DAY013

    Po zakończeniu tego dnia projekt będzie mieć:

    ```text
    RequestedPatientId w audycie
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

    # Krok 1 — Dodać `RequestedPatientId` do encji `AccessLog`.

Opis wykonania:

```text
Dodać `RequestedPatientId` do encji `AccessLog`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 2 — Dodać kolumnę w konfiguracji EF Core i migrację.

Opis wykonania:

```text
Dodać kolumnę w konfiguracji EF Core i migrację.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 3 — Rozszerzyć `AccessLogCreateDto` oraz `AuditService`.

Opis wykonania:

```text
Rozszerzyć `AccessLogCreateDto` oraz `AuditService`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 4 — Zmienić `Details.cshtml.cs`, aby zapisywać `PatientId = Patient?.PatientId` oraz `RequestedPatientId = id`.

Opis wykonania:

```text
Zmienić `Details.cshtml.cs`, aby zapisywać `PatientId = Patient?.PatientId` oraz `RequestedPatientId = id`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 5 — Przetestować sukces i odmowę dostępu.

Opis wykonania:

```text
Przetestować sukces i odmowę dostępu.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.


    ---

    # Fragmenty kodu / SQL

    ## AccessLog.cs

```csharp
public int? RequestedPatientId { get; set; }
```

## AccessLogCreateDto.cs

```csharp
public int? RequestedPatientId { get; init; }
```

## Details.cshtml.cs

```csharp
PatientId = Patient?.PatientId,
RequestedPatientId = id,
```


    ---

    # Komendy

    ```powershell
    dotnet ef migrations add AddRequestedPatientIdToAccessLog `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext `
  --output-dir Data\Migrations

dotnet ef database update `
  --project .\src\HospitalAccessControl.Infrastructure `
  --startup-project .\src\HospitalAccessControl.Web `
  --context HospitalAccessControlDbContext
    ```

    ---

    # Testy ręczne

    - doctor.cardio -> /Patients/Details/1: `PatientId=1`, `RequestedPatientId=1`, `WasSuccessful=1`.
- doctor.cardio -> /Patients/Details/11: `PatientId=NULL`, `RequestedPatientId=11`, `WasSuccessful=0`.
- it.admin -> /Patients/Details/1: `PatientId=NULL`, `RequestedPatientId=1`, `WasSuccessful=0`.

    ---

    # Kontrola końcowa DAY013

    ```text
    [ ] Dodać RequestedPatientId do encji AccessLog.
[ ] Dodać kolumnę w konfiguracji EF Core i migrację.
[ ] Rozszerzyć AccessLogCreateDto oraz AuditService.
[ ] Zmienić Details.cshtml.cs, aby zapisywać PatientId = Patient?.PatientId oraz RequestedPatientId = id.
[ ] Przetestować sukces i odmowę dostępu.
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
    git commit -m "DAY013 RequestedPatientId w audycie"
    ```

    ---

    # Krótkie podsumowanie dla dokumentacji pracy

    W ramach DAY013 rozszerzono projekt HospitalAccessControl o zakres: **RequestedPatientId w audycie**. Zmiana wzmacnia demonstracyjny model kontroli dostępu do danych medycznych oparty o Active Directory, role aplikacyjne, SESSION_CONTEXT, Row-Level Security, Dynamic Data Masking oraz audyt.

    Następny etap:

    ```text
    DAY014 — Panel audytu w aplikacji
    ```
