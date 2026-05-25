#requires -RunAsAdministrator

<#
.SYNOPSIS
    Tworzy użytkowników testowych dla projektu HospitalAccessControl.
#>

$ErrorActionPreference = "Stop"
Import-Module ActiveDirectory

$DomainDN       = "DC=hospital,DC=local"
$DomainName     = "hospital.local"
$DomainNetBIOS  = "HOSPITAL"
$RootOU = "OU=Hospital,$DomainDN"

$DoctorsOU            = "OU=Doctors,OU=Users,$RootOU"
$NursesOU             = "OU=Nurses,OU=Users,$RootOU"
$RegistrationOU       = "OU=Registration,OU=Users,$RootOU"
$DepartmentManagersOU = "OU=DepartmentManagers,OU=Users,$RootOU"
$AuditorsOU           = "OU=Auditors,OU=Users,$RootOU"
$ITOU                 = "OU=IT,OU=Users,$RootOU"

Write-Host ""
Write-Host "Podaj hasło dla użytkowników testowych." -ForegroundColor Yellow
$UserPassword = Read-Host -Prompt "Hasło dla użytkowników testowych" -AsSecureString

$Users = @(
    @{ SamAccountName = "doctor.cardio";     GivenName = "Jan";      Surname = "Kardiolog";    DisplayName = "Jan Kardiolog";                    Description = "Lekarz oddziału kardiologii"; Path = $DoctorsOU;            Groups = @("GG_HOSP_ROLE_Doctor","GG_HOSP_DEPT_Cardiology") },
    @{ SamAccountName = "doctor.ortho";      GivenName = "Marek";    Surname = "Ortopeda";     DisplayName = "Marek Ortopeda";                  Description = "Lekarz oddziału ortopedii";    Path = $DoctorsOU;            Groups = @("GG_HOSP_ROLE_Doctor","GG_HOSP_DEPT_Orthopedics") },
    @{ SamAccountName = "doctor.neuro";      GivenName = "Tomasz";   Surname = "Neurolog";     DisplayName = "Tomasz Neurolog";                 Description = "Lekarz oddziału neurologii";   Path = $DoctorsOU;            Groups = @("GG_HOSP_ROLE_Doctor","GG_HOSP_DEPT_Neurology") },
    @{ SamAccountName = "nurse.cardio";      GivenName = "Anna";     Surname = "Pielegniarka"; DisplayName = "Anna Pielęgniarka Kardiologia";   Description = "Pielęgniarka kardiologii";     Path = $NursesOU;             Groups = @("GG_HOSP_ROLE_Nurse","GG_HOSP_DEPT_Cardiology") },
    @{ SamAccountName = "nurse.ortho";       GivenName = "Ewa";      Surname = "Pielegniarka"; DisplayName = "Ewa Pielęgniarka Ortopedia";       Description = "Pielęgniarka ortopedii";       Path = $NursesOU;             Groups = @("GG_HOSP_ROLE_Nurse","GG_HOSP_DEPT_Orthopedics") },
    @{ SamAccountName = "nurse.ped";         GivenName = "Karolina"; Surname = "Pielegniarka"; DisplayName = "Karolina Pielęgniarka Pediatria"; Description = "Pielęgniarka pediatrii";       Path = $NursesOU;             Groups = @("GG_HOSP_ROLE_Nurse","GG_HOSP_DEPT_Pediatrics") },
    @{ SamAccountName = "registration.user"; GivenName = "Ewa";      Surname = "Rejestracja";  DisplayName = "Ewa Rejestracja";                 Description = "Pracownik rejestracji";        Path = $RegistrationOU;       Groups = @("GG_HOSP_ROLE_Registration") },
    @{ SamAccountName = "registration.emer"; GivenName = "Pawel";    Surname = "IzbaPrzyjec";  DisplayName = "Paweł Izba Przyjęć";              Description = "Rejestracja izby przyjęć";     Path = $RegistrationOU;       Groups = @("GG_HOSP_ROLE_Registration","GG_HOSP_DEPT_Emergency") },
    @{ SamAccountName = "manager.cardio";    GivenName = "Adam";     Surname = "Kierownik";    DisplayName = "Adam Kierownik Kardiologia";      Description = "Kierownik kardiologii";        Path = $DepartmentManagersOU; Groups = @("GG_HOSP_ROLE_DepartmentManager","GG_HOSP_DEPT_Cardiology") },
    @{ SamAccountName = "manager.ortho";     GivenName = "Piotr";    Surname = "Kierownik";    DisplayName = "Piotr Kierownik Ortopedia";       Description = "Kierownik ortopedii";          Path = $DepartmentManagersOU; Groups = @("GG_HOSP_ROLE_DepartmentManager","GG_HOSP_DEPT_Orthopedics") },
    @{ SamAccountName = "auditor.user";      GivenName = "Alicja";   Surname = "Audytor";      DisplayName = "Alicja Audytor";                  Description = "Audytor bezpieczeństwa";       Path = $AuditorsOU;           Groups = @("GG_HOSP_ROLE_Auditor") },
    @{ SamAccountName = "it.admin";          GivenName = "Igor";     Surname = "Administrator";DisplayName = "Igor Administrator IT";           Description = "Administrator IT";              Path = $ITOU;                 Groups = @("GG_HOSP_ROLE_ITAdministrator","GG_HOSP_ADMIN_Server_Admins","GG_HOSP_ADMIN_Workstation_Admins") },
    @{ SamAccountName = "sql.admin";         GivenName = "Sylwia";   Surname = "SQLAdmin";     DisplayName = "Sylwia SQL Administrator";        Description = "Administrator SQL Server";      Path = $ITOU;                 Groups = @("GG_HOSP_ADMIN_SQL_Admins") },
    @{ SamAccountName = "app.admin";         GivenName = "Adrian";   Surname = "AppAdmin";     DisplayName = "Adrian Application Administrator";Description = "Administrator aplikacji";        Path = $ITOU;                 Groups = @("GG_HOSP_ADMIN_App_Admins","GG_HOSP_ROLE_ITAdministrator") }
)

