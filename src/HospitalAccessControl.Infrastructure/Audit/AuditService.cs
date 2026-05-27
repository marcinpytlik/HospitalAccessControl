using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Domain.Entities;
using HospitalAccessControl.Infrastructure.Data;

namespace HospitalAccessControl.Infrastructure.Audit;

public sealed class AuditService : IAuditService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public AuditService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAccessAsync(
        AccessLogCreateDto accessLog,
        CancellationToken cancellationToken = default)
    {
        var entity = new AccessLog
        {
            DomainLogin = accessLog.DomainLogin,
            PatientId = accessLog.PatientId,
            ActionCode = accessLog.ActionCode,
            ObjectName = accessLog.ObjectName,
            AccessDate = DateTime.UtcNow,
            ClientHost = accessLog.ClientHost,
            ApplicationName = accessLog.ApplicationName,
            WasSuccessful = accessLog.WasSuccessful,
            AdditionalInfo = accessLog.AdditionalInfo
        };

        _dbContext.AccessLogs.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}