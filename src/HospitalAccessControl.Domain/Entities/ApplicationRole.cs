namespace HospitalAccessControl.Domain.Entities;

public sealed class ApplicationRole
{
    public int ApplicationRoleId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
}