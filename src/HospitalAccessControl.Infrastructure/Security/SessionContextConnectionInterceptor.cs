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