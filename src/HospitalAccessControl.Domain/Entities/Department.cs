namespace HospitalAccessControl.Domain.Entities;

public sealed class Department
{
    public int DepartmentId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public ICollection<UserDepartmentAccess> UserDepartmentAccesses { get; set; } = new List<UserDepartmentAccess>();
}