using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalAccessControl.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDictionaryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dictionary",
                table: "ApplicationRoles",
                columns: new[] { "ApplicationRoleId", "Code", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Doctor", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Użytkownik medyczny odpowiedzialny za diagnozę i leczenie pacjentów", true, "Lekarz" },
                    { 2, "Nurse", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Użytkownik medyczny z ograniczonym dostępem do dokumentacji pacjenta", true, "Pielęgniarka" },
                    { 3, "Registration", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Użytkownik odpowiedzialny za obsługę rejestracji pacjentów", true, "Rejestracja" },
                    { 4, "DepartmentManager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Użytkownik zarządzający danym oddziałem", true, "Kierownik oddziału" },
                    { 5, "Auditor", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Użytkownik odpowiedzialny za kontrolę i analizę zdarzeń audytowych", true, "Audytor" },
                    { 6, "ITAdministrator", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator techniczny bez domyślnego dostępu biznesowego do danych medycznych", true, "Administrator IT" }
                });

            migrationBuilder.InsertData(
                schema: "dictionary",
                table: "Departments",
                columns: new[] { "DepartmentId", "Code", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "CARD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oddział kardiologiczny", true, "Kardiologia" },
                    { 2, "ORTH", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oddział ortopedyczny", true, "Ortopedia" },
                    { 3, "NEUR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oddział neurologiczny", true, "Neurologia" },
                    { 4, "EMER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Izba przyjęć i obsługa nagłych przypadków", true, "Izba Przyjęć" },
                    { 5, "PED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oddział pediatryczny", true, "Pediatria" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "ApplicationRoles",
                keyColumn: "ApplicationRoleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dictionary",
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5);
        }
    }
}
