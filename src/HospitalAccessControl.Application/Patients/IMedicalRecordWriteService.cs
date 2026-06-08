namespace HospitalAccessControl.Application.Patients;

public interface IMedicalRecordWriteService
{
    Task<bool> CreateAsync(
        CreateMedicalRecordDto dto,
        CancellationToken cancellationToken = default);
}
