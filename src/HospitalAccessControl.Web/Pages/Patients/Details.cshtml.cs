using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class DetailsModel : PageModel
{
    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;

    public DetailsModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public PatientDetailsDto? Patient { get; private set; }

    public bool AccessDeniedOrNotFound { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        Patient = await _patientReadService.GetPatientDetailsAsync(
            id,
            cancellationToken);

        if (Patient is null)
        {
            AccessDeniedOrNotFound = true;
        }

        return Page();
    }
}