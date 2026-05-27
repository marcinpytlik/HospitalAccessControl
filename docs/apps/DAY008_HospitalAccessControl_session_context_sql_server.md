# DAY008 — HospitalAccessControl  
## SESSION_CONTEXT dla SQL Server — przekazanie użytkownika aplikacyjnego do bazy

## Cel dnia

Celem DAY008 jest przygotowanie mechanizmu przekazywania aktualnego użytkownika aplikacji do SQL Server przez:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

Dzięki temu baza danych będzie wiedziała, dla jakiego użytkownika ma wykonywać zapytania.

To jest fundament pod:

```text
DAY009 — Row-Level Security
```

---

## Efekt końcowy DAY008

Po zakończeniu tego dnia aplikacja będzie:

- odczytywać aktualnego użytkownika przez `ICurrentUserService`,
- otwierać połączenie do SQL Server przez EF Core,
- ustawiać w sesji SQL Server wartość:

```text
CurrentUser = HOSPITAL\doctor.cardio
```

- robić to automatycznie przy otwieraniu połączenia,
- umożliwiać późniejsze filtrowanie danych przez RLS.

---

## Dlaczego używamy SESSION_CONTEXT?

W środowisku lokalnym nie mamy jeszcze Active Directory.

Aplikacja działa jako proces developerski, ale chcemy testować scenariusze tak, jakby użytkownikiem był:

```text
HOSPITAL\doctor.cardio
```

albo:

```text
HOSPITAL\doctor.ortho
```

Dlatego przekazujemy użytkownika aplikacyjnego do SQL Server przez `SESSION_CONTEXT`.

Później RLS będzie sprawdzał:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

zamiast polegać wyłącznie na:

```sql
SUSER_SNAME()
```

---

## Ważna decyzja projektowa

W tej aplikacji przyjmujemy model:

```text
Aplikacja rozpoznaje użytkownika
        ↓
Aplikacja ustawia SESSION_CONTEXT w SQL Server
        ↓
SQL Server RLS filtruje dane według SESSION_CONTEXT
```

To pozwala pracować lokalnie bez AD, a jednocześnie zachować bardzo dobrą architekturę pod środowisko labowe.

---

# 1. Jak działa SESSION_CONTEXT?

SQL Server pozwala zapisać wartość w kontekście sesji połączenia.

Przykład:

```sql
EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.cardio';
```

Potem w tej samej sesji można odczytać:

```sql
SELECT SESSION_CONTEXT(N'CurrentUser') AS CurrentUser;
```

Wynik:

```text
HOSPITAL\doctor.cardio
```

---

# 2. Po co to aplikacji?

W DAY007 zrobiliśmy:

```text
DevelopmentCurrentUserService
```

który zwraca np.:

```text
HOSPITAL\doctor.cardio
```

W DAY008 chcemy tę wartość przekazać do SQL Server.

W DAY009 zrobimy RLS, który użyje tej wartości do filtrowania pacjentów.

---

# Krok 1 — przejście do katalogu projektu

Otwórz PowerShell i przejdź do katalogu projektu:

```powershell
Set-Location C:\Projects\HospitalAccessControl
```

Sprawdź lokalizację:

```powershell
Get-Location
```

Oczekiwany wynik:

```text
C:\Projects\HospitalAccessControl
```

Sprawdź build po DAY007:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 2 — utworzenie katalogu Security w Infrastructure

Tworzymy katalog:

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Security
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure
```

Powinieneś zobaczyć katalog:

```text
Security
```

---

# Krok 3 — utworzenie pliku interceptora

Tworzymy plik:

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Security\SessionContextConnectionInterceptor.cs
```

Sprawdź:

```powershell
Get-ChildItem .\src\HospitalAccessControl.Infrastructure\Security
```

Powinieneś zobaczyć:

```text
SessionContextConnectionInterceptor.cs
```

---

# Krok 4 — SessionContextConnectionInterceptor

## Plik

```text
src\HospitalAccessControl.Infrastructure\Security\SessionContextConnectionInterceptor.cs
```

## Zawartość

Wklej:

