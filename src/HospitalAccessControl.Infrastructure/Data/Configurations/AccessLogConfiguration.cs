using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLog", "audit");

        builder.HasKey(x => x.AccessLogId);

        builder.Property(x => x.DomainLogin)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ActionCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ObjectName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.AccessDate)
            .IsRequired();

        builder.Property(x => x.ClientHost)
            .HasMaxLength(256);

        builder.Property(x => x.ApplicationName)
            .HasMaxLength(256);

        builder.Property(x => x.WasSuccessful)
            .IsRequired();

        builder.Property(x => x.AdditionalInfo)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.DomainLogin)
            .HasDatabaseName("IX_AccessLog_DomainLogin");

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_AccessLog_PatientId");

        builder.HasIndex(x => x.AccessDate)
            .HasDatabaseName("IX_AccessLog_AccessDate");

        builder.HasIndex(x => x.ActionCode)
            .HasDatabaseName("IX_AccessLog_ActionCode");

        builder.HasIndex(x => x.WasSuccessful)
            .HasDatabaseName("IX_AccessLog_WasSuccessful");
    }
}