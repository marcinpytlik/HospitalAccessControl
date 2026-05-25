# 06B — Przeniesienie kont komputerów do właściwych OU

Plik skryptu: `06B_Move-Hospital-Computers-To-OU.ps1`

## Cel

Przenosi konta komputerów `SQL01`, `APP01` i `CLIENT01` do odpowiednich OU.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator domeny.

## Wymagania wstępne

- Maszyny muszą być już dołączone do domeny.
- Struktura OU z kroku 02 musi istnieć.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\06B_Move-Hospital-Computers-To-OU.ps1
```

## Oczekiwany efekt

- `SQL01` jest w `OU=SqlServers`.
- `APP01` jest w `OU=ApplicationServers`.
- `CLIENT01` jest w `OU=Training`.

## Testowanie i weryfikacja

```powershell
Get-ADComputer -Filter "Name -in ('SQL01','APP01','CLIENT01')" -Properties DistinguishedName |
Select-Object Name,DistinguishedName |
Sort-Object Name |
Format-Table -AutoSize
```

## Typowe problemy

- Jeśli skrypt nie znajduje komputera, najpierw dołącz maszynę do domeny skryptem 06A.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
