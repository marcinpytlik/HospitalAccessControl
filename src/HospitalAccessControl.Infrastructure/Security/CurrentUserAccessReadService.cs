using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Security;

public sealed class CurrentUserAccessReadService : ICurrentUserAccessReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public CurrentUserAccessReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CurrentUserAccessDto> GetAsync(
        string domainLogin,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainLogin))
        {
            return new CurrentUserAccessDto();
        }

        var now = DateTime.UtcNow;

        var roles = await _dbContext.UserRoleAssignments
            .AsNoTracking()
            .Where(x => x.ApplicationUser.DomainLogin == domainLogin)
            .Where(x => x.ApplicationUser.IsActive)
            .Where(x => x.ApplicationRole.IsActive)
            .Where(x => x.IsActive)
            .Where(x => x.ValidFrom <= now)
            .Where(x => x.ValidTo == null || x.ValidTo >= now)
            .Select(x => x.ApplicationRole.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var departments = await _dbContext.UserDepartmentAccesses
            .AsNoTracking()
            .Where(x => x.ApplicationUser.DomainLogin == domainLogin)
            .Where(x => x.ApplicationUser.IsActive)
            .Where(x => x.Department.IsActive)
            .Where(x => x.IsActive)
            .Where(x => x.ValidFrom <= now)
            .Where(x => x.ValidTo == null || x.ValidTo >= now)
            .Select(x => x.Department.Code + " — " + x.Department.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        return new CurrentUserAccessDto
        {
            DomainLogin = domainLogin,
            Roles = roles,
            Departments = departments
        };
    }
}
