USE [HospitalAccessControlDb_Dev];
GO

IF COL_LENGTH(N'audit.AccessLog', N'RequestedPatientId') IS NULL
BEGIN
    ALTER TABLE audit.AccessLog
        ADD RequestedPatientId int NULL;
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AccessLog_RequestedPatientId'
      AND object_id = OBJECT_ID(N'audit.AccessLog')
)
BEGIN
    CREATE INDEX IX_AccessLog_RequestedPatientId
        ON audit.AccessLog(RequestedPatientId);
END
GO
