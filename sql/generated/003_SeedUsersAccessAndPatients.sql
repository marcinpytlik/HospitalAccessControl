IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF SCHEMA_ID(N'audit') IS NULL EXEC(N'CREATE SCHEMA [audit];');

IF SCHEMA_ID(N'dictionary') IS NULL EXEC(N'CREATE SCHEMA [dictionary];');

IF SCHEMA_ID(N'security') IS NULL EXEC(N'CREATE SCHEMA [security];');

IF SCHEMA_ID(N'medical') IS NULL EXEC(N'CREATE SCHEMA [medical];');

CREATE TABLE [dictionary].[ApplicationRoles] (
    [ApplicationRoleId] int NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ApplicationRoles] PRIMARY KEY ([ApplicationRoleId])
);

CREATE TABLE [security].[ApplicationUsers] (
    [ApplicationUserId] int NOT NULL IDENTITY,
    [DomainLogin] nvarchar(256) NOT NULL,
    [SamAccountName] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(200) NOT NULL,
    [Email] nvarchar(256) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ApplicationUsers] PRIMARY KEY ([ApplicationUserId])
);

CREATE TABLE [dictionary].[Departments] (
    [DepartmentId] int NOT NULL IDENTITY,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([DepartmentId])
);

CREATE TABLE [security].[UserRoleAssignments] (
    [UserRoleAssignmentId] int NOT NULL IDENTITY,
    [ApplicationUserId] int NOT NULL,
    [ApplicationRoleId] int NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_UserRoleAssignments] PRIMARY KEY ([UserRoleAssignmentId]),
    CONSTRAINT [FK_UserRoleAssignments_ApplicationRoles_ApplicationRoleId] FOREIGN KEY ([ApplicationRoleId]) REFERENCES [dictionary].[ApplicationRoles] ([ApplicationRoleId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoleAssignments_ApplicationUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [security].[ApplicationUsers] ([ApplicationUserId]) ON DELETE NO ACTION
);

CREATE TABLE [medical].[Patients] (
    [PatientId] int NOT NULL IDENTITY,
    [MedicalNumber] nvarchar(50) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Pesel] nvarchar(11) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [GenderCode] nvarchar(10) NOT NULL,
    [DepartmentId] int NOT NULL,
    [PatientStatusCode] nvarchar(30) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([PatientId]),
    CONSTRAINT [FK_Patients_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dictionary].[Departments] ([DepartmentId]) ON DELETE NO ACTION
);

