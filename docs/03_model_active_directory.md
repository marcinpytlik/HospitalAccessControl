# Model Active Directory

## Struktura OU

```text
hospital.local
└── OU=Hospital
    ├── OU=Users
    │   ├── OU=Doctors
    │   ├── OU=Nurses
    │   ├── OU=Registration
    │   ├── OU=DepartmentManagers
    │   ├── OU=Auditors
    │   └── OU=IT
    ├── OU=Groups
    │   ├── OU=RoleGroups
    │   ├── OU=DepartmentGroups
    │   ├── OU=SqlAccessGroups
    │   ├── OU=AppAccessGroups
    │   └── OU=AdminGroups
    ├── OU=Servers
    │   ├── OU=DomainControllers
    │   ├── OU=SqlServers
    │   ├── OU=ApplicationServers
    │   └── OU=ManagementServers
    ├── OU=Workstations
    ├── OU=ServiceAccounts
    └── OU=AdminAccounts
```

## Konta usługowe

| Konto | Przeznaczenie |
|---|---|
| `HOSPITAL\svc_sql_engine` | SQL Server Database Engine |
| `HOSPITAL\svc_sql_agent` | SQL Server Agent |
| `HOSPITAL\svc_hac_app` | IIS AppPool / aplikacja |
| `HOSPITAL\svc_hac_migr` | Migracje EF Core |
| `HOSPITAL\svc_sql_backup` | Operacje backupowe |
| `HOSPITAL\svc_sql_monitor` | Monitoring SQL Server |

## Użytkownicy testowi

| Konto | Rola | Oddział |
|---|---|---|
| `doctor.cardio` | Doctor | Cardiology |
| `doctor.ortho` | Doctor | Orthopedics |
| `doctor.neuro` | Doctor | Neurology |
| `nurse.cardio` | Nurse | Cardiology |
| `nurse.ortho` | Nurse | Orthopedics |
| `nurse.ped` | Nurse | Pediatrics |
| `registration.user` | Registration | ogólny |
| `registration.emer` | Registration | Emergency |
| `manager.cardio` | DepartmentManager | Cardiology |
| `manager.ortho` | DepartmentManager | Orthopedics |
| `auditor.user` | Auditor | audyt |
| `it.admin` | ITAdministrator | IT |
| `sql.admin` | SQL Admin | IT |
| `app.admin` | App Admin | IT |
