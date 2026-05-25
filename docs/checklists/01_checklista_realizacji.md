# Checklista realizacji projektu

## Active Directory

- [ ] `DC01` ma statyczny adres IP.
- [ ] DNS na `DC01` wskazuje na siebie.
- [ ] Domena `hospital.local` została utworzona.
- [ ] OU `Hospital` została utworzona.
- [ ] Utworzono OU dla użytkowników, grup, serwerów i kont usługowych.
- [ ] Utworzono konta usługowe.
- [ ] Utworzono grupy ról.
- [ ] Utworzono grupy oddziałów.
- [ ] Utworzono grupy dostępowe SQL.
- [ ] Utworzono użytkowników testowych.
- [ ] `SQL01`, `APP01`, `CLIENT01` dołączono do domeny.

## SQL Server

- [ ] SQL Server 2022 Developer zainstalowany na `SQL01`.
- [ ] Usługi SQL działają na kontach domenowych.
- [ ] Utworzono bazę `HospitalAccessControlDb`.
- [ ] Utworzono schematy.
- [ ] Utworzono tabele.
- [ ] Wprowadzono dane testowe.
- [ ] Utworzono loginy dla grup AD.
- [ ] Utworzono role bazodanowe.
- [ ] Wdrożono RLS.
- [ ] Wdrożono SQL Server Audit.
- [ ] Wdrożono Dynamic Data Masking.

## Aplikacja

- [ ] Utworzono solution .NET 10.
- [ ] Utworzono projekty warstwowe.
- [ ] Skonfigurowano EF Core.
- [ ] Skonfigurowano Windows Authentication.
- [ ] Zaimplementowano listę pacjentów.
- [ ] Zaimplementowano szczegóły pacjenta.
- [ ] Zaimplementowano panel audytu.
- [ ] Opublikowano aplikację na `APP01`.

## Testy

- [ ] Test lekarza kardiologii.
- [ ] Test lekarza ortopedii.
- [ ] Test pielęgniarki.
- [ ] Test rejestracji.
- [ ] Test audytora.
- [ ] Test administratora IT.
- [ ] Test bezpośredniego zapytania SQL.
