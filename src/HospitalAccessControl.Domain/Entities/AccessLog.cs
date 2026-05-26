namespace HospitalAccessControl.Domain.Entities;

public sealed class AccessLog
{
    public long AccessLogId { get; set; }

    public string DomainLogin { get; set; } = string.Empty;

    public int? PatientId { get; set; }

    public Patient? Patient { get; set; }

    public string ActionCode { get; set; } = string.Empty;

    public string ObjectName { get; set; } = string.Empty;

    public DateTime AccessDate { get; set; }

    public string? ClientHost { get; set; }

    public string? ApplicationName { get; set; }

    public bool WasSuccessful { get; set; }

    public string? AdditionalInfo { get; set; }
}