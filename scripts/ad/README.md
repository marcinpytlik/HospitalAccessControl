# HospitalAccessControl — pakiet skryptów AD Lab

Pakiet zawiera skrypty PowerShell do przygotowania laboratoryjnego środowiska Active Directory dla pracy inżynierskiej.

## Założenia

| Element | Wartość |
|---|---|
| Domena | `hospital.local` |
| NetBIOS | `HOSPITAL` |
| DC | `DC01`, `192.168.50.10` |
| SQL Server | `SQL01`, `192.168.50.20` |
| App Server | `APP01`, `192.168.50.30` |
| Klient | `CLIENT01`, `192.168.50.40` |
| DNS | `192.168.50.10` |

## Kolejność wykonania

1. `01_Install-ADDS-DC01.ps1` — na `DC01`
2. `02_Create-Hospital-OU-Structure.ps1` — na `DC01`
3. `03_Create-Hospital-ServiceAccounts.ps1` — na `DC01`
4. `04_Create-Hospital-Groups.ps1` — na `DC01`
5. `05_Create-Hospital-TestUsers.ps1` — na `DC01`
6. `06A_Join-Computer-To-HospitalDomain.ps1` — na `SQL01`, `APP01`, `CLIENT01`
7. `06B_Move-Hospital-Computers-To-OU.ps1` — na `DC01`

## Uruchamianie

Każdy skrypt uruchamiaj w PowerShell jako Administrator:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force
.\NAZWA_SKRYPTU.ps1
```

## Ważne

Przed uruchomieniem skryptów sprawdź nazwę karty sieciowej:

```powershell
Get-NetAdapter
```

Jeśli karta nie nazywa się `Ethernet`, popraw zmienną:

```powershell
$InterfaceAlias = "Ethernet"
```
