using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Patients;

public class AddRecordModel : PageModel
{
    private readonly IMedicalRecordWriteService _writeService;
    private readonly IPatientReadService _patientReadService;
    private readonly ICurrentUserService _currentUserService;

    public AddRecordModel(
        IMedicalRecordWriteService writeService,
        IPatientReadService patientReadService,
        ICurrentUserService currentUserService)
    {
        _writeService = writeService;
        _patientReadService = patientReadService;
        _currentUserService = currentUserService;
    }

    public int PatientId { get; private set; }

    public PatientDetailsDto? Patient { get; private set; }

    public bool AccessDeniedOrNotFound { get; private set; }

    [BindProperty]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public string RecordTypeCode { get; set; } = "OBSERVATION";

    [BindProperty]
    public string? Description { get; set; }

    [BindProperty]
    public string? Diagnosis { get; set; }

    [BindProperty]
    public string? Treatment { get; set; }

    public async Task OnGetAsync(int id, CancellationToken cancellationToken)
    {
        PatientId = id;
        Patient = await _patientReadService.GetPatientDetailsAsync(id, cancellationToken);
        AccessDeniedOrNotFound = Patient is null;
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        PatientId = id;
        Patient = await _patientReadService.GetPatientDetailsAsync(id, cancellationToken);

        if (Patient is null)
        {
            AccessDeniedOrNotFound = true;
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            ModelState.AddModelError(nameof(Title), "Tytuł jest wymagany.");
            return Page();
        }

        var user = _currentUserService.GetCurrentUser();

        var success = await _writeService.CreateAsync(
            new CreateMedicalRecordDto
            {
                PatientId = id,
                RecordTypeCode = RecordTypeCode,
                Title = Title,
                Description = Description,
                Diagnosis = Diagnosis,
                Treatment = Treatment,
                CreatedBy = user.DomainLogin
            },
            cancellationToken);

        if (!success)
        {
            AccessDeniedOrNotFound = true;
            return Page();
        }

        return RedirectToPage("/Patients/Details", new { id });
    }
}
