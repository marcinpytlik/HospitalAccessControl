namespace HospitalAccessControl.Application.Audit;

public sealed class AccessLogCreateDto
{
    public string DomainLogin { get; init; } = string.Empty;

    public int? PatientId { get; init; }

    public string ActionCode { get; init; } = string.Empty;

    public string ObjectName { get; init; } = string.Empty;

    public string? ClientHost { get; init; }

    public string? ApplicationName { get; init; }

    public bool WasSuccessful { get; init; }

    public string? AdditionalInfo { get; init; }
}