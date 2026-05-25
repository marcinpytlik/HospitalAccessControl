# Architektura środowiska — HospitalAccessControl

## Domena

```text
hospital.local
```

## Adresacja

```text
Sieć: 192.168.50.0/24
Brama: 192.168.50.1
DNS: 192.168.50.10
```

## Maszyny

| Maszyna | IP | System | Rola |
|---|---:|---|---|
| `DC01` | `192.168.50.10` | Windows Server 2022 | Active Directory + DNS |
| `SQL01` | `192.168.50.20` | Windows Server 2022 | SQL Server 2022 Developer |
| `APP01` | `192.168.50.30` | Windows Server 2022 | IIS + .NET 10 |
| `CLIENT01` | `192.168.50.40` | Windows 11 | Klient testowy |

## Diagram logiczny

```mermaid
flowchart LR
    CLIENT01[CLIENT01<br/>Windows 11] --> APP01[APP01<br/>IIS + .NET 10]
    APP01 --> SQL01[SQL01<br/>SQL Server 2022]
    CLIENT01 --> DC01[DC01<br/>AD DS + DNS]
    APP01 --> DC01
    SQL01 --> DC01
```

## Przepływ dostępu

```mermaid
sequenceDiagram
    participant U as Użytkownik domenowy
    participant C as CLIENT01
    participant A as APP01 / .NET 10
    participant S as SQL01 / SQL Server
    participant D as DC01 / AD

    U->>C: Logowanie domenowe
    C->>D: Uwierzytelnienie
    U->>A: Otwarcie aplikacji
    A->>D: Windows Authentication
    A->>S: Zapytanie do bazy
    S->>S: RBAC + RLS
    S-->>A: Dane ograniczone do oddziału
    A-->>U: Widok pacjentów
```
