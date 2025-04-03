using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;

namespace Business.Interfaces;

public interface IAuthService
{
    public Task<ServiceResult<MemberEntity>> SignUpAsync(SignUpForm form);
    public Task<ServiceResult<MemberEntity>> SignInAsync(SignInForm form);
    public Task SignOutAsync();
    public ServiceResult<AuthenticationProperties> ConfigureExternalAuthProps(string provider, string redirectUrl);
    public Task<ServiceResult<MemberEntity>> ExternalAuthSignInAsync();
    public Task<ServiceResult<MemberEntity>> ExternalAuthSignUpAsync(ExternalAuthSignUpForm form);
    public Task<IList<AuthenticationScheme>> GetExternalLogins();
}
