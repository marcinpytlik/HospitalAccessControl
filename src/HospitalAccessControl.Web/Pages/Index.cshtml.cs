using HospitalAccessControl.Application.Common.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalAccessControl.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public CurrentUserDto CurrentUser { get; private set; } = new();

    public void OnGet()
    {
        CurrentUser = _currentUserService.GetCurrentUser();
    }
}