namespace HospitalAccessControl.Application.Patients;

public interface IPatientReadService
{
    Task<IReadOnlyList<PatientListItemDto>> GetPatientsAsync(
        CancellationToken cancellationToken = default);

    Task<PatientDetailsDto?> GetPatientDetailsAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}