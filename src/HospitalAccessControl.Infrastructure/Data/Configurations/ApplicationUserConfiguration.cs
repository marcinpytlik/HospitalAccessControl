using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers", "security");

        builder.HasKey(x => x.ApplicationUserId);

        builder.Property(x => x.DomainLogin)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SamAccountName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.DomainLogin)
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationUsers_DomainLogin");

        builder.HasIndex(x => x.SamAccountName)
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationUsers_SamAccountName");
    }
}