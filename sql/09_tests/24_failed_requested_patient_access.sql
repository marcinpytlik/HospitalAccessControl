USE [HospitalAccessControlDb_Dev];
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    RequestedPatientId,
    ActionCode,
    WasSuccessful,
    AdditionalInfo,
    AccessDate
FROM audit.AccessLog
WHERE WasSuccessful = 0
ORDER BY AccessLogId DESC;
GO
