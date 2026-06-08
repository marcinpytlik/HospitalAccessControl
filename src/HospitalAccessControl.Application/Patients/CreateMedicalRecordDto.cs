namespace HospitalAccessControl.Application.Patients;

public sealed class CreateMedicalRecordDto
{
    public int PatientId { get; init; }

    public string RecordTypeCode { get; init; } = "OBSERVATION";

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Diagnosis { get; init; }

    public string? Treatment { get; init; }

    public string CreatedBy { get; init; } = string.Empty;
}
