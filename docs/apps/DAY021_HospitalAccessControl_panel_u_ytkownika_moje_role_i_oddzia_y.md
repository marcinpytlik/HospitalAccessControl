# DAY021 — HospitalAccessControl
    ## Panel użytkownika: moje role i oddziały

    ## Cel dnia

    Dodanie strony `/MyAccess`, która pokazuje aktualnemu użytkownikowi jego role i przypisania do oddziałów.

    Ten etap kontynuuje styl warsztatowy z poprzednich dni: najpierw robimy jedną konkretną zmianę, potem build, test i dopiero kolejny krok.

    ---

    ## Efekt końcowy DAY021

    Po zakończeniu tego dnia projekt będzie mieć:

    ```text
    Panel użytkownika: moje role i oddziały
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

    # Krok 1 — Dodać `CurrentUserAccessDto`.

Opis wykonania:

```text
Dodać `CurrentUserAccessDto`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 2 — Dodać `ICurrentUserAccessReadService`.

Opis wykonania:

```text
Dodać `ICurrentUserAccessReadService`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 3 — Dodać `CurrentUserAccessReadService`.

Opis wykonania:

```text
Dodać `CurrentUserAccessReadService`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 4 — Utworzyć Razor Page `/MyAccess`.

Opis wykonania:

```text
Utworzyć Razor Page `/MyAccess`.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 5 — Dodać link w menu.

Opis wykonania:

```text
Dodać link w menu.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.


    ---

    # Fragmenty kodu / SQL

    ## CurrentUserAccessDto.cs

```csharp
public string DomainLogin { get; init; } = string.Empty;
public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
public IReadOnlyList<string> Departments { get; init; } = Array.Empty<string>();
```

## query roles

```csharp
.Select(x => x.ApplicationRole.Code).Distinct()
```

## query departments

```csharp
.Select(x => x.Department.Code + " — " + x.Department.Name).Distinct()
```


    ---

    # Komendy

    ```powershell
    New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Web\Pages\MyAccess
dotnet build
    ```

    ---

    # Testy ręczne

    - doctor.cardio -> `Doctor`, `CARD — Kardiologia`.
- auditor.user -> `Auditor`, brak oddziałów.
- it.admin -> `ITAdministrator`, brak oddziałów.

    ---

    # Kontrola końcowa DAY021

    ```text
    [ ] Dodać CurrentUserAccessDto.
[ ] Dodać ICurrentUserAccessReadService.
[ ] Dodać CurrentUserAccessReadService.
[ ] Utworzyć Razor Page /MyAccess.
[ ] Dodać link w menu.
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
    git commit -m "DAY021 Panel użytkownika: moje role i oddziały"
    ```

    ---

    # Krótkie podsumowanie dla dokumentacji pracy

    W ramach DAY021 rozszerzono projekt HospitalAccessControl o zakres: **Panel użytkownika: moje role i oddziały**. Zmiana wzmacnia demonstracyjny model kontroli dostępu do danych medycznych oparty o Active Directory, role aplikacyjne, SESSION_CONTEXT, Row-Level Security, Dynamic Data Masking oraz audyt.

    Następny etap:

    ```text
    DAY022 — Dodawanie wpisu medycznego
    ```
