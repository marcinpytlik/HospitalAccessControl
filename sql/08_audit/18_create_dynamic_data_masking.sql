USE [HospitalAccessControlDb_Dev];
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.masked_columns
    WHERE object_id = OBJECT_ID(N'medical.Patients')
      AND name = N'Pesel'
      AND is_masked = 1
)
BEGIN
    ALTER TABLE medical.Patients
    ALTER COLUMN Pesel
        ADD MASKED WITH (FUNCTION = 'partial(0,"XXXXXXX",4)');
END
GO

SELECT
    c.name,
    c.is_masked,
    c.masking_function
FROM sys.masked_columns AS c
WHERE c.object_id = OBJECT_ID(N'medical.Patients');
GO
