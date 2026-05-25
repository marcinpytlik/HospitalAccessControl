#requires -RunAsAdministrator

<#
.SYNOPSIS
    Tworzy grupy Active Directory dla projektu HospitalAccessControl.
#>

$ErrorActionPreference = "Stop"
Import-Module ActiveDirectory

$DomainDN = "DC=hospital,DC=local"
$GroupsRootOU = "OU=Groups,OU=Hospital,$DomainDN"

$RoleGroupsOU       = "OU=RoleGroups,$GroupsRootOU"
$DepartmentGroupsOU = "OU=DepartmentGroups,$GroupsRootOU"
$SqlAccessGroupsOU  = "OU=SqlAccessGroups,$GroupsRootOU"
$AppAccessGroupsOU  = "OU=AppAccessGroups,$GroupsRootOU"
$AdminGroupsOU      = "OU=AdminGroups,$GroupsRootOU"

function New-HospitalGroup {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Description,
        [ValidateSet("Global", "DomainLocal", "Universal")][string]$GroupScope = "Global",
        [ValidateSet("Security", "Distribution")][string]$GroupCategory = "Security"
    )

    $existingGroup = Get-ADGroup -Filter "Name -eq '$Name'" -ErrorAction SilentlyContinue

    if ($existingGroup) {
        Write-Host "[OK] Grupa już istnieje: $Name" -ForegroundColor Yellow
        return
    }

    New-ADGroup -Name $Name -SamAccountName $Name -GroupScope $GroupScope -GroupCategory $GroupCategory -Path $Path -Description $Description
    Write-Host "[UTWORZONO] Grupa: $Name" -ForegroundColor Green
}

function Add-HospitalGroupMember {
    param(
        [Parameter(Mandatory = $true)][string]$ParentGroup,
        [Parameter(Mandatory = $true)][string]$ChildGroup
    )

    $parent = Get-ADGroup -Identity $ParentGroup -ErrorAction Stop
    $child  = Get-ADGroup -Identity $ChildGroup -ErrorAction Stop

    $isMember = Get-ADGroupMember -Identity $parent.DistinguishedName -Recursive |
        Where-Object { $_.DistinguishedName -eq $child.DistinguishedName }

    if ($isMember) {
        Write-Host "[OK] $ChildGroup jest już członkiem $ParentGroup" -ForegroundColor Yellow
        return
    }

    Add-ADGroupMember -Identity $ParentGroup -Members $ChildGroup
    Write-Host "[DODANO] $ChildGroup -> $ParentGroup" -ForegroundColor Green
}

$RoleGroups = @(
    @{ Name = "GG_HOSP_ROLE_Doctor";            Description = "Rola biznesowa: lekarz" },
    @{ Name = "GG_HOSP_ROLE_Nurse";             Description = "Rola biznesowa: pielęgniarka" },
    @{ Name = "GG_HOSP_ROLE_Registration";      Description = "Rola biznesowa: pracownik rejestracji" },
    @{ Name = "GG_HOSP_ROLE_DepartmentManager"; Description = "Rola biznesowa: kierownik oddziału" },
    @{ Name = "GG_HOSP_ROLE_Auditor";           Description = "Rola biznesowa: audytor" },
    @{ Name = "GG_HOSP_ROLE_ITAdministrator";   Description = "Rola biznesowa: administrator IT" }
)

$DepartmentGroups = @(
    @{ Name = "GG_HOSP_DEPT_Cardiology";  Description = "Oddział: Kardiologia" },
    @{ Name = "GG_HOSP_DEPT_Orthopedics"; Description = "Oddział: Ortopedia" },
    @{ Name = "GG_HOSP_DEPT_Neurology";   Description = "Oddział: Neurologia" },
    @{ Name = "GG_HOSP_DEPT_Emergency";   Description = "Oddział: Izba Przyjęć" },
    @{ Name = "GG_HOSP_DEPT_Pediatrics";  Description = "Oddział: Pediatria" }
)

$SqlGroups = @(
    @{ Name = "GG_SQL_HospitalAccessControl_Doctor";       Description = "Dostęp SQL dla lekarzy" },
    @{ Name = "GG_SQL_HospitalAccessControl_Nurse";        Description = "Dostęp SQL dla pielęgniarek" },
    @{ Name = "GG_SQL_HospitalAccessControl_Registration"; Description = "Dostęp SQL dla rejestracji" },
    @{ Name = "GG_SQL_HospitalAccessControl_Manager";      Description = "Dostęp SQL dla kierowników oddziałów" },
    @{ Name = "GG_SQL_HospitalAccessControl_Auditor";      Description = "Dostęp SQL dla audytorów" },
    @{ Name = "GG_SQL_HospitalAccessControl_Admin";        Description = "Dostęp administracyjny SQL" },
    @{ Name = "GG_SQL_HospitalAccessControl_Runtime";      Description = "Dostęp runtime aplikacji" },
    @{ Name = "GG_SQL_HospitalAccessControl_Migration";    Description = "Dostęp migracyjny EF Core" },
    @{ Name = "GG_SQL_HospitalAccessControl_Monitoring";   Description = "Dostęp monitoringowy" },
    @{ Name = "GG_SQL_HospitalAccessControl_Backup";       Description = "Dostęp backupowy" }
)

