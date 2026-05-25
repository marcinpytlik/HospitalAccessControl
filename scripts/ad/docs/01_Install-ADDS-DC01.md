# 01 — Instalacja Active Directory Domain Services na DC01

Plik skryptu: `01_Install-ADDS-DC01.ps1`

## Cel

Instaluje AD DS i DNS oraz tworzy pierwszy kontroler domeny `hospital.local`.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator lokalny.

## Wymagania wstępne

- Windows Server 2022.
- Poprawna nazwa karty sieciowej w `$InterfaceAlias`.
- Serwer nie powinien być wcześniej kontrolerem domeny.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\01_Install-ADDS-DC01.ps1
```

## Oczekiwany efekt

- Serwer nazywa się `DC01`.
- Domena `hospital.local` istnieje.
- DNS działa na `192.168.50.10`.

## Testowanie i weryfikacja

```powershell
Get-ADDomain
Get-ADForest
Get-Service ADWS,DNS,KDC,Netlogon
dcdiag
nslookup hospital.local
```

## Typowe problemy

- Jeśli skrypt zmieni nazwę komputera, uruchom go ponownie po restarcie.
- Jeśli karta nie nazywa się `Ethernet`, popraw `$InterfaceAlias`.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
