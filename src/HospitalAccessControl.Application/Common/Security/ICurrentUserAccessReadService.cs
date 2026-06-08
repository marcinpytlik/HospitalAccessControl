namespace HospitalAccessControl.Application.Common.Security;

public interface ICurrentUserAccessReadService
{
    Task<CurrentUserAccessDto> GetAsync(
        string domainLogin,
        CancellationToken cancellationToken = default);
}
