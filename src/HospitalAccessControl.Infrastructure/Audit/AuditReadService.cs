using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Audit;

public sealed class AuditReadService : IAuditReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public AuditReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AccessLogListItemDto>> GetLatestAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessLogs
            .AsNoTracking()
            .OrderByDescending(x => x.AccessLogId)
            .Take(take)
            .Select(x => new AccessLogListItemDto
            {
                AccessLogId = x.AccessLogId,
                DomainLogin = x.DomainLogin,
                PatientId = x.PatientId,
                RequestedPatientId = x.RequestedPatientId,
                ActionCode = x.ActionCode,
                ObjectName = x.ObjectName,
                AccessDate = x.AccessDate,
                ClientHost = x.ClientHost,
                ApplicationName = x.ApplicationName,
                WasSuccessful = x.WasSuccessful,
                AdditionalInfo = x.AdditionalInfo
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccessLogListItemDto>> GetFailedAttemptsAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AccessLogs
            .AsNoTracking()
            .Where(x => !x.WasSuccessful)
            .OrderByDescending(x => x.AccessLogId)
            .Take(take)
            .Select(x => new AccessLogListItemDto
            {
                AccessLogId = x.AccessLogId,
                DomainLogin = x.DomainLogin,
                PatientId = x.PatientId,
                RequestedPatientId = x.RequestedPatientId,
                ActionCode = x.ActionCode,
                ObjectName = x.ObjectName,
                AccessDate = x.AccessDate,
                ClientHost = x.ClientHost,
                ApplicationName = x.ApplicationName,
                WasSuccessful = x.WasSuccessful,
                AdditionalInfo = x.AdditionalInfo
            })
            .ToListAsync(cancellationToken);
    }
}