CREATE TABLE [security].[UserDepartmentAccess] (
    [UserDepartmentAccessId] int NOT NULL IDENTITY,
    [ApplicationUserId] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [ValidFrom] datetime2 NOT NULL,
    [ValidTo] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_UserDepartmentAccess] PRIMARY KEY ([UserDepartmentAccessId]),
    CONSTRAINT [FK_UserDepartmentAccess_ApplicationUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [security].[ApplicationUsers] ([ApplicationUserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserDepartmentAccess_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dictionary].[Departments] ([DepartmentId]) ON DELETE NO ACTION
);

CREATE TABLE [audit].[AccessLog] (
    [AccessLogId] bigint NOT NULL IDENTITY,
    [DomainLogin] nvarchar(256) NOT NULL,
    [PatientId] int NULL,
    [ActionCode] nvarchar(100) NOT NULL,
    [ObjectName] nvarchar(256) NOT NULL,
    [AccessDate] datetime2 NOT NULL,
    [ClientHost] nvarchar(256) NULL,
    [ApplicationName] nvarchar(256) NULL,
    [WasSuccessful] bit NOT NULL,
    [AdditionalInfo] nvarchar(2000) NULL,
    CONSTRAINT [PK_AccessLog] PRIMARY KEY ([AccessLogId]),
    CONSTRAINT [FK_AccessLog_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [medical].[Patients] ([PatientId]) ON DELETE SET NULL
);

CREATE TABLE [medical].[MedicalRecords] (
    [MedicalRecordId] int NOT NULL IDENTITY,
    [PatientId] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [RecordTypeCode] nvarchar(50) NOT NULL,
    [Title] nvarchar(300) NOT NULL,
    [Description] nvarchar(4000) NULL,
    [Diagnosis] nvarchar(4000) NULL,
    [Treatment] nvarchar(4000) NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(256) NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([MedicalRecordId]),
    CONSTRAINT [FK_MedicalRecords_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dictionary].[Departments] ([DepartmentId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [medical].[Patients] ([PatientId]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AccessLog_AccessDate] ON [audit].[AccessLog] ([AccessDate]);

CREATE INDEX [IX_AccessLog_ActionCode] ON [audit].[AccessLog] ([ActionCode]);

CREATE INDEX [IX_AccessLog_DomainLogin] ON [audit].[AccessLog] ([DomainLogin]);

CREATE INDEX [IX_AccessLog_PatientId] ON [audit].[AccessLog] ([PatientId]);

CREATE INDEX [IX_AccessLog_WasSuccessful] ON [audit].[AccessLog] ([WasSuccessful]);

CREATE UNIQUE INDEX [UQ_ApplicationRoles_Code] ON [dictionary].[ApplicationRoles] ([Code]);

CREATE UNIQUE INDEX [UQ_ApplicationUsers_DomainLogin] ON [security].[ApplicationUsers] ([DomainLogin]);

CREATE UNIQUE INDEX [UQ_ApplicationUsers_SamAccountName] ON [security].[ApplicationUsers] ([SamAccountName]);

CREATE INDEX [IX_Departments_Name] ON [dictionary].[Departments] ([Name]);

CREATE UNIQUE INDEX [UQ_Departments_Code] ON [dictionary].[Departments] ([Code]);

CREATE INDEX [IX_MedicalRecords_CreatedAt] ON [medical].[MedicalRecords] ([CreatedAt]);

CREATE INDEX [IX_MedicalRecords_DepartmentId] ON [medical].[MedicalRecords] ([DepartmentId]);

CREATE INDEX [IX_MedicalRecords_PatientId] ON [medical].[MedicalRecords] ([PatientId]);

CREATE INDEX [IX_MedicalRecords_RecordTypeCode] ON [medical].[MedicalRecords] ([RecordTypeCode]);

CREATE INDEX [IX_Patients_DepartmentId] ON [medical].[Patients] ([DepartmentId]);

CREATE INDEX [IX_Patients_LastName_FirstName] ON [medical].[Patients] ([LastName], [FirstName]);

CREATE INDEX [IX_Patients_Pesel] ON [medical].[Patients] ([Pesel]);

CREATE INDEX [IX_Patients_Status] ON [medical].[Patients] ([PatientStatusCode]);

CREATE UNIQUE INDEX [UQ_Patients_MedicalNumber] ON [medical].[Patients] ([MedicalNumber]);

CREATE INDEX [IX_UserDepartmentAccess_ApplicationUserId] ON [security].[UserDepartmentAccess] ([ApplicationUserId]);

CREATE INDEX [IX_UserDepartmentAccess_DepartmentId] ON [security].[UserDepartmentAccess] ([DepartmentId]);

CREATE INDEX [IX_UserDepartmentAccess_User_Department_Active] ON [security].[UserDepartmentAccess] ([ApplicationUserId], [DepartmentId], [IsActive]);

CREATE INDEX [IX_UserRoleAssignments_ApplicationRoleId] ON [security].[UserRoleAssignments] ([ApplicationRoleId]);

CREATE INDEX [IX_UserRoleAssignments_ApplicationUserId] ON [security].[UserRoleAssignments] ([ApplicationUserId]);

CREATE INDEX [IX_UserRoleAssignments_User_Role_Active] ON [security].[UserRoleAssignments] ([ApplicationUserId], [ApplicationRoleId], [IsActive]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526072248_InitialCreate', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ApplicationRoleId', N'Code', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[dictionary].[ApplicationRoles]'))
    SET IDENTITY_INSERT [dictionary].[ApplicationRoles] ON;
INSERT INTO [dictionary].[ApplicationRoles] ([ApplicationRoleId], [Code], [CreatedAt], [Description], [IsActive], [Name])
VALUES (1, N'Doctor', '2026-01-01T00:00:00.0000000Z', N'Użytkownik medyczny odpowiedzialny za diagnozę i leczenie pacjentów', CAST(1 AS bit), N'Lekarz'),
(2, N'Nurse', '2026-01-01T00:00:00.0000000Z', N'Użytkownik medyczny z ograniczonym dostępem do dokumentacji pacjenta', CAST(1 AS bit), N'Pielęgniarka'),
(3, N'Registration', '2026-01-01T00:00:00.0000000Z', N'Użytkownik odpowiedzialny za obsługę rejestracji pacjentów', CAST(1 AS bit), N'Rejestracja'),
(4, N'DepartmentManager', '2026-01-01T00:00:00.0000000Z', N'Użytkownik zarządzający danym oddziałem', CAST(1 AS bit), N'Kierownik oddziału'),
(5, N'Auditor', '2026-01-01T00:00:00.0000000Z', N'Użytkownik odpowiedzialny za kontrolę i analizę zdarzeń audytowych', CAST(1 AS bit), N'Audytor'),
(6, N'ITAdministrator', '2026-01-01T00:00:00.0000000Z', N'Administrator techniczny bez domyślnego dostępu biznesowego do danych medycznych', CAST(1 AS bit), N'Administrator IT');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ApplicationRoleId', N'Code', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[dictionary].[ApplicationRoles]'))
    SET IDENTITY_INSERT [dictionary].[ApplicationRoles] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'DepartmentId', N'Code', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[dictionary].[Departments]'))
    SET IDENTITY_INSERT [dictionary].[Departments] ON;
INSERT INTO [dictionary].[Departments] ([DepartmentId], [Code], [CreatedAt], [Description], [IsActive], [Name])
VALUES (1, N'CARD', '2026-01-01T00:00:00.0000000Z', N'Oddział kardiologiczny', CAST(1 AS bit), N'Kardiologia'),
(2, N'ORTH', '2026-01-01T00:00:00.0000000Z', N'Oddział ortopedyczny', CAST(1 AS bit), N'Ortopedia'),
(3, N'NEUR', '2026-01-01T00:00:00.0000000Z', N'Oddział neurologiczny', CAST(1 AS bit), N'Neurologia'),
(4, N'EMER', '2026-01-01T00:00:00.0000000Z', N'Izba przyjęć i obsługa nagłych przypadków', CAST(1 AS bit), N'Izba Przyjęć'),
(5, N'PED', '2026-01-01T00:00:00.0000000Z', N'Oddział pediatryczny', CAST(1 AS bit), N'Pediatria');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'DepartmentId', N'Code', N'CreatedAt', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[dictionary].[Departments]'))
    SET IDENTITY_INSERT [dictionary].[Departments] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526084111_SeedDictionaryData', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ApplicationUserId', N'CreatedAt', N'DisplayName', N'DomainLogin', N'Email', N'IsActive', N'SamAccountName') AND [object_id] = OBJECT_ID(N'[security].[ApplicationUsers]'))
    SET IDENTITY_INSERT [security].[ApplicationUsers] ON;
INSERT INTO [security].[ApplicationUsers] ([ApplicationUserId], [CreatedAt], [DisplayName], [DomainLogin], [Email], [IsActive], [SamAccountName])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'Jan Kardiolog', N'HOSPITAL\doctor.cardio', N'doctor.cardio@hospital.local', CAST(1 AS bit), N'doctor.cardio'),
(2, '2026-01-01T00:00:00.0000000Z', N'Marek Ortopeda', N'HOSPITAL\doctor.ortho', N'doctor.ortho@hospital.local', CAST(1 AS bit), N'doctor.ortho'),
(3, '2026-01-01T00:00:00.0000000Z', N'Anna Neurolog', N'HOSPITAL\doctor.neuro', N'doctor.neuro@hospital.local', CAST(1 AS bit), N'doctor.neuro'),
(4, '2026-01-01T00:00:00.0000000Z', N'Ewa Pielęgniarka Kardiologia', N'HOSPITAL\nurse.cardio', N'nurse.cardio@hospital.local', CAST(1 AS bit), N'nurse.cardio'),
(5, '2026-01-01T00:00:00.0000000Z', N'Katarzyna Pielęgniarka Ortopedia', N'HOSPITAL\nurse.ortho', N'nurse.ortho@hospital.local', CAST(1 AS bit), N'nurse.ortho'),
(6, '2026-01-01T00:00:00.0000000Z', N'Magdalena Pielęgniarka Pediatria', N'HOSPITAL\nurse.ped', N'nurse.ped@hospital.local', CAST(1 AS bit), N'nurse.ped'),
(7, '2026-01-01T00:00:00.0000000Z', N'Karolina Rejestracja', N'HOSPITAL\registration.user', N'registration.user@hospital.local', CAST(1 AS bit), N'registration.user'),
(8, '2026-01-01T00:00:00.0000000Z', N'Tomasz Rejestracja Izba Przyjęć', N'HOSPITAL\registration.emer', N'registration.emer@hospital.local', CAST(1 AS bit), N'registration.emer'),
(9, '2026-01-01T00:00:00.0000000Z', N'Piotr Kierownik Kardiologii', N'HOSPITAL\manager.cardio', N'manager.cardio@hospital.local', CAST(1 AS bit), N'manager.cardio'),
(10, '2026-01-01T00:00:00.0000000Z', N'Agnieszka Kierownik Ortopedii', N'HOSPITAL\manager.ortho', N'manager.ortho@hospital.local', CAST(1 AS bit), N'manager.ortho'),
(11, '2026-01-01T00:00:00.0000000Z', N'Alicja Audytor', N'HOSPITAL\auditor.user', N'auditor.user@hospital.local', CAST(1 AS bit), N'auditor.user'),
(12, '2026-01-01T00:00:00.0000000Z', N'Adam Administrator IT', N'HOSPITAL\it.admin', N'it.admin@hospital.local', CAST(1 AS bit), N'it.admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ApplicationUserId', N'CreatedAt', N'DisplayName', N'DomainLogin', N'Email', N'IsActive', N'SamAccountName') AND [object_id] = OBJECT_ID(N'[security].[ApplicationUsers]'))
    SET IDENTITY_INSERT [security].[ApplicationUsers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PatientId', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'DepartmentId', N'FirstName', N'GenderCode', N'IsDeleted', N'LastName', N'MedicalNumber', N'PatientStatusCode', N'Pesel', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[medical].[Patients]'))
    SET IDENTITY_INSERT [medical].[Patients] ON;
