namespace HospitalAccessControl.Application.Common.Security;

public sealed class CurrentUserDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public string SamAccountName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public bool IsAuthenticated { get; init; }
}