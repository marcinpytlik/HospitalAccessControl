# 04 — Utworzenie grup RBAC

Plik skryptu: `04_Create-Hospital-Groups.ps1`

## Cel

Tworzy grupy ról, oddziałów, dostępowe do SQL Server, dostępowe do aplikacji i administracyjne.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator domeny.

## Wymagania wstępne

- Struktura OU z kroku 02 musi istnieć.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\04_Create-Hospital-Groups.ps1
```

## Oczekiwany efekt

- Istnieją grupy `GG_HOSP_ROLE_*`, `GG_HOSP_DEPT_*`, `GG_SQL_*`, `GG_APP_*`.
- Podstawowe zagnieżdżenia grup są wykonane.

## Testowanie i weryfikacja

```powershell
Get-ADGroup -SearchBase "OU=Groups,OU=Hospital,DC=hospital,DC=local" -Filter * |
Select-Object Name,DistinguishedName |
Sort-Object Name |
Format-Table -AutoSize

Get-ADGroupMember GG_SQL_HospitalAccessControl_Doctor -Recursive
```

## Typowe problemy

- Jeśli brakuje OU, uruchom skrypt 02.
- Skrypt można uruchomić ponownie.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
