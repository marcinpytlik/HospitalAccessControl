using HospitalAccessControl.Application.Common.Security;

namespace HospitalAccessControl.Web.Services;

public sealed class WindowsCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WindowsCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserDto GetCurrentUser()
    {
        var identityName = _httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .Name;

        if (string.IsNullOrWhiteSpace(identityName))
        {
            return new CurrentUserDto
            {
                DomainLogin = string.Empty,
                SamAccountName = string.Empty,
                DisplayName = "Anonymous",
                IsAuthenticated = false,
                Roles = Array.Empty<string>()
            };
        }

        var samAccountName = identityName.Contains('\\')
            ? identityName.Split('\\')[1]
            : identityName;

        return new CurrentUserDto
        {
            DomainLogin = identityName,
            SamAccountName = samAccountName,
            DisplayName = identityName,
            IsAuthenticated = true,
            Roles = Array.Empty<string>()
        };
    }
}
