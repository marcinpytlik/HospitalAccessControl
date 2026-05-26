namespace HospitalAccessControl.Domain.Entities;

public sealed class UserDepartmentAccess
{
    public int UserDepartmentAccessId { get; set; }

    public int ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}