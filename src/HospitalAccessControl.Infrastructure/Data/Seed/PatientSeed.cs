using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class PatientSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<Patient> Data
    {
        get
        {
            var patients = new List<Patient>();
            var id = 1;

            AddPatients(patients, ref id, departmentId: 1, prefix: "CARD", count: 10, lastNamePrefix: "Kardiologiczny");
            AddPatients(patients, ref id, departmentId: 2, prefix: "ORTH", count: 10, lastNamePrefix: "Ortopedyczny");
            AddPatients(patients, ref id, departmentId: 3, prefix: "NEUR", count: 10, lastNamePrefix: "Neurologiczny");
            AddPatients(patients, ref id, departmentId: 4, prefix: "EMER", count: 5, lastNamePrefix: "Nagły");
            AddPatients(patients, ref id, departmentId: 5, prefix: "PED", count: 5, lastNamePrefix: "Pediatryczny");

            return patients;
        }
    }

    private static void AddPatients(
        List<Patient> patients,
        ref int id,
        int departmentId,
        string prefix,
        int count,
        string lastNamePrefix)
    {
        for (var i = 1; i <= count; i++)
        {
            patients.Add(new Patient
            {
                PatientId = id,
                MedicalNumber = $"{prefix}-{i:000}",
                FirstName = $"Pacjent{i:00}",
                LastName = $"{lastNamePrefix}{i:00}",
                Pesel = $"900101{id:00000}",
                DateOfBirth = new DateOnly(1990, 1, 1).AddDays(id),
                GenderCode = id % 2 == 0 ? "F" : "M",
                DepartmentId = departmentId,
                PatientStatusCode = "ACTIVE",
                CreatedAt = CreatedAt,
                CreatedBy = "seed",
                IsDeleted = false
            });

            id++;
        }
    }
}