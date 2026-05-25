# 03 — Utworzenie kont usługowych

Plik skryptu: `03_Create-Hospital-ServiceAccounts.ps1`

## Cel

Tworzy konta usługowe dla SQL Engine, SQL Agent, aplikacji, migracji, backupu i monitoringu.

## Gdzie uruchomić

`DC01`, PowerShell jako Administrator domeny.

## Wymagania wstępne

- Struktura OU z kroku 02 musi istnieć.
- Hasło musi spełniać politykę domenową.

## Uruchomienie

```powershell
cd C:\HospitalAccessControl\scripts
Set-ExecutionPolicy Bypass -Scope Process -Force
.\03_Create-Hospital-ServiceAccounts.ps1
```

## Oczekiwany efekt

- Istnieją konta `svc_sql_engine`, `svc_sql_agent`, `svc_hac_app`, `svc_hac_migr`, `svc_sql_backup`, `svc_sql_monitor`.

## Testowanie i weryfikacja

```powershell
Get-ADUser -SearchBase "OU=ServiceAccounts,OU=Hospital,DC=hospital,DC=local" -Filter * -Properties DisplayName,PasswordNeverExpires |
Select-Object SamAccountName,DisplayName,Enabled,PasswordNeverExpires |
Sort-Object SamAccountName |
Format-Table -AutoSize
```

## Typowe problemy

- Jeśli brakuje OU, uruchom skrypt 02.
- Jeśli hasło jest zbyt słabe, podaj silniejsze.

## Notatka do pracy inżynierskiej

Ten skrypt jest częścią automatyzacji przygotowania środowiska laboratoryjnego. Automatyzacja pozwala powtarzalnie odtworzyć konfigurację, ograniczyć błędy ręczne i jasno udokumentować proces wdrożenia.
