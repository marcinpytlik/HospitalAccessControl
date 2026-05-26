using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecords", "medical");

        builder.HasKey(x => x.MedicalRecordId);

        builder.Property(x => x.RecordTypeCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Diagnosis)
            .HasMaxLength(4000);

        builder.Property(x => x.Treatment)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.MedicalRecords)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.MedicalRecords)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("IX_MedicalRecords_PatientId");

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_MedicalRecords_DepartmentId");

        builder.HasIndex(x => x.RecordTypeCode)
            .HasDatabaseName("IX_MedicalRecords_RecordTypeCode");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_MedicalRecords_CreatedAt");
    }
}