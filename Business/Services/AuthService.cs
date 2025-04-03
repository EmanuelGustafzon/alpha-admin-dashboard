using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;

namespace Business.Services;

public class AuthService(
    UserManager<MemberEntity> userManager, 
    SignInManager<MemberEntity> signInManager, 
    IMemberService memberService
    ) : IAuthService
{
    private readonly UserManager<MemberEntity> _userManager = userManager;
    private readonly SignInManager<MemberEntity> _signInManager = signInManager;
    private readonly IMemberService _memberService = memberService;
    public async Task<ServiceResult<MemberEntity>> SignUpAsync(SignUpForm form)
    {
        if (form == null)
            return ServiceResult<MemberEntity>.BadRequest("A sign up form need to be provided");
        try
        {
            ServiceResult<MemberEntity> result = await _memberService.CreateMemberAsync(form.FirstName, form.LastName, form.Email, form.Password);
            if(!result.Success || result.Data is null)
                return ServiceResult<MemberEntity>.Error(result.ErrorMessage ?? "Failed to create account");

            return ServiceResult<MemberEntity>.Created(result.Data);

        } catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<MemberEntity>.Error("Failed to sign up");
        }
    }
    public async Task<ServiceResult<MemberEntity>> SignInAsync(SignInForm form)
    {
        try
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
            if (result.Succeeded)
            {
                return ServiceResult<MemberEntity>.NoContent();
            }
            return ServiceResult<MemberEntity>.Error("Failed to login user");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<MemberEntity>.Error("Failed to login user");
        }
    }

    public async Task SignOutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

    }
    public ServiceResult<AuthenticationProperties> ConfigureExternalAuthProps(string provider, string redirectUrl)
    {
        if (provider == null) return ServiceResult<AuthenticationProperties>.Error("No auth provider was provided");
        if (redirectUrl == null) return ServiceResult<AuthenticationProperties>.Error("No redirect url was provided");

        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return ServiceResult<AuthenticationProperties>.Ok(properties);
    }
    public async Task<ServiceResult<MemberEntity>> ExternalAuthSignInAsync()
    {
        try
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return ServiceResult<MemberEntity>.Error("No external login info was found");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return ServiceResult<MemberEntity>.NoContent();
            }
            return ServiceResult<MemberEntity>.NotFound("Account needs to be registered");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return ServiceResult<MemberEntity>.Error("No external login info was found");
        }
    }
    public async Task<ServiceResult<MemberEntity>> ExternalAuthSignUpAsync(ExternalAuthSignUpForm form)
    {
        if (form == null)
            return ServiceResult<MemberEntity>.BadRequest("A sign up form need to be provided");
        try
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return ServiceResult<MemberEntity>.Error("No external login info was found");

            string? email = info.Principal?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return ServiceResult<MemberEntity>.Error("Email not found in external login provider claims.");


            ServiceResult<MemberEntity> result = await _memberService.CreateMemberAsync(form.FirstName, form.LastName, email);
            if (!result.Success || result.Data is null)
            {
                return ServiceResult<MemberEntity>.Error(result.ErrorMessage ?? "Failed to create account");
            }


            var addAccountResult = await _userManager.AddLoginAsync(result.Data, info);
            if (!addAccountResult.Succeeded)
            {
                return ServiceResult<MemberEntity>.Error("Failed to add external login user.");
            }
            return ServiceResult<MemberEntity>.Created(result.Data);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return ServiceResult<MemberEntity>.Error("Failed to Create User");
        }
    }
    public async Task<IList<AuthenticationScheme>> GetExternalLogins()
    {
        return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }
}
