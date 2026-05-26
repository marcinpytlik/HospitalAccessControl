using HospitalAccessControl.Infrastructure.Data.Seed;
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoleAssignments", "security");

        builder.HasKey(x => x.UserRoleAssignmentId);

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.ValidTo);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.RoleAssignments)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApplicationRole)
            .WithMany(x => x.UserRoleAssignments)
            .HasForeignKey(x => x.ApplicationRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationUserId)
            .HasDatabaseName("IX_UserRoleAssignments_ApplicationUserId");

        builder.HasIndex(x => x.ApplicationRoleId)
            .HasDatabaseName("IX_UserRoleAssignments_ApplicationRoleId");

        builder.HasIndex(x => new { x.ApplicationUserId, x.ApplicationRoleId, x.IsActive })
            .HasDatabaseName("IX_UserRoleAssignments_User_Role_Active");
         builder.HasData(UserRoleAssignmentSeed.Data);   
    }
}