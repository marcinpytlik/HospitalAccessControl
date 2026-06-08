# DAY024 — Scenariusz demo i checklista końcowa

## Kolejność demo

1. Pokaż stronę główną `/` z `DomainLogin`, rolami i `SESSION_CONTEXT`.
2. Jako `HOSPITAL\doctor.cardio` pokaż `/Patients` — 10 pacjentów CARD.
3. Jako `HOSPITAL\doctor.cardio` pokaż `/Patients/Details/1` — dostęp działa.
4. Jako `HOSPITAL\doctor.cardio` pokaż `/Patients/Details/11` — brak dostępu.
5. Jako `HOSPITAL\doctor.ortho` pokaż `/Patients` — 10 pacjentów ORTH.
6. Jako `HOSPITAL\it.admin` pokaż `/Patients` — 0 pacjentów.
7. Jako `HOSPITAL\auditor.user` pokaż `/Audit` — panel audytu.
8. Pokaż SQL Server Audit przez `sys.fn_get_audit_file`.
9. Pokaż Dynamic Data Masking testem `hac_mask_test`.
10. Pokaż testy integracyjne RLS.

## Checklista

```text
[ ] migracje wykonane
[ ] seed danych wykonany
[ ] RLS włączony
[ ] SESSION_CONTEXT działa
[ ] /Patients działa
[ ] /Patients/Details działa
[ ] /Audit działa
[ ] /MyAccess działa
[ ] AddRecord działa dla pacjenta widocznego
[ ] AddRecord blokuje pacjenta niewidocznego przez RLS
[ ] DDM skonfigurowany
[ ] SQL Server Audit skonfigurowany
```
