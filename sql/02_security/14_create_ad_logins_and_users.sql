USE [master];
GO

IF SUSER_ID(N'HOSPITAL\svc_hac_app') IS NULL CREATE LOGIN [HOSPITAL\svc_hac_app] FROM WINDOWS;
IF SUSER_ID(N'HOSPITAL\svc_hac_migr') IS NULL CREATE LOGIN [HOSPITAL\svc_hac_migr] FROM WINDOWS;
GO

USE [HospitalAccessControlDb_Dev];
GO

IF USER_ID(N'HOSPITAL\svc_hac_app') IS NULL CREATE USER [HOSPITAL\svc_hac_app] FOR LOGIN [HOSPITAL\svc_hac_app];
IF USER_ID(N'HOSPITAL\svc_hac_migr') IS NULL CREATE USER [HOSPITAL\svc_hac_migr] FOR LOGIN [HOSPITAL\svc_hac_migr];
GO

ALTER ROLE db_hac_app_runtime ADD MEMBER [HOSPITAL\svc_hac_app];
ALTER ROLE db_hac_migration ADD MEMBER [HOSPITAL\svc_hac_migr];
GO
