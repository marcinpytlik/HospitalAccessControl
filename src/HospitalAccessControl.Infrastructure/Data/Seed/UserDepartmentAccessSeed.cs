using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class UserDepartmentAccessSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<UserDepartmentAccess> Data => new List<UserDepartmentAccess>
    {
        new() { UserDepartmentAccessId = 1, ApplicationUserId = 1, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 2, ApplicationUserId = 2, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 3, ApplicationUserId = 3, DepartmentId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 4, ApplicationUserId = 4, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 5, ApplicationUserId = 5, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 6, ApplicationUserId = 6, DepartmentId = 5, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 7, ApplicationUserId = 8, DepartmentId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 8, ApplicationUserId = 9, DepartmentId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserDepartmentAccessId = 9, ApplicationUserId = 10, DepartmentId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" }
    };
}