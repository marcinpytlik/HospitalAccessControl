using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAccessControl.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.EnsureSchema(
                name: "dictionary");

            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.EnsureSchema(
                name: "medical");

            migrationBuilder.CreateTable(
                name: "ApplicationRoles",
                schema: "dictionary",
                columns: table => new
                {
                    ApplicationRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoles", x => x.ApplicationRoleId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                schema: "security",
                columns: table => new
                {
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainLogin = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SamAccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.ApplicationUserId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "dictionary",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleAssignments",
                schema: "security",
                columns: table => new
                {
                    UserRoleAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationRoleId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAssignments", x => x.UserRoleAssignmentId);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_ApplicationRoles_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalSchema: "dictionary",
                        principalTable: "ApplicationRoles",
                        principalColumn: "ApplicationRoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoleAssignments_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "security",
                        principalTable: "ApplicationUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "medical",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pesel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    GenderCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    PatientStatusCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dictionary",
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDepartmentAccess",
                schema: "security",
                columns: table => new
                {
                    UserDepartmentAccessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartmentAccess", x => x.UserDepartmentAccessId);
                    table.ForeignKey(
                        name: "FK_UserDepartmentAccess_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "security",
                        principalTable: "ApplicationUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDepartmentAccess_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dictionary",
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessLog",
                schema: "audit",
                columns: table => new
                {
                    AccessLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainLogin = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    ActionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AccessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientHost = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLog", x => x.AccessLogId);
                    table.ForeignKey(
                        name: "FK_AccessLog_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "medical",
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                schema: "medical",
                columns: table => new
                {
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    RecordTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Treatment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.MedicalRecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dictionary",
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "medical",
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_AccessDate",
                schema: "audit",
                table: "AccessLog",
                column: "AccessDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_ActionCode",
                schema: "audit",
                table: "AccessLog",
                column: "ActionCode");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_DomainLogin",
                schema: "audit",
                table: "AccessLog",
                column: "DomainLogin");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_PatientId",
                schema: "audit",
                table: "AccessLog",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_WasSuccessful",
                schema: "audit",
                table: "AccessLog",
                column: "WasSuccessful");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationRoles_Code",
                schema: "dictionary",
                table: "ApplicationRoles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUsers_DomainLogin",
                schema: "security",
                table: "ApplicationUsers",
                column: "DomainLogin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationUsers_SamAccountName",
                schema: "security",
                table: "ApplicationUsers",
                column: "SamAccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                schema: "dictionary",
                table: "Departments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "UQ_Departments_Code",
                schema: "dictionary",
                table: "Departments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_CreatedAt",
                schema: "medical",
                table: "MedicalRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DepartmentId",
                schema: "medical",
                table: "MedicalRecords",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                schema: "medical",
                table: "MedicalRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_RecordTypeCode",
                schema: "medical",
                table: "MedicalRecords",
                column: "RecordTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_DepartmentId",
                schema: "medical",
                table: "Patients",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_LastName_FirstName",
                schema: "medical",
                table: "Patients",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Pesel",
                schema: "medical",
                table: "Patients",
                column: "Pesel");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Status",
                schema: "medical",
                table: "Patients",
                column: "PatientStatusCode");

            migrationBuilder.CreateIndex(
                name: "UQ_Patients_MedicalNumber",
                schema: "medical",
                table: "Patients",
                column: "MedicalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentAccess_ApplicationUserId",
                schema: "security",
                table: "UserDepartmentAccess",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentAccess_DepartmentId",
                schema: "security",
                table: "UserDepartmentAccess",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentAccess_User_Department_Active",
                schema: "security",
                table: "UserDepartmentAccess",
                columns: new[] { "ApplicationUserId", "DepartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_ApplicationRoleId",
                schema: "security",
                table: "UserRoleAssignments",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_ApplicationUserId",
                schema: "security",
                table: "UserRoleAssignments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_User_Role_Active",
                schema: "security",
                table: "UserRoleAssignments",
                columns: new[] { "ApplicationUserId", "ApplicationRoleId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLog",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "MedicalRecords",
                schema: "medical");

            migrationBuilder.DropTable(
                name: "UserDepartmentAccess",
                schema: "security");

            migrationBuilder.DropTable(
                name: "UserRoleAssignments",
                schema: "security");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "medical");

            migrationBuilder.DropTable(
                name: "ApplicationRoles",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "ApplicationUsers",
                schema: "security");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "dictionary");
        }
    }
}