function New-HospitalTestUser {
    param(
        [Parameter(Mandatory = $true)][hashtable]$User,
        [Parameter(Mandatory = $true)][securestring]$Password
    )

    $sam = $User.SamAccountName
    $upn = "$sam@$DomainName"

    $existingUser = Get-ADUser -Filter "SamAccountName -eq '$sam'" -ErrorAction SilentlyContinue

    if ($existingUser) {
        Write-Host "[OK] Użytkownik już istnieje: $DomainNetBIOS\$sam" -ForegroundColor Yellow
    }
    else {
        New-ADUser `
            -SamAccountName $sam `
            -UserPrincipalName $upn `
            -Name $User.DisplayName `
            -GivenName $User.GivenName `
            -Surname $User.Surname `
            -DisplayName $User.DisplayName `
            -Description $User.Description `
            -Path $User.Path `
            -AccountPassword $Password `
            -Enabled $true `
            -PasswordNeverExpires $true `
            -ChangePasswordAtLogon $false

        Write-Host "[UTWORZONO] Użytkownik: $DomainNetBIOS\$sam" -ForegroundColor Green
    }
}

function Add-HospitalUserToGroups {
    param(
        [Parameter(Mandatory = $true)][string]$SamAccountName,
        [Parameter(Mandatory = $true)][string[]]$Groups
    )

    foreach ($groupName in $Groups) {
        $group = Get-ADGroup -Identity $groupName -ErrorAction SilentlyContinue

        if (-not $group) {
            Write-Host "[BŁĄD] Nie znaleziono grupy: $groupName" -ForegroundColor Red
            continue
        }

        $isMember = Get-ADGroupMember -Identity $groupName -Recursive -ErrorAction SilentlyContinue |
            Where-Object { $_.SamAccountName -eq $SamAccountName }

        if ($isMember) {
            Write-Host "[OK] $DomainNetBIOS\$SamAccountName jest już członkiem $groupName" -ForegroundColor Yellow
        }
        else {
            Add-ADGroupMember -Identity $groupName -Members $SamAccountName
            Write-Host "[DODANO] $DomainNetBIOS\$SamAccountName -> $groupName" -ForegroundColor Green
        }
    }
}

foreach ($user in $Users) {
    New-HospitalTestUser -User $user -Password $UserPassword
    Add-HospitalUserToGroups -SamAccountName $user.SamAccountName -Groups $user.Groups
    Write-Host ""
}

Get-ADUser -SearchBase "OU=Users,$RootOU" -Filter * -Properties DisplayName, Description, Enabled |
Select-Object SamAccountName, DisplayName, Enabled, Description, DistinguishedName |
Sort-Object SamAccountName |
Format-Table -AutoSize
