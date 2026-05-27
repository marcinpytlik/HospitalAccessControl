using HospitalAccessControl.Application.Patients;
using HospitalAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAccessControl.Infrastructure.Patients;

public sealed class PatientReadService : IPatientReadService
{
    private readonly HospitalAccessControlDbContext _dbContext;

    public PatientReadService(HospitalAccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new PatientListItemDto
            {
                PatientId = x.PatientId,
                MedicalNumber = x.MedicalNumber,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                GenderCode = x.GenderCode,
                PatientStatusCode = x.PatientStatusCode,
                DepartmentId = x.DepartmentId,
                DepartmentCode = x.Department.Code,
                DepartmentName = x.Department.Name
            })
            .ToListAsync(cancellationToken);
    }
}