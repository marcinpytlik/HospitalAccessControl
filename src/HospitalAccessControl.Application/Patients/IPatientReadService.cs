namespace HospitalAccessControl.Application.Patients;

public interface IPatientReadService
{
    Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default);
}