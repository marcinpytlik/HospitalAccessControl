namespace HospitalAccessControl.Application.Audit;

public interface IAuditReadService
{
    Task<IReadOnlyList<AccessLogListItemDto>> GetLatestAsync(
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccessLogListItemDto>> GetFailedAttemptsAsync(
        int take = 100,
        CancellationToken cancellationToken = default);
}