INSERT INTO [medical].[Patients] ([PatientId], [CreatedAt], [CreatedBy], [DateOfBirth], [DepartmentId], [FirstName], [GenderCode], [IsDeleted], [LastName], [MedicalNumber], [PatientStatusCode], [Pesel], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-02', 1, N'Pacjent01', N'M', CAST(0 AS bit), N'Kardiologiczny01', N'CARD-001', N'ACTIVE', N'90010100001', NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-03', 1, N'Pacjent02', N'F', CAST(0 AS bit), N'Kardiologiczny02', N'CARD-002', N'ACTIVE', N'90010100002', NULL, NULL),
(3, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-04', 1, N'Pacjent03', N'M', CAST(0 AS bit), N'Kardiologiczny03', N'CARD-003', N'ACTIVE', N'90010100003', NULL, NULL),
(4, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-05', 1, N'Pacjent04', N'F', CAST(0 AS bit), N'Kardiologiczny04', N'CARD-004', N'ACTIVE', N'90010100004', NULL, NULL),
(5, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-06', 1, N'Pacjent05', N'M', CAST(0 AS bit), N'Kardiologiczny05', N'CARD-005', N'ACTIVE', N'90010100005', NULL, NULL),
(6, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-07', 1, N'Pacjent06', N'F', CAST(0 AS bit), N'Kardiologiczny06', N'CARD-006', N'ACTIVE', N'90010100006', NULL, NULL),
(7, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-08', 1, N'Pacjent07', N'M', CAST(0 AS bit), N'Kardiologiczny07', N'CARD-007', N'ACTIVE', N'90010100007', NULL, NULL),
(8, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-09', 1, N'Pacjent08', N'F', CAST(0 AS bit), N'Kardiologiczny08', N'CARD-008', N'ACTIVE', N'90010100008', NULL, NULL),
(9, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-10', 1, N'Pacjent09', N'M', CAST(0 AS bit), N'Kardiologiczny09', N'CARD-009', N'ACTIVE', N'90010100009', NULL, NULL),
(10, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-11', 1, N'Pacjent10', N'F', CAST(0 AS bit), N'Kardiologiczny10', N'CARD-010', N'ACTIVE', N'90010100010', NULL, NULL),
(11, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-12', 2, N'Pacjent01', N'M', CAST(0 AS bit), N'Ortopedyczny01', N'ORTH-001', N'ACTIVE', N'90010100011', NULL, NULL),
(12, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-13', 2, N'Pacjent02', N'F', CAST(0 AS bit), N'Ortopedyczny02', N'ORTH-002', N'ACTIVE', N'90010100012', NULL, NULL),
(13, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-14', 2, N'Pacjent03', N'M', CAST(0 AS bit), N'Ortopedyczny03', N'ORTH-003', N'ACTIVE', N'90010100013', NULL, NULL),
(14, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-15', 2, N'Pacjent04', N'F', CAST(0 AS bit), N'Ortopedyczny04', N'ORTH-004', N'ACTIVE', N'90010100014', NULL, NULL),
(15, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-16', 2, N'Pacjent05', N'M', CAST(0 AS bit), N'Ortopedyczny05', N'ORTH-005', N'ACTIVE', N'90010100015', NULL, NULL),
(16, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-17', 2, N'Pacjent06', N'F', CAST(0 AS bit), N'Ortopedyczny06', N'ORTH-006', N'ACTIVE', N'90010100016', NULL, NULL),
(17, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-18', 2, N'Pacjent07', N'M', CAST(0 AS bit), N'Ortopedyczny07', N'ORTH-007', N'ACTIVE', N'90010100017', NULL, NULL),
(18, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-19', 2, N'Pacjent08', N'F', CAST(0 AS bit), N'Ortopedyczny08', N'ORTH-008', N'ACTIVE', N'90010100018', NULL, NULL),
(19, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-20', 2, N'Pacjent09', N'M', CAST(0 AS bit), N'Ortopedyczny09', N'ORTH-009', N'ACTIVE', N'90010100019', NULL, NULL),
(20, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-21', 2, N'Pacjent10', N'F', CAST(0 AS bit), N'Ortopedyczny10', N'ORTH-010', N'ACTIVE', N'90010100020', NULL, NULL),
(21, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-22', 3, N'Pacjent01', N'M', CAST(0 AS bit), N'Neurologiczny01', N'NEUR-001', N'ACTIVE', N'90010100021', NULL, NULL),
(22, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-23', 3, N'Pacjent02', N'F', CAST(0 AS bit), N'Neurologiczny02', N'NEUR-002', N'ACTIVE', N'90010100022', NULL, NULL),
(23, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-24', 3, N'Pacjent03', N'M', CAST(0 AS bit), N'Neurologiczny03', N'NEUR-003', N'ACTIVE', N'90010100023', NULL, NULL),
(24, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-25', 3, N'Pacjent04', N'F', CAST(0 AS bit), N'Neurologiczny04', N'NEUR-004', N'ACTIVE', N'90010100024', NULL, NULL),
(25, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-26', 3, N'Pacjent05', N'M', CAST(0 AS bit), N'Neurologiczny05', N'NEUR-005', N'ACTIVE', N'90010100025', NULL, NULL),
(26, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-27', 3, N'Pacjent06', N'F', CAST(0 AS bit), N'Neurologiczny06', N'NEUR-006', N'ACTIVE', N'90010100026', NULL, NULL),
(27, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-28', 3, N'Pacjent07', N'M', CAST(0 AS bit), N'Neurologiczny07', N'NEUR-007', N'ACTIVE', N'90010100027', NULL, NULL),
(28, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-29', 3, N'Pacjent08', N'F', CAST(0 AS bit), N'Neurologiczny08', N'NEUR-008', N'ACTIVE', N'90010100028', NULL, NULL),
(29, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-30', 3, N'Pacjent09', N'M', CAST(0 AS bit), N'Neurologiczny09', N'NEUR-009', N'ACTIVE', N'90010100029', NULL, NULL),
(30, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-01-31', 3, N'Pacjent10', N'F', CAST(0 AS bit), N'Neurologiczny10', N'NEUR-010', N'ACTIVE', N'90010100030', NULL, NULL),
(31, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-01', 4, N'Pacjent01', N'M', CAST(0 AS bit), N'Nagły01', N'EMER-001', N'ACTIVE', N'90010100031', NULL, NULL),
(32, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-02', 4, N'Pacjent02', N'F', CAST(0 AS bit), N'Nagły02', N'EMER-002', N'ACTIVE', N'90010100032', NULL, NULL),
(33, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-03', 4, N'Pacjent03', N'M', CAST(0 AS bit), N'Nagły03', N'EMER-003', N'ACTIVE', N'90010100033', NULL, NULL),
(34, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-04', 4, N'Pacjent04', N'F', CAST(0 AS bit), N'Nagły04', N'EMER-004', N'ACTIVE', N'90010100034', NULL, NULL),
(35, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-05', 4, N'Pacjent05', N'M', CAST(0 AS bit), N'Nagły05', N'EMER-005', N'ACTIVE', N'90010100035', NULL, NULL),
(36, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-06', 5, N'Pacjent01', N'F', CAST(0 AS bit), N'Pediatryczny01', N'PED-001', N'ACTIVE', N'90010100036', NULL, NULL),
(37, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-07', 5, N'Pacjent02', N'M', CAST(0 AS bit), N'Pediatryczny02', N'PED-002', N'ACTIVE', N'90010100037', NULL, NULL),
(38, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-08', 5, N'Pacjent03', N'F', CAST(0 AS bit), N'Pediatryczny03', N'PED-003', N'ACTIVE', N'90010100038', NULL, NULL),
(39, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-09', 5, N'Pacjent04', N'M', CAST(0 AS bit), N'Pediatryczny04', N'PED-004', N'ACTIVE', N'90010100039', NULL, NULL),
(40, '2026-01-01T00:00:00.0000000Z', N'seed', '1990-02-10', 5, N'Pacjent05', N'F', CAST(0 AS bit), N'Pediatryczny05', N'PED-005', N'ACTIVE', N'90010100040', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PatientId', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'DepartmentId', N'FirstName', N'GenderCode', N'IsDeleted', N'LastName', N'MedicalNumber', N'PatientStatusCode', N'Pesel', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[medical].[Patients]'))
    SET IDENTITY_INSERT [medical].[Patients] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MedicalRecordId', N'CreatedAt', N'CreatedBy', N'DepartmentId', N'Description', N'Diagnosis', N'IsDeleted', N'PatientId', N'RecordTypeCode', N'Title', N'Treatment', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[medical].[MedicalRecords]'))
    SET IDENTITY_INSERT [medical].[MedicalRecords] ON;
INSERT INTO [medical].[MedicalRecords] ([MedicalRecordId], [CreatedAt], [CreatedBy], [DepartmentId], [Description], [Diagnosis], [IsDeleted], [PatientId], [RecordTypeCode], [Title], [Treatment], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-001.', N'Kontrola kardiologiczna', CAST(0 AS bit), 1, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-001', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(2, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-002.', N'Kontrola kardiologiczna', CAST(0 AS bit), 2, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-002', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(3, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-003.', N'Kontrola kardiologiczna', CAST(0 AS bit), 3, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-003', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(4, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-004.', N'Kontrola kardiologiczna', CAST(0 AS bit), 4, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-004', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(5, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-005.', N'Kontrola kardiologiczna', CAST(0 AS bit), 5, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-005', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(6, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-006.', N'Kontrola kardiologiczna', CAST(0 AS bit), 6, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-006', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(7, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-007.', N'Kontrola kardiologiczna', CAST(0 AS bit), 7, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-007', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(8, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-008.', N'Kontrola kardiologiczna', CAST(0 AS bit), 8, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-008', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(9, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-009.', N'Kontrola kardiologiczna', CAST(0 AS bit), 9, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-009', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(10, '2026-01-02T08:00:00.0000000Z', N'seed', 1, N'Testowy wpis medyczny dla pacjenta CARD-010.', N'Kontrola kardiologiczna', CAST(0 AS bit), 10, N'OBSERVATION', N'Pierwsza obserwacja pacjenta CARD-010', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(11, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-001.', N'Kontrola ortopedyczna', CAST(0 AS bit), 11, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-001', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(12, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-002.', N'Kontrola ortopedyczna', CAST(0 AS bit), 12, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-002', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(13, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-003.', N'Kontrola ortopedyczna', CAST(0 AS bit), 13, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-003', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(14, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-004.', N'Kontrola ortopedyczna', CAST(0 AS bit), 14, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-004', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(15, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-005.', N'Kontrola ortopedyczna', CAST(0 AS bit), 15, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-005', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(16, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-006.', N'Kontrola ortopedyczna', CAST(0 AS bit), 16, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-006', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(17, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-007.', N'Kontrola ortopedyczna', CAST(0 AS bit), 17, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-007', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(18, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-008.', N'Kontrola ortopedyczna', CAST(0 AS bit), 18, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-008', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(19, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-009.', N'Kontrola ortopedyczna', CAST(0 AS bit), 19, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-009', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(20, '2026-01-02T08:00:00.0000000Z', N'seed', 2, N'Testowy wpis medyczny dla pacjenta ORTH-010.', N'Kontrola ortopedyczna', CAST(0 AS bit), 20, N'OBSERVATION', N'Pierwsza obserwacja pacjenta ORTH-010', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(21, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-001.', N'Kontrola neurologiczna', CAST(0 AS bit), 21, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-001', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(22, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-002.', N'Kontrola neurologiczna', CAST(0 AS bit), 22, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-002', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(23, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-003.', N'Kontrola neurologiczna', CAST(0 AS bit), 23, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-003', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(24, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-004.', N'Kontrola neurologiczna', CAST(0 AS bit), 24, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-004', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(25, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-005.', N'Kontrola neurologiczna', CAST(0 AS bit), 25, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-005', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(26, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-006.', N'Kontrola neurologiczna', CAST(0 AS bit), 26, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-006', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(27, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-007.', N'Kontrola neurologiczna', CAST(0 AS bit), 27, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-007', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(28, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-008.', N'Kontrola neurologiczna', CAST(0 AS bit), 28, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-008', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(29, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-009.', N'Kontrola neurologiczna', CAST(0 AS bit), 29, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-009', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(30, '2026-01-02T08:00:00.0000000Z', N'seed', 3, N'Testowy wpis medyczny dla pacjenta NEUR-010.', N'Kontrola neurologiczna', CAST(0 AS bit), 30, N'OBSERVATION', N'Pierwsza obserwacja pacjenta NEUR-010', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(31, '2026-01-02T08:00:00.0000000Z', N'seed', 4, N'Testowy wpis medyczny dla pacjenta EMER-001.', N'Ocena stanu w izbie przyjęć', CAST(0 AS bit), 31, N'OBSERVATION', N'Pierwsza obserwacja pacjenta EMER-001', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(32, '2026-01-02T08:00:00.0000000Z', N'seed', 4, N'Testowy wpis medyczny dla pacjenta EMER-002.', N'Ocena stanu w izbie przyjęć', CAST(0 AS bit), 32, N'OBSERVATION', N'Pierwsza obserwacja pacjenta EMER-002', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(33, '2026-01-02T08:00:00.0000000Z', N'seed', 4, N'Testowy wpis medyczny dla pacjenta EMER-003.', N'Ocena stanu w izbie przyjęć', CAST(0 AS bit), 33, N'OBSERVATION', N'Pierwsza obserwacja pacjenta EMER-003', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(34, '2026-01-02T08:00:00.0000000Z', N'seed', 4, N'Testowy wpis medyczny dla pacjenta EMER-004.', N'Ocena stanu w izbie przyjęć', CAST(0 AS bit), 34, N'OBSERVATION', N'Pierwsza obserwacja pacjenta EMER-004', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(35, '2026-01-02T08:00:00.0000000Z', N'seed', 4, N'Testowy wpis medyczny dla pacjenta EMER-005.', N'Ocena stanu w izbie przyjęć', CAST(0 AS bit), 35, N'OBSERVATION', N'Pierwsza obserwacja pacjenta EMER-005', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(36, '2026-01-02T08:00:00.0000000Z', N'seed', 5, N'Testowy wpis medyczny dla pacjenta PED-001.', N'Kontrola pediatryczna', CAST(0 AS bit), 36, N'OBSERVATION', N'Pierwsza obserwacja pacjenta PED-001', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(37, '2026-01-02T08:00:00.0000000Z', N'seed', 5, N'Testowy wpis medyczny dla pacjenta PED-002.', N'Kontrola pediatryczna', CAST(0 AS bit), 37, N'OBSERVATION', N'Pierwsza obserwacja pacjenta PED-002', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(38, '2026-01-02T08:00:00.0000000Z', N'seed', 5, N'Testowy wpis medyczny dla pacjenta PED-003.', N'Kontrola pediatryczna', CAST(0 AS bit), 38, N'OBSERVATION', N'Pierwsza obserwacja pacjenta PED-003', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(39, '2026-01-02T08:00:00.0000000Z', N'seed', 5, N'Testowy wpis medyczny dla pacjenta PED-004.', N'Kontrola pediatryczna', CAST(0 AS bit), 39, N'OBSERVATION', N'Pierwsza obserwacja pacjenta PED-004', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL),
(40, '2026-01-02T08:00:00.0000000Z', N'seed', 5, N'Testowy wpis medyczny dla pacjenta PED-005.', N'Kontrola pediatryczna', CAST(0 AS bit), 40, N'OBSERVATION', N'Pierwsza obserwacja pacjenta PED-005', N'Zalecenia testowe do demonstracji systemu.', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MedicalRecordId', N'CreatedAt', N'CreatedBy', N'DepartmentId', N'Description', N'Diagnosis', N'IsDeleted', N'PatientId', N'RecordTypeCode', N'Title', N'Treatment', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[medical].[MedicalRecords]'))
    SET IDENTITY_INSERT [medical].[MedicalRecords] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserDepartmentAccessId', N'ApplicationUserId', N'CreatedAt', N'CreatedBy', N'DepartmentId', N'IsActive', N'ValidFrom', N'ValidTo') AND [object_id] = OBJECT_ID(N'[security].[UserDepartmentAccess]'))
    SET IDENTITY_INSERT [security].[UserDepartmentAccess] ON;
INSERT INTO [security].[UserDepartmentAccess] ([UserDepartmentAccessId], [ApplicationUserId], [CreatedAt], [CreatedBy], [DepartmentId], [IsActive], [ValidFrom], [ValidTo])
VALUES (1, 1, '2026-01-01T00:00:00.0000000Z', N'seed', 1, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(2, 2, '2026-01-01T00:00:00.0000000Z', N'seed', 2, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(3, 3, '2026-01-01T00:00:00.0000000Z', N'seed', 3, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(4, 4, '2026-01-01T00:00:00.0000000Z', N'seed', 1, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(5, 5, '2026-01-01T00:00:00.0000000Z', N'seed', 2, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(6, 6, '2026-01-01T00:00:00.0000000Z', N'seed', 5, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(7, 8, '2026-01-01T00:00:00.0000000Z', N'seed', 4, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(8, 9, '2026-01-01T00:00:00.0000000Z', N'seed', 1, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(9, 10, '2026-01-01T00:00:00.0000000Z', N'seed', 2, CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserDepartmentAccessId', N'ApplicationUserId', N'CreatedAt', N'CreatedBy', N'DepartmentId', N'IsActive', N'ValidFrom', N'ValidTo') AND [object_id] = OBJECT_ID(N'[security].[UserDepartmentAccess]'))
    SET IDENTITY_INSERT [security].[UserDepartmentAccess] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserRoleAssignmentId', N'ApplicationRoleId', N'ApplicationUserId', N'CreatedAt', N'CreatedBy', N'IsActive', N'ValidFrom', N'ValidTo') AND [object_id] = OBJECT_ID(N'[security].[UserRoleAssignments]'))
    SET IDENTITY_INSERT [security].[UserRoleAssignments] ON;
INSERT INTO [security].[UserRoleAssignments] ([UserRoleAssignmentId], [ApplicationRoleId], [ApplicationUserId], [CreatedAt], [CreatedBy], [IsActive], [ValidFrom], [ValidTo])
VALUES (1, 1, 1, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(2, 1, 2, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(3, 1, 3, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(4, 2, 4, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(5, 2, 5, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(6, 2, 6, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(7, 3, 7, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(8, 3, 8, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(9, 4, 9, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(10, 4, 10, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(11, 5, 11, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL),
(12, 6, 12, '2026-01-01T00:00:00.0000000Z', N'seed', CAST(1 AS bit), '2026-01-01T00:00:00.0000000Z', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserRoleAssignmentId', N'ApplicationRoleId', N'ApplicationUserId', N'CreatedAt', N'CreatedBy', N'IsActive', N'ValidFrom', N'ValidTo') AND [object_id] = OBJECT_ID(N'[security].[UserRoleAssignments]'))
    SET IDENTITY_INSERT [security].[UserRoleAssignments] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526091003_SeedUsersAccessAndPatients', N'10.0.8');

COMMIT;
GO

