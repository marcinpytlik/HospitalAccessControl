#requires -RunAsAdministrator

<#
.SYNOPSIS
    Tworzy strukturę OU dla projektu HospitalAccessControl.
#>

$ErrorActionPreference = "Stop"
Import-Module ActiveDirectory

$DomainDN = "DC=hospital,DC=local"
$RootOUName = "Hospital"
$RootOUPath = "OU=$RootOUName,$DomainDN"

function New-HospitalOU {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Path,
        [string]$Description = ""
    )

    $ouDN = "OU=$Name,$Path"

    $existingOU = Get-ADOrganizationalUnit `
        -LDAPFilter "(ou=$Name)" `
        -SearchBase $Path `
        -SearchScope OneLevel `
        -ErrorAction SilentlyContinue

    if ($existingOU) {
        Write-Host "[OK] OU już istnieje: $ouDN" -ForegroundColor Yellow
    }
    else {
        New-ADOrganizationalUnit `
            -Name $Name `
            -Path $Path `
            -Description $Description `
            -ProtectedFromAccidentalDeletion $true

        Write-Host "[UTWORZONO] OU: $ouDN" -ForegroundColor Green
    }
}

$existingRootOU = Get-ADOrganizationalUnit `
    -LDAPFilter "(ou=$RootOUName)" `
    -SearchBase $DomainDN `
    -SearchScope OneLevel `
    -ErrorAction SilentlyContinue

if ($existingRootOU) {
    Write-Host "[OK] OU główne już istnieje: $RootOUPath" -ForegroundColor Yellow
}
else {
    New-ADOrganizationalUnit `
        -Name $RootOUName `
        -Path $DomainDN `
        -Description "Główna jednostka organizacyjna dla środowiska HospitalAccessControl" `
        -ProtectedFromAccidentalDeletion $true

    Write-Host "[UTWORZONO] OU główne: $RootOUPath" -ForegroundColor Green
}

New-HospitalOU -Name "Users"           -Path $RootOUPath -Description "Użytkownicy szpitala"
New-HospitalOU -Name "Groups"          -Path $RootOUPath -Description "Grupy bezpieczeństwa i grupy dostępowe"
New-HospitalOU -Name "Servers"         -Path $RootOUPath -Description "Serwery środowiska"
New-HospitalOU -Name "Workstations"    -Path $RootOUPath -Description "Stacje robocze użytkowników"
New-HospitalOU -Name "ServiceAccounts" -Path $RootOUPath -Description "Konta serwisowe aplikacji i usług"
New-HospitalOU -Name "AdminAccounts"   -Path $RootOUPath -Description "Konta administracyjne"
New-HospitalOU -Name "TestLab"         -Path $RootOUPath -Description "Obiekty testowe środowiska laboratoryjnego"

$UsersOU = "OU=Users,$RootOUPath"
New-HospitalOU -Name "Doctors"            -Path $UsersOU -Description "Lekarze"
New-HospitalOU -Name "Nurses"             -Path $UsersOU -Description "Pielęgniarki"
New-HospitalOU -Name "Registration"       -Path $UsersOU -Description "Pracownicy rejestracji"
New-HospitalOU -Name "DepartmentManagers" -Path $UsersOU -Description "Kierownicy oddziałów"
New-HospitalOU -Name "Auditors"           -Path $UsersOU -Description "Audytorzy"
New-HospitalOU -Name "IT"                 -Path $UsersOU -Description "Pracownicy IT"

$GroupsOU = "OU=Groups,$RootOUPath"
New-HospitalOU -Name "RoleGroups"       -Path $GroupsOU -Description "Grupy ról biznesowych"
New-HospitalOU -Name "DepartmentGroups" -Path $GroupsOU -Description "Grupy oddziałów szpitalnych"
New-HospitalOU -Name "SqlAccessGroups"  -Path $GroupsOU -Description "Grupy dostępowe do SQL Server"
New-HospitalOU -Name "AppAccessGroups"  -Path $GroupsOU -Description "Grupy dostępowe do aplikacji"
New-HospitalOU -Name "AdminGroups"      -Path $GroupsOU -Description "Grupy administracyjne"

$ServersOU = "OU=Servers,$RootOUPath"
New-HospitalOU -Name "DomainControllers"  -Path $ServersOU -Description "Kontrolery domeny"
New-HospitalOU -Name "SqlServers"         -Path $ServersOU -Description "Serwery SQL Server"
New-HospitalOU -Name "ApplicationServers" -Path $ServersOU -Description "Serwery aplikacyjne"
New-HospitalOU -Name "ManagementServers"  -Path $ServersOU -Description "Serwery administracyjne"

$WorkstationsOU = "OU=Workstations,$RootOUPath"
New-HospitalOU -Name "MedicalStaff" -Path $WorkstationsOU -Description "Stacje personelu medycznego"
New-HospitalOU -Name "Registration" -Path $WorkstationsOU -Description "Stacje rejestracji"
New-HospitalOU -Name "IT"           -Path $WorkstationsOU -Description "Stacje działu IT"
New-HospitalOU -Name "Training"     -Path $WorkstationsOU -Description "Stacje szkoleniowe"

$ServiceAccountsOU = "OU=ServiceAccounts,$RootOUPath"
New-HospitalOU -Name "SqlServices" -Path $ServiceAccountsOU -Description "Konta usług SQL Server"
New-HospitalOU -Name "AppServices" -Path $ServiceAccountsOU -Description "Konta usług aplikacyjnych"
New-HospitalOU -Name "Monitoring"  -Path $ServiceAccountsOU -Description "Konta monitoringu"
New-HospitalOU -Name "Backup"      -Path $ServiceAccountsOU -Description "Konta backupowe"

$AdminAccountsOU = "OU=AdminAccounts,$RootOUPath"
New-HospitalOU -Name "DomainAdmins" -Path $AdminAccountsOU -Description "Konta administracji domenowej"
New-HospitalOU -Name "SqlAdmins"    -Path $AdminAccountsOU -Description "Konta administratorów SQL Server"
New-HospitalOU -Name "AppAdmins"    -Path $AdminAccountsOU -Description "Konta administratorów aplikacji"

$TestLabOU = "OU=TestLab,$RootOUPath"
New-HospitalOU -Name "TestUsers"   -Path $TestLabOU -Description "Użytkownicy testowi"
New-HospitalOU -Name "TestGroups"  -Path $TestLabOU -Description "Grupy testowe"
New-HospitalOU -Name "TestServers" -Path $TestLabOU -Description "Serwery testowe"
New-HospitalOU -Name "TestClients" -Path $TestLabOU -Description "Klienci testowi"

Get-ADOrganizationalUnit `
    -SearchBase $RootOUPath `
    -Filter * |
Select-Object Name, DistinguishedName |
Sort-Object DistinguishedName |
Format-Table -AutoSize
