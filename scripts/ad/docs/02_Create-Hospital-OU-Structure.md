# 02 — Utworzenie struktury OU

Plik skryptu: `02_Create-Hospital-OU-Structure.ps1`

## Cel

Tworzy strukturę OU dla użytkowników, grup, serwerów, stacji, kont serwisowych i administracyjnych.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator domeny.

## Wymagania wstępne

- Domena `hospital.local` musi istnieć.
- Moduł `ActiveDirectory` musi być dostępny.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\02_Create-Hospital-OU-Structure.ps1
```

## Oczekiwany efekt

- Istnieje `OU=Hospital` oraz komplet pod-OU.

## Testowanie i weryfikacja

```powershell
Get-ADOrganizationalUnit -SearchBase "OU=Hospital,DC=hospital,DC=local" -Filter * |
Select-Object Name, DistinguishedName |
Sort-Object DistinguishedName |
Format-Table -AutoSize
```

## Typowe problemy

- Jeśli brakuje domeny, najpierw uruchom skrypt 01.
- Komunikaty `[OK] OU już istnieje` są poprawne przy ponownym uruchomieniu.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
