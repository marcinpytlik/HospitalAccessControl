# 05 — Utworzenie użytkowników testowych

Plik skryptu: `05_Create-Hospital-TestUsers.ps1`

## Cel

Tworzy konta testowe lekarzy, pielęgniarek, rejestracji, kierowników, audytora i administratorów.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator domeny.

## Wymagania wstępne

- Struktura OU z kroku 02 musi istnieć.
- Grupy z kroku 04 muszą istnieć.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\05_Create-Hospital-TestUsers.ps1
```

## Oczekiwany efekt

- Istnieją konta np. `doctor.cardio`, `nurse.cardio`, `registration.user`, `auditor.user`, `it.admin`.
- Konta są przypisane do grup ról i oddziałów.

## Testowanie i weryfikacja

```powershell
Get-ADUser -SearchBase "OU=Users,OU=Hospital,DC=hospital,DC=local" -Filter * |
Select-Object SamAccountName,Name,DistinguishedName |
Sort-Object SamAccountName |
Format-Table -AutoSize

Get-ADPrincipalGroupMembership doctor.cardio | Select-Object Name | Sort-Object Name
```

## Typowe problemy

- Jeśli brakuje grupy, uruchom skrypt 04.
- Jeśli hasło jest za słabe, podaj silniejsze.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
