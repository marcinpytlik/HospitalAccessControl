USE [master];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.sql_logins
    WHERE name = N'hac_mask_test'
)
BEGIN
    CREATE LOGIN hac_mask_test
    WITH PASSWORD = 'Str0ng!Password123',
         CHECK_POLICY = OFF;
END
GO

USE [HospitalAccessControlDb_Dev];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'hac_mask_test'
)
BEGIN
    CREATE USER hac_mask_test FOR LOGIN hac_mask_test;
END
GO

GRANT SELECT ON medical.Patients TO hac_mask_test;
GRANT SELECT ON dictionary.Departments TO hac_mask_test;
GO

EXECUTE AS USER = 'hac_mask_test';

SELECT TOP (10)
    PatientId,
    MedicalNumber,
    FirstName,
    LastName,
    Pesel
FROM medical.Patients
ORDER BY PatientId;

REVERT;
GO
