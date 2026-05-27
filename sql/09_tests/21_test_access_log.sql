USE [HospitalAccessControlDb_Dev];
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    ActionCode,
    ObjectName,
    WasSuccessful,
    AdditionalInfo,
    AccessDate,
    ClientHost,
    ApplicationName
FROM audit.AccessLog
ORDER BY AccessLogId DESC;
GO

SELECT
    DomainLogin,
    WasSuccessful,
    COUNT(*) AS EventsCount
FROM audit.AccessLog
GROUP BY
    DomainLogin,
    WasSuccessful
ORDER BY
    DomainLogin,
    WasSuccessful;
GO

SELECT TOP (50)
    AccessLogId,
    DomainLogin,
    PatientId,
    ActionCode,
    AdditionalInfo,
    AccessDate
FROM audit.AccessLog
WHERE WasSuccessful = 0
ORDER BY AccessLogId DESC;
GO