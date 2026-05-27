namespace HospitalAccessControl.Application.Patients;

public sealed class PatientListItemDto
{
    public int PatientId { get; init; }

    public string MedicalNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateOnly DateOfBirth { get; init; }

    public string GenderCode { get; init; } = string.Empty;

    public string PatientStatusCode { get; init; } = string.Empty;

    public int DepartmentId { get; init; }

    public string DepartmentCode { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;
}