$AppGroups = @(
    @{ Name = "GG_APP_HospitalAccessControl_Users";        Description = "Użytkownicy aplikacji" },
    @{ Name = "GG_APP_HospitalAccessControl_MedicalStaff"; Description = "Personel medyczny aplikacji" },
    @{ Name = "GG_APP_HospitalAccessControl_Registration"; Description = "Rejestracja w aplikacji" },
    @{ Name = "GG_APP_HospitalAccessControl_Auditors";     Description = "Audytorzy aplikacji" },
    @{ Name = "GG_APP_HospitalAccessControl_Admins";       Description = "Administratorzy aplikacji" }
)

$AdminGroups = @(
    @{ Name = "GG_HOSP_ADMIN_DomainAdmins_Delegated"; Description = "Delegowana administracja domenowa" },
    @{ Name = "GG_HOSP_ADMIN_SQL_Admins";             Description = "Administratorzy SQL Server" },
    @{ Name = "GG_HOSP_ADMIN_App_Admins";             Description = "Administratorzy aplikacji" },
    @{ Name = "GG_HOSP_ADMIN_Workstation_Admins";     Description = "Lokalni administratorzy stacji" },
    @{ Name = "GG_HOSP_ADMIN_Server_Admins";          Description = "Lokalni administratorzy serwerów" }
)

foreach ($g in $RoleGroups)       { New-HospitalGroup -Name $g.Name -Path $RoleGroupsOU       -Description $g.Description }
foreach ($g in $DepartmentGroups) { New-HospitalGroup -Name $g.Name -Path $DepartmentGroupsOU -Description $g.Description }
foreach ($g in $SqlGroups)        { New-HospitalGroup -Name $g.Name -Path $SqlAccessGroupsOU  -Description $g.Description }
foreach ($g in $AppGroups)        { New-HospitalGroup -Name $g.Name -Path $AppAccessGroupsOU  -Description $g.Description }
foreach ($g in $AdminGroups)      { New-HospitalGroup -Name $g.Name -Path $AdminGroupsOU      -Description $g.Description }

Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Doctor"       -ChildGroup "GG_HOSP_ROLE_Doctor"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_MedicalStaff" -ChildGroup "GG_HOSP_ROLE_Doctor"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Users"        -ChildGroup "GG_HOSP_ROLE_Doctor"

Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Nurse"        -ChildGroup "GG_HOSP_ROLE_Nurse"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_MedicalStaff" -ChildGroup "GG_HOSP_ROLE_Nurse"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Users"        -ChildGroup "GG_HOSP_ROLE_Nurse"

Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Registration" -ChildGroup "GG_HOSP_ROLE_Registration"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Registration" -ChildGroup "GG_HOSP_ROLE_Registration"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Users"        -ChildGroup "GG_HOSP_ROLE_Registration"

Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Manager"      -ChildGroup "GG_HOSP_ROLE_DepartmentManager"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_MedicalStaff" -ChildGroup "GG_HOSP_ROLE_DepartmentManager"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Users"        -ChildGroup "GG_HOSP_ROLE_DepartmentManager"

Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Auditor"      -ChildGroup "GG_HOSP_ROLE_Auditor"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Auditors"     -ChildGroup "GG_HOSP_ROLE_Auditor"
Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Users"        -ChildGroup "GG_HOSP_ROLE_Auditor"

Add-HospitalGroupMember -ParentGroup "GG_APP_HospitalAccessControl_Admins"       -ChildGroup "GG_HOSP_ROLE_ITAdministrator"
Add-HospitalGroupMember -ParentGroup "GG_HOSP_ADMIN_App_Admins"                  -ChildGroup "GG_HOSP_ROLE_ITAdministrator"
Add-HospitalGroupMember -ParentGroup "GG_SQL_HospitalAccessControl_Admin"        -ChildGroup "GG_HOSP_ADMIN_SQL_Admins"

Get-ADGroup -SearchBase $GroupsRootOU -Filter * -Properties Description, GroupScope, GroupCategory |
Select-Object Name, GroupScope, GroupCategory, Description, DistinguishedName |
Sort-Object Name |
Format-Table -AutoSize
