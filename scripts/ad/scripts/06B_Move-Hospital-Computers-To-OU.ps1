#requires -RunAsAdministrator

<#
.SYNOPSIS
    Przenosi konta komputerów SQL01, APP01 i CLIENT01 do odpowiednich OU.
#>

$ErrorActionPreference = "Stop"
Import-Module ActiveDirectory

$DomainDN = "DC=hospital,DC=local"
$RootOU   = "OU=Hospital,$DomainDN"

$ComputerMoves = @(
    @{ ComputerName = "SQL01";    TargetOU = "OU=SqlServers,OU=Servers,$RootOU" },
    @{ ComputerName = "APP01";    TargetOU = "OU=ApplicationServers,OU=Servers,$RootOU" },
    @{ ComputerName = "CLIENT01"; TargetOU = "OU=Training,OU=Workstations,$RootOU" }
)

function Move-HospitalComputer {
    param(
        [Parameter(Mandatory = $true)][string]$ComputerName,
        [Parameter(Mandatory = $true)][string]$TargetOU
    )

    $computer = Get-ADComputer -Identity $ComputerName -ErrorAction SilentlyContinue

    if (-not $computer) {
        Write-Host "[POMINIĘTO] Nie znaleziono konta komputera: $ComputerName" -ForegroundColor Yellow
        return
    }

    $targetOuObject = Get-ADOrganizationalUnit -Identity $TargetOU -ErrorAction SilentlyContinue

    if (-not $targetOuObject) {
        Write-Host "[BŁĄD] Nie znaleziono docelowej OU: $TargetOU" -ForegroundColor Red
        return
    }

    if ($computer.DistinguishedName -like "*$TargetOU") {
        Write-Host "[OK] Komputer $ComputerName jest już w docelowej OU." -ForegroundColor Yellow
        return
    }

    Move-ADObject -Identity $computer.DistinguishedName -TargetPath $TargetOU
    Write-Host "[PRZENIESIONO] $ComputerName -> $TargetOU" -ForegroundColor Green
}

foreach ($move in $ComputerMoves) {
    Move-HospitalComputer -ComputerName $move.ComputerName -TargetOU $move.TargetOU
}

Get-ADComputer -Filter "Name -in ('SQL01','APP01','CLIENT01')" -Properties DistinguishedName |
Select-Object Name, DistinguishedName |
Sort-Object Name |
Format-Table -AutoSize
