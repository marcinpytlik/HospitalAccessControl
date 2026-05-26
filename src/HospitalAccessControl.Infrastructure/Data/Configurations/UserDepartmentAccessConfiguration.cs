using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class UserDepartmentAccessConfiguration : IEntityTypeConfiguration<UserDepartmentAccess>
{
    public void Configure(EntityTypeBuilder<UserDepartmentAccess> builder)
    {
        builder.ToTable("UserDepartmentAccess", "security");

        builder.HasKey(x => x.UserDepartmentAccessId);

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
            .WithMany(x => x.DepartmentAccesses)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.UserDepartmentAccesses)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationUserId)
            .HasDatabaseName("IX_UserDepartmentAccess_ApplicationUserId");

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_UserDepartmentAccess_DepartmentId");

        builder.HasIndex(x => new { x.ApplicationUserId, x.DepartmentId, x.IsActive })
            .HasDatabaseName("IX_UserDepartmentAccess_User_Department_Active");
    }
}