using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Security;

public sealed class UserRoleReadService : IUserRoleReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public UserRoleReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<string>> GetRoleCodesAsync(
        string domainLogin,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainLogin))
        {
            return Array.Empty<string>();
        }

        var now = DateTime.UtcNow;

        return await _dbContext.UserRoleAssignments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.ValidFrom <= now)
            .Where(x => x.ValidTo == null || x.ValidTo >= now)
            .Where(x => x.ApplicationUser.IsActive)
            .Where(x => x.ApplicationUser.DomainLogin == domainLogin)
            .Where(x => x.ApplicationRole.IsActive)
            .Select(x => x.ApplicationRole.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }
}
