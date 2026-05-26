using HospitalAccessControl.Domain.Entities;

namespace HospitalAccessControl.Infrastructure.Data.Seed;

public static class UserRoleAssignmentSeed
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<UserRoleAssignment> Data => new List<UserRoleAssignment>
    {
        new() { UserRoleAssignmentId = 1, ApplicationUserId = 1, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 2, ApplicationUserId = 2, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 3, ApplicationUserId = 3, ApplicationRoleId = 1, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 4, ApplicationUserId = 4, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 5, ApplicationUserId = 5, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 6, ApplicationUserId = 6, ApplicationRoleId = 2, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 7, ApplicationUserId = 7, ApplicationRoleId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 8, ApplicationUserId = 8, ApplicationRoleId = 3, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 9, ApplicationUserId = 9, ApplicationRoleId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 10, ApplicationUserId = 10, ApplicationRoleId = 4, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },

        new() { UserRoleAssignmentId = 11, ApplicationUserId = 11, ApplicationRoleId = 5, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" },
        new() { UserRoleAssignmentId = 12, ApplicationUserId = 12, ApplicationRoleId = 6, ValidFrom = CreatedAt, IsActive = true, CreatedAt = CreatedAt, CreatedBy = "seed" }
    };
}