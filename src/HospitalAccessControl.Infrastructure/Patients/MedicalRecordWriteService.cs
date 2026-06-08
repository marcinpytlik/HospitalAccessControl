using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Domain.Entities;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Patients;

public sealed class MedicalRecordWriteService : IMedicalRecordWriteService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public MedicalRecordWriteService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(
        CreateMedicalRecordDto dto,
        CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients
            .Where(x => !x.IsDeleted)
            .SingleOrDefaultAsync(x => x.PatientId == dto.PatientId, cancellationToken);

        if (patient is null)
        {
            return false;
        }

        var record = new MedicalRecord
        {
            PatientId = patient.PatientId,
            DepartmentId = patient.DepartmentId,
            RecordTypeCode = string.IsNullOrWhiteSpace(dto.RecordTypeCode)
                ? "OBSERVATION"
                : dto.RecordTypeCode,
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Diagnosis = dto.Diagnosis,
            Treatment = dto.Treatment,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy,
            IsDeleted = false
        };

        _dbContext.MedicalRecords.Add(record);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