```csharp
using System.Data.Common;
using HospitalAccessControl.Application.Common.Security;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HospitalAccessControl.Infrastructure.Security;

public sealed class SessionContextConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public SessionContextConnectionInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetSessionContext(connection);

        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await SetSessionContextAsync(connection, cancellationToken);

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void SetSessionContext(DbConnection connection)
    {
        var currentUser = _currentUserService.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.DomainLogin))
        {
            return;
        }

        using var command = connection.CreateCommand();

        command.CommandText = """
            EXEC sys.sp_set_session_context
                @key = N'CurrentUser',
                @value = @CurrentUser,
                @read_only = 0;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@CurrentUser";
        parameter.Value = currentUser.DomainLogin;
        command.Parameters.Add(parameter);

        command.ExecuteNonQuery();
    }

    private async Task SetSessionContextAsync(
        DbConnection connection,
        CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetCurrentUser();

        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.DomainLogin))
        {
            return;
        }

        await using var command = connection.CreateCommand();

        command.CommandText = """
            EXEC sys.sp_set_session_context
                @key = N'CurrentUser',
                @value = @CurrentUser,
                @read_only = 0;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@CurrentUser";
        parameter.Value = currentUser.DomainLogin;
        command.Parameters.Add(parameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
```

---

## Dlaczego `@read_only = 0`?

W SQL Server `sp_set_session_context` pozwala ustawić wartość jako tylko do odczytu:

```sql
@read_only = 1
```

Ale w aplikacjach używających connection poolingu może to utrudnić ponowne użycie połączenia dla innego użytkownika.

Na etapie developerskim ustawiamy:

```sql
@read_only = 0
```

Dzięki temu przy każdym otwarciu połączenia aplikacja może ustawić aktualnego użytkownika.

W dokumentacji pracy można opisać:

> W środowisku developerskim użyto `@read_only = 0`, aby umożliwić przełączanie symulowanych użytkowników podczas testów. W środowisku produkcyjnym decyzja o użyciu `@read_only = 1` wymaga uwzględnienia connection poolingu oraz sposobu zarządzania połączeniami.

---

# Krok 5 — rejestracja interceptora w DI

Otwórz plik:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Na górze dodaj:

```csharp
using HospitalAccessControl.Infrastructure.Security;
```

Teraz trzeba zarejestrować interceptor.

Znajdź metodę:

```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
```

Dodaj przed `services.AddDbContext`:

```csharp
services.AddScoped<SessionContextConnectionInterceptor>();
```

Następnie zmień konfigurację `AddDbContext`, aby dodać interceptor.

## Docelowa zawartość pliku

Plik powinien wyglądać tak:

```csharp
using HospitalAccessControl.Infrastructure.Data;
using HospitalAccessControl.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalAccessControl.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HospitalAccessControlDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'HospitalAccessControlDb' was not found.");
        }

        services.AddScoped<SessionContextConnectionInterceptor>();

        services.AddDbContext<HospitalAccessControlDbContext>((serviceProvider, options) =>
        {
            var sessionContextInterceptor =
                serviceProvider.GetRequiredService<SessionContextConnectionInterceptor>();

            options.UseSqlServer(connectionString);
            options.AddInterceptors(sessionContextInterceptor);
        });

        return services;
    }
}
```

---

## Co tu się zmieniło?

W DAY003 mieliśmy prostą konfigurację:

```csharp
services.AddDbContext<HospitalAccessControlDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
```

Teraz potrzebujemy `serviceProvider`, żeby pobrać interceptor z DI:

```csharp
services.AddDbContext<HospitalAccessControlDbContext>((serviceProvider, options) =>
{
    var sessionContextInterceptor =
        serviceProvider.GetRequiredService<SessionContextConnectionInterceptor>();

    options.UseSqlServer(connectionString);
    options.AddInterceptors(sessionContextInterceptor);
});
```

---

# Krok 6 — build po dodaniu interceptora

Wykonaj:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 7 — przygotowanie serwisu testowego

Na razie nie mamy jeszcze strony `/Patients`, ale chcemy łatwo sprawdzić, czy `SESSION_CONTEXT` działa.

Dodamy prostą usługę diagnostyczną w `Application` i implementację w `Infrastructure`.

---

## Utworzenie katalogów

```powershell
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Application\Diagnostics
New-Item -ItemType Directory -Force .\src\HospitalAccessControl.Infrastructure\Diagnostics
```

