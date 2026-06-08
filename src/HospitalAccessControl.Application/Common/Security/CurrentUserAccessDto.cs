namespace HospitalAccessControl.Application.Common.Security;

public sealed class CurrentUserAccessDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Departments { get; init; } = Array.Empty<string>();
}
