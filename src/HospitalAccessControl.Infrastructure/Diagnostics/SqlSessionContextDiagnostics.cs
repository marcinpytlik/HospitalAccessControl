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
    .SqlQueryRaw<string>(
        "SELECT CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS [Value]")
    .SingleOrDefaultAsync(cancellationToken);
    }
}