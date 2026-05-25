# 06A — Dołączenie SQL01, APP01 i CLIENT01 do domeny

Plik skryptu: `06A_Join-Computer-To-HospitalDomain.ps1`

## Cel

Konfiguruje nazwę, statyczny IP, DNS na DC01 i dołącza maszynę do domeny.

## Gdzie uruchomić

`SQL01`, `APP01` albo `CLIENT01`, PowerShell jako Administrator lokalny.

## Wymagania wstępne

- `DC01` musi działać.
- Przed uruchomieniem popraw `$ComputerName` i `$IPAddress` dla danej maszyny.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\06A_Join-Computer-To-HospitalDomain.ps1
```

## Oczekiwany efekt

- Maszyna ma poprawną nazwę i jest członkiem domeny `hospital.local`.

## Testowanie i weryfikacja

```powershell
(Get-CimInstance Win32_ComputerSystem).Domain
Test-ComputerSecureChannel -Verbose
Resolve-DnsName dc01.hospital.local
whoami
```

## Typowe problemy

- Jeśli karta nie nazywa się `Ethernet`, popraw `$InterfaceAlias`.
- Po zmianie nazwy skrypt restartuje maszynę — uruchom go ponownie.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
