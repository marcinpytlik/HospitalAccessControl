using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class IndexModel : PageModel
{
    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public IReadOnlyList<PatientListItemDto> Patients { get; private set; }
        = Array.Empty<PatientListItemDto>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        Patients = await _patientReadService.GetPatientsAsync(cancellationToken);
    }
}