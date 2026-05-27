using HospitalAccessControl.Application.Common.Security;
using HospitalAccessControl.Application.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISqlSessionContextDiagnostics _sqlSessionContextDiagnostics;

    public IndexModel(
        ICurrentUserService currentUserService,
        ISqlSessionContextDiagnostics sqlSessionContextDiagnostics)
    {
        _currentUserService = currentUserService;
        _sqlSessionContextDiagnostics = sqlSessionContextDiagnostics;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public string? SqlSessionCurrentUser { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        CurrentUser = _currentUserService.GetCurrentUser();

        SqlSessionCurrentUser =
            await _sqlSessionContextDiagnostics.GetCurrentUserFromSessionContextAsync(cancellationToken);
    }
}