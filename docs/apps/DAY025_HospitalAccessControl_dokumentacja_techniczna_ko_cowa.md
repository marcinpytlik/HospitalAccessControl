# DAY025 — HospitalAccessControl
    ## Dokumentacja techniczna końcowa

    ## Cel dnia

    Zebranie końcowej dokumentacji technicznej projektu: architektura, przepływy, klasy, tabele, bezpieczeństwo, testy i demo.

    Ten etap kontynuuje styl warsztatowy z poprzednich dni: najpierw robimy jedną konkretną zmianę, potem build, test i dopiero kolejny krok.

    ---

    ## Efekt końcowy DAY025

    Po zakończeniu tego dnia projekt będzie mieć:

    ```text
    Dokumentacja techniczna końcowa
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

    # Krok 1 — Opisać architekturę projektów.

Opis wykonania:

```text
Opisać architekturę projektów.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 2 — Opisać przepływ CurrentUser -> SESSION_CONTEXT -> RLS.

Opis wykonania:

```text
Opisać przepływ CurrentUser -> SESSION_CONTEXT -> RLS.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 3 — Wypisać najważniejsze klasy.

Opis wykonania:

```text
Wypisać najważniejsze klasy.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 4 — Wypisać najważniejsze tabele i skrypty SQL.

Opis wykonania:

```text
Wypisać najważniejsze tabele i skrypty SQL.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.
# Krok 5 — Dodać checklistę końcową i tag Git.

Opis wykonania:

```text
Dodać checklistę końcową i tag Git.
```

Wykonaj zmianę zgodnie z sekcją kodu lub komend poniżej.


    ---

    # Fragmenty kodu / SQL

    ## architektura

```csharp
Web -> Application -> Infrastructure -> Domain -> SQL Server
```

## przepływ

```csharp
CurrentUserService -> SessionContextConnectionInterceptor -> SESSION_CONTEXT -> RLS -> wynik zapytania
```

## tag

```csharp
git tag v1.0.0-hospital-access-control-demo
```


    ---

    # Komendy

    ```powershell
    dotnet build
dotnet test
git status
git tag v1.0.0-hospital-access-control-demo
    ```

    ---

    # Testy ręczne

    - Dokumentacja opisuje wszystkie mechanizmy.
- Repo ma tag końcowy.
- Aplikacja jest gotowa do demonstracji.

    ---

    # Kontrola końcowa DAY025

    ```text
    [ ] Opisać architekturę projektów.
[ ] Opisać przepływ CurrentUser -> SESSION_CONTEXT -> RLS.
[ ] Wypisać najważniejsze klasy.
[ ] Wypisać najważniejsze tabele i skrypty SQL.
[ ] Dodać checklistę końcową i tag Git.
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
    git commit -m "DAY025 Dokumentacja techniczna końcowa"
    ```

    ---

    # Krótkie podsumowanie dla dokumentacji pracy

    W ramach DAY025 rozszerzono projekt HospitalAccessControl o zakres: **Dokumentacja techniczna końcowa**. Zmiana wzmacnia demonstracyjny model kontroli dostępu do danych medycznych oparty o Active Directory, role aplikacyjne, SESSION_CONTEXT, Row-Level Security, Dynamic Data Masking oraz audyt.

    Następny etap:

    ```text
    Koniec zakresu DAY000–DAY025
    ```