## Utworzenie plików

```powershell
New-Item -ItemType File -Force .\src\HospitalAccessControl.Application\Diagnostics\ISqlSessionContextDiagnostics.cs

New-Item -ItemType File -Force .\src\HospitalAccessControl.Infrastructure\Diagnostics\SqlSessionContextDiagnostics.cs
```

---

# Krok 8 — interfejs diagnostyczny

## Plik

```text
src\HospitalAccessControl.Application\Diagnostics\ISqlSessionContextDiagnostics.cs
```

## Zawartość

Wklej:

```csharp
namespace HospitalAccessControl.Application.Diagnostics;

public interface ISqlSessionContextDiagnostics
{
    Task<string?> GetCurrentUserFromSessionContextAsync(
        CancellationToken cancellationToken = default);
}
```

---

# Krok 9 — implementacja diagnostyczna

## Plik

```text
src\HospitalAccessControl.Infrastructure\Diagnostics\SqlSessionContextDiagnostics.cs
```

## Zawartość

Wklej:

```csharp
using HospitalAccessControl.Application.Diagnostics;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Diagnostics;

public sealed class SqlSessionContextDiagnostics : ISqlSessionContextDiagnostics
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public SqlSessionContextDiagnostics(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetCurrentUserFromSessionContextAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Database
            .SqlQueryRaw<string>("SELECT CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser'))")
            .SingleOrDefaultAsync(cancellationToken);
    }
}
```

---

## Co robi ta klasa?

Wykonuje zapytanie:

```sql
SELECT CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser'))
```

i zwraca wartość ustawioną przez interceptor.

Czyli powinna zwrócić:

```text
HOSPITAL\doctor.cardio
```

---

# Krok 10 — rejestracja diagnostyki w DI

Otwórz:

```text
src\HospitalAccessControl.Infrastructure\DependencyInjection\ServiceCollectionExtensions.cs
```

Dodaj na górze:

```csharp
using HospitalAccessControl.Application.Diagnostics;
using HospitalAccessControl.Infrastructure.Diagnostics;
```

W metodzie `AddInfrastructure`, przed `return services;`, dodaj:

```csharp
services.AddScoped<ISqlSessionContextDiagnostics, SqlSessionContextDiagnostics>();
```

Docelowo końcówka metody powinna zawierać:

```csharp
services.AddScoped<ISqlSessionContextDiagnostics, SqlSessionContextDiagnostics>();

return services;
```

---

# Krok 11 — dodanie diagnostyki na stronie głównej

Teraz pokażemy na stronie głównej dwie wartości:

```text
użytkownik z aplikacji
użytkownik z SQL SESSION_CONTEXT
```

---

## Plik PageModel

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Index.cshtml.cs
```

Zastąp zawartość:

```csharp
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISqlSessionContextDiagnostics _sqlSessionContextDiagnostics;

    public IndexModel(
        ICurrentUserService currentUserService,
        ISqlSessionContextDiagnostics sqlSessionContextDiagnostics)
    {
        _currentUserService = currentUserService;
        _sqlSessionContextDiagnostics = sqlSessionContextDiagnostics;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public string? SqlSessionCurrentUser { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        SqlSessionCurrentUser =
            await _sqlSessionContextDiagnostics.GetCurrentUserFromSessionContextAsync(cancellationToken);
    }
}
```

---

## Plik widoku

Otwórz:

```text
src\HospitalAccessControl.Web\Pages\Index.cshtml
```

Zastąp zawartość:

```cshtml
@page
@model IndexModel

@{
    ViewData["Title"] = "HospitalAccessControl";
}

<div class="text-center">
    <h1 class="display-4">HospitalAccessControl</h1>
    <p>Demonstracyjna aplikacja kontroli dostępu do danych medycznych.</p>
</div>

<hr />

<h2>Aktualny użytkownik DEV</h2>

<table class="table">
    <tbody>
        <tr>
            <th>IsAuthenticated</th>
            <td>@Model.CurrentUser.IsAuthenticated</td>
        </tr>
        <tr>
            <th>DomainLogin</th>
            <td>@Model.CurrentUser.DomainLogin</td>
        </tr>
        <tr>
            <th>SamAccountName</th>
            <td>@Model.CurrentUser.SamAccountName</td>
        </tr>
        <tr>
            <th>DisplayName</th>
            <td>@Model.CurrentUser.DisplayName</td>
        </tr>
    </tbody>
