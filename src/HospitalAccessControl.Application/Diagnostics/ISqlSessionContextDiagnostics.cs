namespace HospitalAccessControl.Application.Diagnostics;

public interface ISqlSessionContextDiagnostics
{
    Task<string?> GetCurrentUserFromSessionContextAsync(
        CancellationToken cancellationToken = default);
}