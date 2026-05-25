# HospitalAccessControl

Projekt laboratoryjny do pracy inżynierskiej:

**Projekt i implementacja modelu kontroli dostępu do danych medycznych z wykorzystaniem Active Directory, RBAC oraz Row-Level Security w Microsoft SQL Server**

## Cel repozytorium

Repozytorium przechowuje komplet materiałów technicznych dla projektu:

- dokumentację projektową,
- skrypty PowerShell do Active Directory i serwerów,
- skrypty T-SQL do SQL Server,
- projekt aplikacji .NET 10,
- testy,
- checklisty,
- diagramy,
- wyniki testów i zrzuty ekranu.

## Stack technologiczny

- Windows Server 2022
- Active Directory Domain Services
- DNS
- SQL Server 2022 Developer Edition
- SQL Server Audit
- Row-Level Security
- Dynamic Data Masking
- .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core 10
- IIS
- Windows Authentication
- Windows 11 jako klient testowy
- PowerShell
- T-SQL
- Visual Studio Code
- Git

## Środowisko laboratoryjne

| Maszyna | FQDN | IP | Rola |
|---|---|---:|---|
| `DC01` | `dc01.hospital.local` | `192.168.50.10` | Active Directory + DNS |
| `SQL01` | `sql01.hospital.local` | `192.168.50.20` | SQL Server 2022 Developer |
| `APP01` | `app01.hospital.local` | `192.168.50.30` | IIS + .NET 10 + aplikacja |
| `CLIENT01` | `client01.hospital.local` | `192.168.50.40` | Windows 11, klient testowy |

## Struktura repozytorium

```text
HospitalAccessControl/
├── README.md
├── .gitignore
├── docs/
│   ├── 01_harmonogram_prac.md
│   ├── 02_architektura_srodowiska.md
│   ├── 03_model_active_directory.md
│   ├── 04_model_rbac.md
│   ├── 05_model_bazy_danych.md
│   ├── 06_mechanizmy_sql_security.md
│   ├── 07_aplikacja_dotnet10.md
│   ├── 08_plan_testow.md
│   ├── diagrams/
│   ├── checklists/
│   └── test-plans/
├── scripts/
│   ├── ad/
│   ├── server/
│   └── sql/
├── sql/
│   ├── 00_instance/
│   ├── 01_database/
│   ├── 02_security/
│   ├── 03_seed/
│   └── 04_tests/
├── src/
│   ├── HospitalAccessControl.Domain/
│   ├── HospitalAccessControl.Application/
│   ├── HospitalAccessControl.Infrastructure/
│   └── HospitalAccessControl.Web/
├── tests/
│   ├── HospitalAccessControl.Tests/
│   └── HospitalAccessControl.IntegrationTests/
└── artifacts/
    ├── screenshots/
    └── reports/
```

## Kolejność prac

1. Przygotowanie środowiska laboratoryjnego.
2. Instalacja i konfiguracja Active Directory.
3. Utworzenie OU, grup, kont usługowych i użytkowników testowych.
4. Dołączenie `SQL01`, `APP01` i `CLIENT01` do domeny.
5. Instalacja i konfiguracja SQL Server 2022.
6. Utworzenie bazy `HospitalAccessControlDb`.
7. Implementacja modelu danych.
8. Implementacja RBAC i Row-Level Security.
9. Implementacja SQL Server Audit i Dynamic Data Masking.
10. Utworzenie aplikacji .NET 10.
11. Publikacja aplikacji na `APP01`.
12. Testy dostępu i bezpieczeństwa.
13. Dokumentacja wyników.
14. Przygotowanie demonstracji na obronę.

## Projekty aplikacyjne

Docelowa struktura solution:

```text
HospitalAccessControl.slnx

src/
├── HospitalAccessControl.Domain
├── HospitalAccessControl.Application
├── HospitalAccessControl.Infrastructure
└── HospitalAccessControl.Web

tests/
├── HospitalAccessControl.Tests
└── HospitalAccessControl.IntegrationTests
```

## Warstwy aplikacji

| Warstwa | Projekt | Odpowiedzialność |
|---|---|---|
| Domain | `HospitalAccessControl.Domain` | Encje domenowe i logika domenowa |
| Application | `HospitalAccessControl.Application` | Usługi aplikacyjne, DTO, interfejsy |
| Infrastructure | `HospitalAccessControl.Infrastructure` | EF Core, SQL Server, konfiguracje encji |
| Web | `HospitalAccessControl.Web` | Razor Pages, Windows Authentication, UI |
| Tests | `HospitalAccessControl.Tests` | Testy jednostkowe |
| IntegrationTests | `HospitalAccessControl.IntegrationTests` | Testy integracyjne i bezpieczeństwa |

## Główne encje

- `Department`
- `Patient`
- `PatientAdmission`
- `MedicalRecord`
- `PatientNote`
- `ApplicationUser`
- `ApplicationRole`
- `UserDepartmentAccess`
- `UserRoleAssignment`
- `AccessLog`
- `SecurityEvent`

## Uwaga

Repozytorium jest projektem laboratoryjnym przygotowanym do celów pracy inżynierskiej. Nie jest pełnym systemem szpitalnym klasy HIS. Celem projektu jest demonstracja modelu bezpiecznego dostępu do danych pacjentów.
