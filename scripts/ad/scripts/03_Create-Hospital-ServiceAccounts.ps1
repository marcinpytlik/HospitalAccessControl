#requires -RunAsAdministrator

<#
.SYNOPSIS
    Tworzy konta usługowe dla środowiska HospitalAccessControl.
#>

$ErrorActionPreference = "Stop"
Import-Module ActiveDirectory

$DomainDN = "DC=hospital,DC=local"
$DomainNetBIOS = "HOSPITAL"
$RootOU = "OU=Hospital,$DomainDN"

$SqlServicesOU = "OU=SqlServices,OU=ServiceAccounts,$RootOU"
$AppServicesOU = "OU=AppServices,OU=ServiceAccounts,$RootOU"
$MonitoringOU  = "OU=Monitoring,OU=ServiceAccounts,$RootOU"
$BackupOU      = "OU=Backup,OU=ServiceAccounts,$RootOU"

Write-Host ""
Write-Host "Podaj hasło dla kont usługowych." -ForegroundColor Yellow
$ServiceAccountPassword = Read-Host -Prompt "Hasło dla kont usługowych" -AsSecureString

$ServiceAccounts = @(
    @{ SamAccountName = "svc_sql_engine"; Name = "svc_sql_engine"; DisplayName = "SQL Server Database Engine Service Account"; Description = "Konto usługi SQL Server Database Engine na SQL01"; Path = $SqlServicesOU },
    @{ SamAccountName = "svc_sql_agent";  Name = "svc_sql_agent";  DisplayName = "SQL Server Agent Service Account";           Description = "Konto usługi SQL Server Agent na SQL01";           Path = $SqlServicesOU },
    @{ SamAccountName = "svc_hac_app";    Name = "svc_hac_app";    DisplayName = "HospitalAccessControl Application Account"; Description = "Konto aplikacji ASP.NET Core / IIS AppPool";      Path = $AppServicesOU },
    @{ SamAccountName = "svc_hac_migr";   Name = "svc_hac_migr";   DisplayName = "HospitalAccessControl Migration Account";    Description = "Konto do migracji EF Core i zmian schematu bazy"; Path = $AppServicesOU },
    @{ SamAccountName = "svc_sql_backup"; Name = "svc_sql_backup"; DisplayName = "SQL Server Backup Service Account";          Description = "Konto techniczne do operacji backupowych";        Path = $BackupOU },
    @{ SamAccountName = "svc_sql_monitor";Name = "svc_sql_monitor";DisplayName = "SQL Server Monitoring Service Account";      Description = "Konto techniczne do monitoringu SQL Server";      Path = $MonitoringOU }
)

function New-HospitalServiceAccount {
    param(
        [Parameter(Mandatory = $true)][hashtable]$Account,
        [Parameter(Mandatory = $true)][securestring]$Password
    )

    $sam = $Account.SamAccountName
    $upn = "$sam@hospital.local"

    $existingUser = Get-ADUser -Filter "SamAccountName -eq '$sam'" -ErrorAction SilentlyContinue

    if ($existingUser) {
        Write-Host "[OK] Konto już istnieje: $DomainNetBIOS\$sam" -ForegroundColor Yellow
        return
    }

    New-ADUser `
        -SamAccountName $sam `
        -UserPrincipalName $upn `
        -Name $Account.Name `
        -DisplayName $Account.DisplayName `
        -Description $Account.Description `
        -Path $Account.Path `
        -AccountPassword $Password `
        -Enabled $true `
        -PasswordNeverExpires $true `
        -CannotChangePassword $true

    Write-Host "[UTWORZONO] Konto: $DomainNetBIOS\$sam" -ForegroundColor Green
}

foreach ($account in $ServiceAccounts) {
    New-HospitalServiceAccount -Account $account -Password $ServiceAccountPassword
}

Get-ADUser `
    -SearchBase "OU=ServiceAccounts,$RootOU" `
    -Filter * `
    -Properties DisplayName, Description, Enabled, PasswordNeverExpires |
Select-Object SamAccountName, DisplayName, Enabled, PasswordNeverExpires, DistinguishedName |
Sort-Object SamAccountName |
Format-Table -AutoSize
