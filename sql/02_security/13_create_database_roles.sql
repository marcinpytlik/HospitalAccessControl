USE [HospitalAccessControlDb_Dev];
GO

IF DATABASE_PRINCIPAL_ID(N'db_hac_app_runtime') IS NULL CREATE ROLE db_hac_app_runtime;
IF DATABASE_PRINCIPAL_ID(N'db_hac_migration') IS NULL CREATE ROLE db_hac_migration;
IF DATABASE_PRINCIPAL_ID(N'db_hac_auditor') IS NULL CREATE ROLE db_hac_auditor;
IF DATABASE_PRINCIPAL_ID(N'db_hac_monitoring') IS NULL CREATE ROLE db_hac_monitoring;
GO
