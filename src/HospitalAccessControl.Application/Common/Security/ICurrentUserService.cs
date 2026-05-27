namespace HospitalAccessControl.Application.Common.Security;

public interface ICurrentUserService
{
    CurrentUserDto GetCurrentUser();
}