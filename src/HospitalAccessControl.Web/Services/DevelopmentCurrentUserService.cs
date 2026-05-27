using HospitalAccessControl.Application.Common.Security;
using Microsoft.Extensions.Options;

namespace HospitalAccessControl.Web.Services;

public sealed class DevelopmentCurrentUserService : ICurrentUserService
{
    private readonly DevelopmentUserOptions _options;

    public DevelopmentCurrentUserService(IOptions<DevelopmentUserOptions> options)
    {
        _options = options.Value;
    }

    public CurrentUserDto GetCurrentUser()
    {
        if (string.IsNullOrWhiteSpace(_options.DomainLogin))
        {
            return new CurrentUserDto
            {
                DomainLogin = string.Empty,
                SamAccountName = string.Empty,
                DisplayName = "Anonymous",
                IsAuthenticated = false
            };
        }

        return new CurrentUserDto
        {
            DomainLogin = _options.DomainLogin,
            SamAccountName = _options.SamAccountName,
            DisplayName = _options.DisplayName,
            IsAuthenticated = true
        };
    }
}