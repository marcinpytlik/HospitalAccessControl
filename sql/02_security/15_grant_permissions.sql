USE [HospitalAccessControlDb_Dev];
GO

GRANT SELECT ON SCHEMA::dictionary TO db_hac_app_runtime;
GRANT SELECT ON SCHEMA::security TO db_hac_app_runtime;
GRANT SELECT ON SCHEMA::medical TO db_hac_app_runtime;
GRANT INSERT ON audit.AccessLog TO db_hac_app_runtime;
GRANT SELECT ON audit.AccessLog TO db_hac_auditor;
GO

ALTER ROLE db_ddladmin ADD MEMBER [HOSPITAL\svc_hac_migr];
GO