</table>

<hr />

<h2>SQL Server SESSION_CONTEXT</h2>

<table class="table">
    <tbody>
        <tr>
            <th>SESSION_CONTEXT('CurrentUser')</th>
            <td>@Model.SqlSessionCurrentUser</td>
        </tr>
    </tbody>
</table>
```

---

# Krok 12 — build po diagnostyce

Wykonaj:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 13 — uruchomienie aplikacji

Uruchom:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Otwórz adres z terminala, np.:

```text
https://localhost:xxxx
```

Na stronie powinieneś zobaczyć:

```text
Aktualny użytkownik DEV
DomainLogin: HOSPITAL\doctor.cardio
```

oraz:

```text
SQL Server SESSION_CONTEXT
SESSION_CONTEXT('CurrentUser'): HOSPITAL\doctor.cardio
```

To oznacza, że aplikacja poprawnie przekazała użytkownika do SQL Server.

---

# Krok 14 — test zmiany użytkownika

Zatrzymaj aplikację:

```text
CTRL + C
```

Otwórz:

```text
src\HospitalAccessControl.Web\appsettings.Development.json
```

Zmień użytkownika na:

```json
"DevelopmentUser": {
  "DomainLogin": "HOSPITAL\\doctor.ortho",
  "SamAccountName": "doctor.ortho",
  "DisplayName": "Marek Ortopeda"
}
```

Uruchom aplikację ponownie:

```powershell
dotnet run --project .\src\HospitalAccessControl.Web\HospitalAccessControl.Web.csproj
```

Oczekiwany wynik:

```text
DomainLogin: HOSPITAL\doctor.ortho
SESSION_CONTEXT('CurrentUser'): HOSPITAL\doctor.ortho
```

---

# Krok 15 — dodatkowy test przez SQL Profiler / Extended Events

Ten krok jest opcjonalny.

Jeżeli chcesz zobaczyć wywołanie:

```sql
EXEC sys.sp_set_session_context
```

możesz użyć:

```text
SQL Server Profiler
Extended Events
```

Na potrzeby pracy wystarczy jednak test pokazujący wartość:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

na stronie aplikacji.

---

# Krok 16 — test bezpośredni w SQL Server

Możesz samodzielnie sprawdzić, jak działa `SESSION_CONTEXT`.

## Default instance

```powershell
sqlcmd -S localhost -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT SESSION_CONTEXT(N'CurrentUser') AS CurrentUser;"
```

## SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -d HospitalAccessControlDb_Dev -Q "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=N'HOSPITAL\doctor.cardio'; SELECT SESSION_CONTEXT(N'CurrentUser') AS CurrentUser;"
```

Oczekiwany wynik:

```text
HOSPITAL\doctor.cardio
```

---

# Krok 17 — build końcowy

Wykonaj:

```powershell
dotnet build
```

Oczekiwany wynik:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Krok 18 — opcjonalny commit Git

Jeżeli używasz Gita:

```powershell
git status
git add .
git commit -m "DAY008 Add SQL Server session context interceptor"
```

---

# Kontrola końcowa DAY008

Lista kontrolna:

```text
[ ] Utworzono katalog Infrastructure\Security
[ ] Utworzono SessionContextConnectionInterceptor
[ ] Interceptor używa ICurrentUserService
[ ] Interceptor wykonuje sp_set_session_context
[ ] Zarejestrowano interceptor w DI
[ ] Dodano options.AddInterceptors
[ ] Utworzono ISqlSessionContextDiagnostics
[ ] Utworzono SqlSessionContextDiagnostics
[ ] Zarejestrowano diagnostykę w DI
[ ] Index.cshtml.cs odczytuje SESSION_CONTEXT
[ ] Index.cshtml pokazuje SESSION_CONTEXT
[ ] dotnet build kończy się sukcesem
[ ] dotnet run uruchamia aplikację
[ ] Na stronie widać HOSPITAL\doctor.cardio jako użytkownika aplikacji
[ ] Na stronie widać HOSPITAL\doctor.cardio jako SESSION_CONTEXT
[ ] Zmiana użytkownika w appsettings.Development.json zmienia też SESSION_CONTEXT
```

