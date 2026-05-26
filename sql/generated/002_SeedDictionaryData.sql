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

