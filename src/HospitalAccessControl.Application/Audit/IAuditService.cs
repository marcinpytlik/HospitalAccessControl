namespace HospitalAccessControl.Application.Audit;

public interface IAuditService
{
    Task LogAccessAsync(
        AccessLogCreateDto accessLog,
        CancellationToken cancellationToken = default);
}