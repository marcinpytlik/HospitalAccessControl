#requires -RunAsAdministrator

<#
.SYNOPSIS
    Konfiguruje nazwę, IP, DNS i dołącza maszynę do domeny hospital.local.

.DESCRIPTION
    Przed uruchomieniem ustaw właściwe wartości:
    - SQL01:    $ComputerName = "SQL01",    $IPAddress = "192.168.50.20"
    - APP01:    $ComputerName = "APP01",    $IPAddress = "192.168.50.30"
    - CLIENT01: $ComputerName = "CLIENT01", $IPAddress = "192.168.50.40"
#>

$ErrorActionPreference = "Stop"

$ComputerName   = "SQL01"
$IPAddress      = "192.168.50.20"

$InterfaceAlias = "Ethernet"
$PrefixLength   = 24
$DefaultGateway = "192.168.50.1"
$DnsServer      = "192.168.50.10"
$DomainName     = "hospital.local"

$adapter = Get-NetAdapter -Name $InterfaceAlias -ErrorAction SilentlyContinue

if (-not $adapter) {
    Write-Host "Nie znaleziono karty sieciowej: $InterfaceAlias" -ForegroundColor Red
    Get-NetAdapter | Select-Object Name, Status, MacAddress, LinkSpeed | Format-Table -AutoSize
    throw "Popraw zmienną `$InterfaceAlias w skrypcie."
}

Get-NetIPAddress -InterfaceAlias $InterfaceAlias -AddressFamily IPv4 -ErrorAction SilentlyContinue |
Where-Object { $_.IPAddress -ne "127.0.0.1" } |
Remove-NetIPAddress -Confirm:$false -ErrorAction SilentlyContinue

Get-NetRoute -InterfaceAlias $InterfaceAlias -DestinationPrefix "0.0.0.0/0" -ErrorAction SilentlyContinue |
Remove-NetRoute -Confirm:$false -ErrorAction SilentlyContinue

New-NetIPAddress -InterfaceAlias $InterfaceAlias -IPAddress $IPAddress -PrefixLength $PrefixLength -DefaultGateway $DefaultGateway
Set-DnsClientServerAddress -InterfaceAlias $InterfaceAlias -ServerAddresses $DnsServer

if (-not (Test-Connection -ComputerName $DnsServer -Count 2 -Quiet)) {
    throw "Brak odpowiedzi z DC01/DNS: $DnsServer"
}

Resolve-DnsName $DomainName -Server $DnsServer -ErrorAction Stop | Out-Null

$currentComputerName = $env:COMPUTERNAME

if ($currentComputerName -ne $ComputerName) {
    Rename-Computer -NewName $ComputerName -Force
    Write-Host "Po restarcie uruchom ten sam skrypt ponownie." -ForegroundColor Yellow
    Restart-Computer -Force
    exit
}

$computerSystem = Get-CimInstance Win32_ComputerSystem

if ($computerSystem.PartOfDomain -eq $true -and $computerSystem.Domain -ieq $DomainName) {
    Write-Host "[OK] Maszyna jest już w domenie: $DomainName" -ForegroundColor Yellow
    return
}

Write-Host "Podaj konto z uprawnieniami do dołączenia komputera do domeny, np. HOSPITAL\Administrator." -ForegroundColor Yellow
$DomainCredential = Get-Credential

Add-Computer -DomainName $DomainName -Credential $DomainCredential -Force
Write-Host "[OK] Komputer został dołączony do domeny. Nastąpi restart." -ForegroundColor Green
Restart-Computer -Force