---

# Najczęstsze problemy

## Problem 1 — `ICurrentUserService` nie jest zarejestrowany

Objaw:

```text
Unable to resolve service for type 'ICurrentUserService'
```

Rozwiązanie:

Sprawdź DAY007 i `Program.cs`.

W `Program.cs` powinno być:

```csharp
builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
```

---

## Problem 2 — interceptor nie jest zarejestrowany

Objaw:

```text
Unable to resolve service for type 'SessionContextConnectionInterceptor'
```

Rozwiązanie:

W `ServiceCollectionExtensions.cs` powinno być:

```csharp
services.AddScoped<SessionContextConnectionInterceptor>();
```

oraz:

```csharp
options.AddInterceptors(sessionContextInterceptor);
```

---

## Problem 3 — SESSION_CONTEXT jest pusty

Objaw na stronie:

```text
SESSION_CONTEXT('CurrentUser'):
```

jest pusty.

Sprawdź:

1. Czy `DevelopmentUser.DomainLogin` nie jest pusty.
2. Czy `DevelopmentCurrentUserService` zwraca `IsAuthenticated = true`.
3. Czy `options.AddInterceptors(sessionContextInterceptor)` jest dodane.
4. Czy diagnostyka wykonuje zapytanie przez ten sam `DbContext`.

---

## Problem 4 — błąd `SqlQueryRaw<string>`

Objaw:

```text
The type 'string' is not a supported entity type
```

W nowszych wersjach EF Core `SqlQueryRaw<T>` powinno działać dla typów prostych.  
Jeżeli jednak pojawi się problem, użyj wariantu z ADO.NET.

Zastąp implementację `SqlSessionContextDiagnostics` takim kodem:

```csharp
using System.Data;
using HospitalAccessControl.Application.Diagnostics;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Diagnostics;

public sealed class SqlSessionContextDiagnostics : ISqlSessionContextDiagnostics
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public SqlSessionContextDiagnostics(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetCurrentUserFromSessionContextAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser'))";

        var result = await command.ExecuteScalarAsync(cancellationToken);

        return result as string;
    }
}
```

---

## Problem 5 — błąd connection string

Objaw:

```text
Cannot open database HospitalAccessControlDb_Dev
```

Sprawdź, czy baza istnieje:

### Default instance

```powershell
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases WHERE name = N'HospitalAccessControlDb_Dev';"
```

### SQL Express

```powershell
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT name FROM sys.databases WHERE name = N'HospitalAccessControlDb_Dev';"
```

Jeżeli baza nie istnieje, wróć do DAY004.

---

# Efekt końcowy DAY008

Po zakończeniu DAY008 aplikacja ustawia w SQL Server kontekst aktualnego użytkownika:

```sql
SESSION_CONTEXT(N'CurrentUser')
```

Przykład:

```text
HOSPITAL\doctor.cardio
```

To oznacza, że jesteśmy gotowi do następnego kroku:

```text
DAY009 — Row-Level Security
```

W DAY009 przygotujemy funkcję RLS, która sprawdzi:

```text
SESSION_CONTEXT(N'CurrentUser')
        ↓
security.ApplicationUsers
        ↓
security.UserDepartmentAccess
        ↓
medical.Patients.DepartmentId
```

i zwróci tylko pacjentów z dozwolonego oddziału.

---

# Krótkie podsumowanie dla dokumentacji pracy

W ramach ósmego etapu implementacji przygotowano mechanizm przekazywania tożsamości aktualnego użytkownika aplikacji do SQL Server za pomocą `SESSION_CONTEXT`. Utworzono interceptor połączenia Entity Framework Core, który po otwarciu połączenia ustawia wartość `CurrentUser` w kontekście sesji SQL Server na podstawie `ICurrentUserService`. Dodano również prostą diagnostykę pozwalającą potwierdzić, że wartość ustawiona przez aplikację jest widoczna po stronie SQL Server. Mechanizm ten stanowi bezpośrednią podstawę do implementacji Row-Level Security w kolejnym etapie.
