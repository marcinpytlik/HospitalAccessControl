SELECT TOP (100)
    event_time,
    action_id,
    succeeded,
    server_principal_name,
    database_name,
    schema_name,
    object_name,
    statement
FROM sys.fn_get_audit_file
(
    'C:\SqlAudit\HospitalAccessControl\*.sqlaudit',
    DEFAULT,
    DEFAULT
)
ORDER BY event_time DESC;
GO
