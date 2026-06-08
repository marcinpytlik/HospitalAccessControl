USE [master];
GO

IF EXISTS
(
    SELECT 1
    FROM sys.server_file_audits
    WHERE name = N'HospitalAccessControl_ServerAudit'
)
BEGIN
    ALTER SERVER AUDIT HospitalAccessControl_ServerAudit
    WITH (STATE = OFF);

    DROP SERVER AUDIT HospitalAccessControl_ServerAudit;
END
GO

CREATE SERVER AUDIT HospitalAccessControl_ServerAudit
TO FILE
(
    FILEPATH = N'C:\SqlAudit\HospitalAccessControl\',
    MAXSIZE = 100 MB,
    MAX_ROLLOVER_FILES = 10,
    RESERVE_DISK_SPACE = OFF
)
WITH
(
    QUEUE_DELAY = 1000,
    ON_FAILURE = CONTINUE
);
GO

ALTER SERVER AUDIT HospitalAccessControl_ServerAudit
WITH (STATE = ON);
GO

USE [HospitalAccessControlDb_Dev];
GO

IF EXISTS
(
    SELECT 1
    FROM sys.database_audit_specifications
    WHERE name = N'HospitalAccessControl_DatabaseAuditSpec'
)
BEGIN
    ALTER DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec
    WITH (STATE = OFF);

    DROP DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec;
END
GO

CREATE DATABASE AUDIT SPECIFICATION HospitalAccessControl_DatabaseAuditSpec
FOR SERVER AUDIT HospitalAccessControl_ServerAudit
ADD (SELECT ON OBJECT::medical.Patients BY public),
ADD (SELECT ON OBJECT::medical.MedicalRecords BY public),
ADD (DATABASE_PERMISSION_CHANGE_GROUP),
ADD (DATABASE_ROLE_MEMBER_CHANGE_GROUP),
ADD (SCHEMA_OBJECT_PERMISSION_CHANGE_GROUP)
WITH (STATE = ON);
GO
