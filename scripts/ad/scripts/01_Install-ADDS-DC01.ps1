#requires -RunAsAdministrator

<#
.SYNOPSIS
    Instalacja Active Directory Domain Services na serwerze DC01.

.DESCRIPTION
    Skrypt konfiguruje pierwszy kontroler domeny:
    - nazwa serwera: DC01
    - adres IP: 192.168.50.10/24
    - DNS: 192.168.50.10
    - domena: hospital.local
    - NetBIOS: HOSPITAL
#>

$ErrorActionPreference = "Stop"

$ComputerName = "DC01"
$InterfaceAlias = "Ethernet"

$IPAddress = "192.168.50.10"
$PrefixLength = 24
$DefaultGateway = "192.168.50.1"
$DnsServer = "192.168.50.10"

$DomainName = "hospital.local"
$DomainNetbiosName = "HOSPITAL"

$DatabasePath = "C:\Windows\NTDS"
$LogPath = "C:\Windows\NTDS"
$SysvolPath = "C:\Windows\SYSVOL"

Write-Host "Podaj hasło trybu przywracania usług katalogowych DSRM." -ForegroundColor Yellow
$SafeModeAdministratorPassword = Read-Host -Prompt "DSRM Password" -AsSecureString

Write-Host ""
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host " Instalacja Active Directory na DC01" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "Nazwa serwera: $ComputerName"
Write-Host "Adres IP:      $IPAddress/$PrefixLength"
Write-Host "Brama:         $DefaultGateway"
Write-Host "DNS:           $DnsServer"
Write-Host "Domena:        $DomainName"
Write-Host "NetBIOS:       $DomainNetbiosName"
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host ""

$adapter = Get-NetAdapter -Name $InterfaceAlias -ErrorAction SilentlyContinue

if (-not $adapter) {
    Write-Host "Nie znaleziono karty sieciowej o nazwie: $InterfaceAlias" -ForegroundColor Red
    Get-NetAdapter | Select-Object Name, Status, MacAddress, LinkSpeed | Format-Table
    throw "Popraw zmienną `$InterfaceAlias w skrypcie."
}

$currentName = $env:COMPUTERNAME

if ($currentName -ne $ComputerName) {
    Write-Host "Zmiana nazwy komputera z $currentName na $ComputerName..." -ForegroundColor Yellow
    Rename-Computer -NewName $ComputerName -Force
    Write-Host "Po restarcie uruchom ten skrypt ponownie." -ForegroundColor Yellow
    Restart-Computer -Force
    exit
}

Write-Host "Konfiguracja adresu IP..." -ForegroundColor Cyan

Get-NetIPAddress -InterfaceAlias $InterfaceAlias -AddressFamily IPv4 -ErrorAction SilentlyContinue |
Where-Object { $_.IPAddress -ne "127.0.0.1" } |
Remove-NetIPAddress -Confirm:$false -ErrorAction SilentlyContinue

Get-NetRoute -InterfaceAlias $InterfaceAlias -DestinationPrefix "0.0.0.0/0" -ErrorAction SilentlyContinue |
Remove-NetRoute -Confirm:$false -ErrorAction SilentlyContinue

New-NetIPAddress `
    -InterfaceAlias $InterfaceAlias `
    -IPAddress $IPAddress `
    -PrefixLength $PrefixLength `
    -DefaultGateway $DefaultGateway

Set-DnsClientServerAddress `
    -InterfaceAlias $InterfaceAlias `
    -ServerAddresses $DnsServer

Write-Host "Instalacja roli AD DS oraz DNS..." -ForegroundColor Cyan

Install-WindowsFeature `
    -Name AD-Domain-Services, DNS `
    -IncludeManagementTools

Import-Module ADDSDeployment

Write-Host "Promocja serwera do kontrolera domeny..." -ForegroundColor Cyan

Install-ADDSForest `
    -DomainName $DomainName `
    -DomainNetbiosName $DomainNetbiosName `
    -SafeModeAdministratorPassword $SafeModeAdministratorPassword `
    -DatabasePath $DatabasePath `
    -LogPath $LogPath `
    -SysvolPath $SysvolPath `
    -InstallDns `
    -CreateDnsDelegation:$false `
    -Force
