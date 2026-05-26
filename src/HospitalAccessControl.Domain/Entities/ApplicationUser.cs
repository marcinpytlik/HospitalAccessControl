namespace HospitalAccessControl.Domain.Entities;

public sealed class ApplicationUser
{
    public int ApplicationUserId { get; set; }

    public string DomainLogin { get; set; } = string.Empty;

    public string SamAccountName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<UserDepartmentAccess> DepartmentAccesses { get; set; } = new List<UserDepartmentAccess>();

    public ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
}