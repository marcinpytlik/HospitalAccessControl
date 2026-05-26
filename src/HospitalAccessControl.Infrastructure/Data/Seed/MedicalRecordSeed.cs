using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class MedicalRecordSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 2, 8, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<MedicalRecord> Data
    {
        get
        {
            var records = new List<MedicalRecord>();
            var id = 1;

            foreach (var patient in PatientSeed.Data)
            {
                records.Add(new MedicalRecord
                {
                    MedicalRecordId = id,
                    PatientId = patient.PatientId,
                    DepartmentId = patient.DepartmentId,
                    RecordTypeCode = "OBSERVATION",
                    Title = $"Pierwsza obserwacja pacjenta {patient.MedicalNumber}",
                    Description = $"Testowy wpis medyczny dla pacjenta {patient.MedicalNumber}.",
                    Diagnosis = patient.DepartmentId switch
                    {
                        1 => "Kontrola kardiologiczna",
                        2 => "Kontrola ortopedyczna",
                        3 => "Kontrola neurologiczna",
                        4 => "Ocena stanu w izbie przyjęć",
                        5 => "Kontrola pediatryczna",
                        _ => "Obserwacja ogólna"
                    },
                    Treatment = "Zalecenia testowe do demonstracji systemu.",
                    CreatedAt = CreatedAt,
                    CreatedBy = "seed",
                    IsDeleted = false
                });

                id++;
            }

            return records;
        }
    }
}