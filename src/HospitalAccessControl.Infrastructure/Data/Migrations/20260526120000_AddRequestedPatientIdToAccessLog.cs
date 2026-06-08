using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAccessControl.Infrastructure.Data.Migrations;

public partial class AddRequestedPatientIdToAccessLog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "RequestedPatientId",
            schema: "audit",
            table: "AccessLog",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_AccessLog_RequestedPatientId",
            schema: "audit",
            table: "AccessLog",
            column: "RequestedPatientId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AccessLog_RequestedPatientId",
            schema: "audit",
            table: "AccessLog");

        migrationBuilder.DropColumn(
            name: "RequestedPatientId",
            schema: "audit",
            table: "AccessLog");
    }
}
