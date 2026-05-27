USE [HospitalAccessControlDb_Dev];
GO

/*
    DAY009
    Row-Level Security predicate function

    Funkcja sprawdza, czy aktualny użytkownik aplikacyjny zapisany
    w SESSION_CONTEXT(N'CurrentUser') ma aktywny dostęp do oddziału.
*/

CREATE OR ALTER FUNCTION security.fn_rls_department_access
(
    @DepartmentId int
)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
(
    SELECT 1 AS fn_access_result
    WHERE EXISTS
    (
        SELECT 1
        FROM security.ApplicationUsers AS au
        INNER JOIN security.UserDepartmentAccess AS uda
            ON uda.ApplicationUserId = au.ApplicationUserId
        WHERE
            au.DomainLogin = CONVERT(nvarchar(256), SESSION_CONTEXT(N'CurrentUser'))
            AND au.IsActive = 1
            AND uda.IsActive = 1
            AND uda.DepartmentId = @DepartmentId
            AND uda.ValidFrom <= SYSUTCDATETIME()
            AND (uda.ValidTo IS NULL OR uda.ValidTo >= SYSUTCDATETIME())
    )
);
GO