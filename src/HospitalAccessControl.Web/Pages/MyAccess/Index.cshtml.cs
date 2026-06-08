using HospitalAccessControl.Application.Common.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages.MyAccess;

public class IndexModel : PageModel
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentUserAccessReadService _accessReadService;

    public IndexModel(
        ICurrentUserService currentUserService,
        ICurrentUserAccessReadService accessReadService)
    {
        _currentUserService = currentUserService;
        _accessReadService = accessReadService;
    }

    public CurrentUserAccessDto Access { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();
        Access = await _accessReadService.GetAsync(user.DomainLogin, cancellationToken);
    }
}
