# HospitalAccessControl — Database Design

Ten katalog zawiera projekt bazy danych dla pracy inżynierskiej **HospitalAccessControl**.

## Zawartość

```text
docs/database/
├── 01_projekt_bazy_danych.md
├── 02_tabele.md
├── 03_relacje_i_erd.md
├── 04_bezpieczenstwo_rls_rbac_audit.md
├── 05_dane_testowe.md
├── 06_konwencje_nazewnicze.md
├── 07_kolejnosc_implementacji_sql.md
├── diagrams/
│   └── hospitalaccesscontrol_erd.mmd
├── checklists/
│   └── checklista_projektu_bazy.md
└── test-plans/
    └── plan_testow_bazy_i_rls.md

sql/
├── 01_database/
├── 02_schemas/
├── 03_tables/
├── 04_constraints_indexes/
├── 05_seed/
├── 06_security/
├── 07_rls/
├── 08_audit/
└── 09_tests/
```

## Cel

Celem projektu bazy jest przygotowanie struktury danych umożliwiającej testowanie:

- RBAC,
- Row-Level Security,
- Dynamic Data Masking,
- SQL Server Audit,
- audytu aplikacyjnego,
- kontroli dostępu według oddziałów i ról użytkowników.

## Następny krok

Następny etap to przygotowanie właściwych skryptów T-SQL w katalogu `sql/`.
