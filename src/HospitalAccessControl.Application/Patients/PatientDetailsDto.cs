namespace HospitalAccessControl.Application.Patients;

public sealed class PatientDetailsDto
{
    public int PatientId { get; init; }

    public string MedicalNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Pesel { get; init; } = string.Empty;

    public DateOnly DateOfBirth { get; init; }

    public string GenderCode { get; init; } = string.Empty;

    public string PatientStatusCode { get; init; } = string.Empty;

    public int DepartmentId { get; init; }

    public string DepartmentCode { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public IReadOnlyList<PatientMedicalRecordDto> MedicalRecords { get; init; }
        = Array.Empty<PatientMedicalRecordDto>();
}

public sealed class PatientMedicalRecordDto
{
    public int MedicalRecordId { get; init; }

    public string RecordTypeCode { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Diagnosis { get; init; }

    public string? Treatment { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}