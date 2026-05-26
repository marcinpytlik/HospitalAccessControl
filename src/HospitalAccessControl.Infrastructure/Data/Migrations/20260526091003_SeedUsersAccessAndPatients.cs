using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalAccessControl.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAccessAndPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "security",
                table: "ApplicationUsers",
                columns: new[] { "ApplicationUserId", "CreatedAt", "DisplayName", "DomainLogin", "Email", "IsActive", "SamAccountName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jan Kardiolog", "HOSPITAL\\doctor.cardio", "doctor.cardio@hospital.local", true, "doctor.cardio" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marek Ortopeda", "HOSPITAL\\doctor.ortho", "doctor.ortho@hospital.local", true, "doctor.ortho" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Anna Neurolog", "HOSPITAL\\doctor.neuro", "doctor.neuro@hospital.local", true, "doctor.neuro" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ewa Pielęgniarka Kardiologia", "HOSPITAL\\nurse.cardio", "nurse.cardio@hospital.local", true, "nurse.cardio" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Katarzyna Pielęgniarka Ortopedia", "HOSPITAL\\nurse.ortho", "nurse.ortho@hospital.local", true, "nurse.ortho" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Magdalena Pielęgniarka Pediatria", "HOSPITAL\\nurse.ped", "nurse.ped@hospital.local", true, "nurse.ped" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Karolina Rejestracja", "HOSPITAL\\registration.user", "registration.user@hospital.local", true, "registration.user" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tomasz Rejestracja Izba Przyjęć", "HOSPITAL\\registration.emer", "registration.emer@hospital.local", true, "registration.emer" },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Piotr Kierownik Kardiologii", "HOSPITAL\\manager.cardio", "manager.cardio@hospital.local", true, "manager.cardio" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agnieszka Kierownik Ortopedii", "HOSPITAL\\manager.ortho", "manager.ortho@hospital.local", true, "manager.ortho" },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alicja Audytor", "HOSPITAL\\auditor.user", "auditor.user@hospital.local", true, "auditor.user" },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adam Administrator IT", "HOSPITAL\\it.admin", "it.admin@hospital.local", true, "it.admin" }
                });

            migrationBuilder.InsertData(
                schema: "medical",
                table: "Patients",
                columns: new[] { "PatientId", "CreatedAt", "CreatedBy", "DateOfBirth", "DepartmentId", "FirstName", "GenderCode", "IsDeleted", "LastName", "MedicalNumber", "PatientStatusCode", "Pesel", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 2), 1, "Pacjent01", "M", false, "Kardiologiczny01", "CARD-001", "ACTIVE", "90010100001", null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 3), 1, "Pacjent02", "F", false, "Kardiologiczny02", "CARD-002", "ACTIVE", "90010100002", null, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 4), 1, "Pacjent03", "M", false, "Kardiologiczny03", "CARD-003", "ACTIVE", "90010100003", null, null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 5), 1, "Pacjent04", "F", false, "Kardiologiczny04", "CARD-004", "ACTIVE", "90010100004", null, null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 6), 1, "Pacjent05", "M", false, "Kardiologiczny05", "CARD-005", "ACTIVE", "90010100005", null, null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 7), 1, "Pacjent06", "F", false, "Kardiologiczny06", "CARD-006", "ACTIVE", "90010100006", null, null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 8), 1, "Pacjent07", "M", false, "Kardiologiczny07", "CARD-007", "ACTIVE", "90010100007", null, null },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 9), 1, "Pacjent08", "F", false, "Kardiologiczny08", "CARD-008", "ACTIVE", "90010100008", null, null },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 10), 1, "Pacjent09", "M", false, "Kardiologiczny09", "CARD-009", "ACTIVE", "90010100009", null, null },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 11), 1, "Pacjent10", "F", false, "Kardiologiczny10", "CARD-010", "ACTIVE", "90010100010", null, null },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 12), 2, "Pacjent01", "M", false, "Ortopedyczny01", "ORTH-001", "ACTIVE", "90010100011", null, null },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 13), 2, "Pacjent02", "F", false, "Ortopedyczny02", "ORTH-002", "ACTIVE", "90010100012", null, null },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 14), 2, "Pacjent03", "M", false, "Ortopedyczny03", "ORTH-003", "ACTIVE", "90010100013", null, null },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 15), 2, "Pacjent04", "F", false, "Ortopedyczny04", "ORTH-004", "ACTIVE", "90010100014", null, null },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 16), 2, "Pacjent05", "M", false, "Ortopedyczny05", "ORTH-005", "ACTIVE", "90010100015", null, null },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 17), 2, "Pacjent06", "F", false, "Ortopedyczny06", "ORTH-006", "ACTIVE", "90010100016", null, null },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 18), 2, "Pacjent07", "M", false, "Ortopedyczny07", "ORTH-007", "ACTIVE", "90010100017", null, null },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 19), 2, "Pacjent08", "F", false, "Ortopedyczny08", "ORTH-008", "ACTIVE", "90010100018", null, null },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 20), 2, "Pacjent09", "M", false, "Ortopedyczny09", "ORTH-009", "ACTIVE", "90010100019", null, null },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 21), 2, "Pacjent10", "F", false, "Ortopedyczny10", "ORTH-010", "ACTIVE", "90010100020", null, null },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 22), 3, "Pacjent01", "M", false, "Neurologiczny01", "NEUR-001", "ACTIVE", "90010100021", null, null },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 23), 3, "Pacjent02", "F", false, "Neurologiczny02", "NEUR-002", "ACTIVE", "90010100022", null, null },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 24), 3, "Pacjent03", "M", false, "Neurologiczny03", "NEUR-003", "ACTIVE", "90010100023", null, null },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 25), 3, "Pacjent04", "F", false, "Neurologiczny04", "NEUR-004", "ACTIVE", "90010100024", null, null },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 26), 3, "Pacjent05", "M", false, "Neurologiczny05", "NEUR-005", "ACTIVE", "90010100025", null, null },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 27), 3, "Pacjent06", "F", false, "Neurologiczny06", "NEUR-006", "ACTIVE", "90010100026", null, null },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 28), 3, "Pacjent07", "M", false, "Neurologiczny07", "NEUR-007", "ACTIVE", "90010100027", null, null },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 29), 3, "Pacjent08", "F", false, "Neurologiczny08", "NEUR-008", "ACTIVE", "90010100028", null, null },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 30), 3, "Pacjent09", "M", false, "Neurologiczny09", "NEUR-009", "ACTIVE", "90010100029", null, null },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 1, 31), 3, "Pacjent10", "F", false, "Neurologiczny10", "NEUR-010", "ACTIVE", "90010100030", null, null },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 1), 4, "Pacjent01", "M", false, "Nagły01", "EMER-001", "ACTIVE", "90010100031", null, null },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 2), 4, "Pacjent02", "F", false, "Nagły02", "EMER-002", "ACTIVE", "90010100032", null, null },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 3), 4, "Pacjent03", "M", false, "Nagły03", "EMER-003", "ACTIVE", "90010100033", null, null },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 4), 4, "Pacjent04", "F", false, "Nagły04", "EMER-004", "ACTIVE", "90010100034", null, null },
                    { 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 5), 4, "Pacjent05", "M", false, "Nagły05", "EMER-005", "ACTIVE", "90010100035", null, null },
                    { 36, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 6), 5, "Pacjent01", "F", false, "Pediatryczny01", "PED-001", "ACTIVE", "90010100036", null, null },
                    { 37, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 7), 5, "Pacjent02", "M", false, "Pediatryczny02", "PED-002", "ACTIVE", "90010100037", null, null },
                    { 38, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 8), 5, "Pacjent03", "F", false, "Pediatryczny03", "PED-003", "ACTIVE", "90010100038", null, null },
                    { 39, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 9), 5, "Pacjent04", "M", false, "Pediatryczny04", "PED-004", "ACTIVE", "90010100039", null, null },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(1990, 2, 10), 5, "Pacjent05", "F", false, "Pediatryczny05", "PED-005", "ACTIVE", "90010100040", null, null }
                });

            migrationBuilder.InsertData(
                schema: "medical",
                table: "MedicalRecords",
                columns: new[] { "MedicalRecordId", "CreatedAt", "CreatedBy", "DepartmentId", "Description", "Diagnosis", "IsDeleted", "PatientId", "RecordTypeCode", "Title", "Treatment", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-001.", "Kontrola kardiologiczna", false, 1, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-001", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 2, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-002.", "Kontrola kardiologiczna", false, 2, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-002", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 3, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-003.", "Kontrola kardiologiczna", false, 3, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-003", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 4, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-004.", "Kontrola kardiologiczna", false, 4, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-004", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 5, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-005.", "Kontrola kardiologiczna", false, 5, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-005", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 6, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-006.", "Kontrola kardiologiczna", false, 6, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-006", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 7, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-007.", "Kontrola kardiologiczna", false, 7, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-007", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 8, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-008.", "Kontrola kardiologiczna", false, 8, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-008", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 9, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-009.", "Kontrola kardiologiczna", false, 9, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-009", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 10, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 1, "Testowy wpis medyczny dla pacjenta CARD-010.", "Kontrola kardiologiczna", false, 10, "OBSERVATION", "Pierwsza obserwacja pacjenta CARD-010", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 11, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-001.", "Kontrola ortopedyczna", false, 11, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-001", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 12, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-002.", "Kontrola ortopedyczna", false, 12, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-002", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 13, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-003.", "Kontrola ortopedyczna", false, 13, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-003", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 14, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-004.", "Kontrola ortopedyczna", false, 14, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-004", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 15, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-005.", "Kontrola ortopedyczna", false, 15, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-005", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 16, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-006.", "Kontrola ortopedyczna", false, 16, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-006", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 17, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-007.", "Kontrola ortopedyczna", false, 17, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-007", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 18, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-008.", "Kontrola ortopedyczna", false, 18, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-008", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 19, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-009.", "Kontrola ortopedyczna", false, 19, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-009", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 20, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 2, "Testowy wpis medyczny dla pacjenta ORTH-010.", "Kontrola ortopedyczna", false, 20, "OBSERVATION", "Pierwsza obserwacja pacjenta ORTH-010", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 21, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-001.", "Kontrola neurologiczna", false, 21, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-001", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 22, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-002.", "Kontrola neurologiczna", false, 22, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-002", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 23, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-003.", "Kontrola neurologiczna", false, 23, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-003", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 24, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-004.", "Kontrola neurologiczna", false, 24, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-004", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 25, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-005.", "Kontrola neurologiczna", false, 25, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-005", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 26, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-006.", "Kontrola neurologiczna", false, 26, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-006", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 27, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-007.", "Kontrola neurologiczna", false, 27, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-007", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 28, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-008.", "Kontrola neurologiczna", false, 28, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-008", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 29, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-009.", "Kontrola neurologiczna", false, 29, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-009", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 30, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 3, "Testowy wpis medyczny dla pacjenta NEUR-010.", "Kontrola neurologiczna", false, 30, "OBSERVATION", "Pierwsza obserwacja pacjenta NEUR-010", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 31, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 4, "Testowy wpis medyczny dla pacjenta EMER-001.", "Ocena stanu w izbie przyjęć", false, 31, "OBSERVATION", "Pierwsza obserwacja pacjenta EMER-001", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 32, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 4, "Testowy wpis medyczny dla pacjenta EMER-002.", "Ocena stanu w izbie przyjęć", false, 32, "OBSERVATION", "Pierwsza obserwacja pacjenta EMER-002", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 33, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 4, "Testowy wpis medyczny dla pacjenta EMER-003.", "Ocena stanu w izbie przyjęć", false, 33, "OBSERVATION", "Pierwsza obserwacja pacjenta EMER-003", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 34, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 4, "Testowy wpis medyczny dla pacjenta EMER-004.", "Ocena stanu w izbie przyjęć", false, 34, "OBSERVATION", "Pierwsza obserwacja pacjenta EMER-004", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 35, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 4, "Testowy wpis medyczny dla pacjenta EMER-005.", "Ocena stanu w izbie przyjęć", false, 35, "OBSERVATION", "Pierwsza obserwacja pacjenta EMER-005", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 36, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 5, "Testowy wpis medyczny dla pacjenta PED-001.", "Kontrola pediatryczna", false, 36, "OBSERVATION", "Pierwsza obserwacja pacjenta PED-001", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 37, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 5, "Testowy wpis medyczny dla pacjenta PED-002.", "Kontrola pediatryczna", false, 37, "OBSERVATION", "Pierwsza obserwacja pacjenta PED-002", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 38, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 5, "Testowy wpis medyczny dla pacjenta PED-003.", "Kontrola pediatryczna", false, 38, "OBSERVATION", "Pierwsza obserwacja pacjenta PED-003", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 39, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 5, "Testowy wpis medyczny dla pacjenta PED-004.", "Kontrola pediatryczna", false, 39, "OBSERVATION", "Pierwsza obserwacja pacjenta PED-004", "Zalecenia testowe do demonstracji systemu.", null, null },
                    { 40, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "seed", 5, "Testowy wpis medyczny dla pacjenta PED-005.", "Kontrola pediatryczna", false, 40, "OBSERVATION", "Pierwsza obserwacja pacjenta PED-005", "Zalecenia testowe do demonstracji systemu.", null, null }
                });

            migrationBuilder.InsertData(
                schema: "security",
                table: "UserDepartmentAccess",
                columns: new[] { "UserDepartmentAccessId", "ApplicationUserId", "CreatedAt", "CreatedBy", "DepartmentId", "IsActive", "ValidFrom", "ValidTo" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 3, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 5, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 4, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                schema: "security",
                table: "UserRoleAssignments",
                columns: new[] { "UserRoleAssignmentId", "ApplicationRoleId", "ApplicationUserId", "CreatedAt", "CreatedBy", "IsActive", "ValidFrom", "ValidTo" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 1, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 2, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 2, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 2, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 3, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 3, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, 4, 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, 4, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 11, 5, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, 6, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "MedicalRecords",
                keyColumn: "MedicalRecordId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserDepartmentAccess",
                keyColumn: "UserDepartmentAccessId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "UserRoleAssignments",
                keyColumn: "UserRoleAssignmentId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "security",
                table: "ApplicationUsers",
                keyColumn: "ApplicationUserId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "medical",
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 40);
        }
    }
}
