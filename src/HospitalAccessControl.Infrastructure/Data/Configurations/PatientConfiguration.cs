using HospitalAccessControl.Infrastructure.Data.Seed;
using HospitalAccessControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAccessControl.Infrastructure.Data.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients", "medical");

        builder.HasKey(x => x.PatientId);

        builder.Property(x => x.MedicalNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Pesel)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.GenderCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.PatientStatusCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Patients)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DepartmentId)
            .HasDatabaseName("IX_Patients_DepartmentId");

        builder.HasIndex(x => x.MedicalNumber)
            .IsUnique()
            .HasDatabaseName("UQ_Patients_MedicalNumber");

        builder.HasIndex(x => new { x.LastName, x.FirstName })
            .HasDatabaseName("IX_Patients_LastName_FirstName");

        builder.HasIndex(x => x.Pesel)
            .HasDatabaseName("IX_Patients_Pesel");

        builder.HasIndex(x => x.PatientStatusCode)
            .HasDatabaseName("IX_Patients_Status");
        builder.HasData(PatientSeed.Data);
    }
}