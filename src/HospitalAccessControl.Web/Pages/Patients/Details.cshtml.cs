using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class DetailsModel : PageModel
{
    private const string ViewPatientDetailsAction = "ViewPatientDetails";

    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public DetailsModel(
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
        _auditService = auditService;
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

        var wasSuccessful = Patient is not null;

        if (!wasSuccessful)
        {
            AccessDeniedOrNotFound = true;
        }

        await _auditService.LogAccessAsync(
            new AccessLogCreateDto
            {
                DomainLogin = CurrentUser.DomainLogin,
                PatientId = id,
                ActionCode = ViewPatientDetailsAction,
                ObjectName = "medical.Patients",
                ClientHost = HttpContext.Connection.RemoteIpAddress?.ToString(),
                ApplicationName = "HospitalAccessControl.Web",
                WasSuccessful = wasSuccessful,
                AdditionalInfo = wasSuccessful
                    ? "Patient details viewed."
                    : "Patient not found or access denied."
            },
            cancellationToken);

        return Page();
    }
}