USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Testy Row-Level Security oparte o SESSION_CONTEXT
*/

PRINT '========================================';
PRINT 'TEST 1: HOSPITAL\doctor.cardio';
PRINT 'Oczekiwany wynik: tylko CARD / DepartmentId = 1';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.cardio';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;

SELECT TOP (20)
    p.PatientId,
    p.MedicalNumber,
    p.FirstName,
    p.LastName,
    p.DepartmentId
FROM medical.Patients AS p
ORDER BY p.PatientId;
GO

PRINT '========================================';
PRINT 'TEST 2: HOSPITAL\doctor.ortho';
PRINT 'Oczekiwany wynik: tylko ORTH / DepartmentId = 2';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.ortho';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;

SELECT TOP (20)
    p.PatientId,
    p.MedicalNumber,
    p.FirstName,
    p.LastName,
    p.DepartmentId
FROM medical.Patients AS p
ORDER BY p.PatientId;
GO

PRINT '========================================';
PRINT 'TEST 3: HOSPITAL\doctor.neuro';
PRINT 'Oczekiwany wynik: tylko NEUR / DepartmentId = 3';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\doctor.neuro';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;
GO

PRINT '========================================';
PRINT 'TEST 4: HOSPITAL\nurse.ped';
PRINT 'Oczekiwany wynik: tylko PED / DepartmentId = 5';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\nurse.ped';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    p.DepartmentId,
    d.Code AS DepartmentCode,
    COUNT(*) AS PatientsCount
FROM medical.Patients AS p
INNER JOIN dictionary.Departments AS d
    ON d.DepartmentId = p.DepartmentId
GROUP BY
    p.DepartmentId,
    d.Code
ORDER BY
    p.DepartmentId;
GO

PRINT '========================================';
PRINT 'TEST 5: HOSPITAL\it.admin';
PRINT 'Oczekiwany wynik: brak pacjentów';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = N'HOSPITAL\it.admin';

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    COUNT(*) AS PatientsVisibleForItAdmin
FROM medical.Patients;
GO

PRINT '========================================';
PRINT 'TEST 6: brak SESSION_CONTEXT';
PRINT 'Oczekiwany wynik: brak pacjentów';
PRINT '========================================';

EXEC sys.sp_set_session_context
    @key = N'CurrentUser',
    @value = NULL;

SELECT
    CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser')) AS CurrentUser;

SELECT
    COUNT(*) AS PatientsVisibleWithoutContext
FROM medical.Patients;
GO