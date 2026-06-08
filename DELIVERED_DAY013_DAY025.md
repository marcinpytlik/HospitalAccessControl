# HospitalAccessControl — dostarczenie DAY013-DAY025

## Zakres wykonany w paczce

```text
DAY013 RequestedPatientId w audycie
DAY014 Panel audytu /Audit
DAY015 Role aplikacyjne i ograniczenie /Audit
DAY016 Dynamic Data Masking — skrypty SQL
DAY017 SQL Server Audit — skrypty SQL
DAY018 Windows Authentication mode
DAY019 Publikacja IIS — dokumentacja w docs/apps
DAY020 Least privilege — skrypty SQL
DAY021 Panel /MyAccess
DAY022 Dodawanie wpisu medycznego /Patients/AddRecord/{id}
DAY023 Testy integracyjne RLS
DAY024 Scenariusz demo
DAY025 Dokumentacja techniczna końcowa
```

## Ważna uwaga

Po rozpakowaniu na komputerze developerskim wykonaj:

```powershell
dotnet build
dotnet test
```

Jeżeli pojawi się błąd, zacznij od pierwszego błędu kompilacji i popraw pojedynczo.
