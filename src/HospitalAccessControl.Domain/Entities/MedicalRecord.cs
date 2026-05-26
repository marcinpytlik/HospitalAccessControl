using HospitalAccessControl.Domain.Common;

namespace HospitalAccessControl.Domain.Entities;

public sealed class MedicalRecord : AuditableEntity
{
    public int MedicalRecordId { get; set; }

    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public string RecordTypeCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public bool IsDeleted { get; set; }
}