namespace HospitalAccessControl.Application.Common.Security;

public interface IUserRoleReadService
{
    Task<IReadOnlyList<string>> GetRoleCodesAsync(
        string domainLogin,
        CancellationToken cancellationToken = default);
}
