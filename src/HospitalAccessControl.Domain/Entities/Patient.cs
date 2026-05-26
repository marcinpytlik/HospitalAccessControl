using HospitalAccessControl.Domain.Common;

namespace HospitalAccessControl.Domain.Entities;

public sealed class Patient : AuditableEntity
{
    public int PatientId { get; set; }

    public string MedicalNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Pesel { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string GenderCode { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public string PatientStatusCode { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}