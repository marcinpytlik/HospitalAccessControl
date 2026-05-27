USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Row-Level Security policy

    Polityka RLS nakłada filtr na tabele medyczne.
*/

IF EXISTS
(
    SELECT 1
    FROM sys.security_policies
    WHERE name = N'HospitalDepartmentSecurityPolicy'
      AND SCHEMA_NAME(schema_id) = N'security'
)
BEGIN
    DROP SECURITY POLICY security.HospitalDepartmentSecurityPolicy;
END
GO

CREATE SECURITY POLICY security.HospitalDepartmentSecurityPolicy
ADD FILTER PREDICATE security.fn_rls_department_access(DepartmentId)
ON medical.Patients,
ADD FILTER PREDICATE security.fn_rls_department_access(DepartmentId)
ON medical.MedicalRecords
WITH (STATE = ON);
GO