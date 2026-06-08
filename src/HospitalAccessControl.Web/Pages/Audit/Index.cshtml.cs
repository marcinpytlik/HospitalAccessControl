using HospitalAccessControl.Application.Audit;
using HospitalAccessControl.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.Audit;

public class IndexModel : PageModel
{
    private readonly IAuditReadService _auditReadService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRoleReadService _userRoleReadService;

    public IndexModel(
        IAuditReadService auditReadService,
        ICurrentUserService currentUserService,
        IUserRoleReadService userRoleReadService)
    {
        _auditReadService = auditReadService;
        _currentUserService = currentUserService;
        _userRoleReadService = userRoleReadService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public bool AccessDenied { get; private set; }

    public IReadOnlyList<AccessLogListItemDto> LatestEvents { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public IReadOnlyList<AccessLogListItemDto> FailedAttempts { get; private set; }
        = Array.Empty<AccessLogListItemDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        var roles = await _userRoleReadService.GetRoleCodesAsync(
            currentUser.DomainLogin,
            cancellationToken);

        CurrentUser = new CurrentUserDto
        {
            DomainLogin = currentUser.DomainLogin,
            SamAccountName = currentUser.SamAccountName,
            DisplayName = currentUser.DisplayName,
            IsAuthenticated = currentUser.IsAuthenticated,
            Roles = roles
        };

        if (!CurrentUser.HasRole("Auditor") &&
            !CurrentUser.HasRole("ITAdministrator"))
        {
            AccessDenied = true;
            return Page();
        }

        LatestEvents = await _auditReadService.GetLatestAsync(100, cancellationToken);
        FailedAttempts = await _auditReadService.GetFailedAttemptsAsync(50, cancellationToken);

        return Page();
    }
}
