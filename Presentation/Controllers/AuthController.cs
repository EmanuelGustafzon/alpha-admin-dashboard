using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    public async Task<IActionResult> SignUp()
    {
        var model = new AuthViewModel
        {
            ExternalLogins = await GetExternalLogins()
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp([Bind(Prefix = "SignUpForm")] SignUpFormModel form)
    {
        if (!ModelState.IsValid)
        {
            var model = new AuthViewModel
            {
                SignUpForm = form,
                ExternalLogins = await GetExternalLogins()
            };
            return View(model);
        }

        var user = new UserEntity
        {
            Email = form.Email,
            UserName = form.Email,
            
        };
        var profile = new ProfileEntity
        {
            User = user,
            Firstname = form.Firstname,
            Lastname = form.Lastname
        };

        var result = await _userManager.CreateAsync(user, form.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var model = new AuthViewModel
            {
                SignUpForm = form,
                ExternalLogins = await GetExternalLogins()
            };
            return View(model);
        }
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> SignIn(string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;

        var model = new AuthViewModel
        {
            ExternalLogins = await GetExternalLogins()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([Bind(Prefix = "SignInForm")] SignInFormModel form, string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "Wrong email or password";
            var model = new AuthViewModel
            {
                SignInForm = form,
                ExternalLogins = await GetExternalLogins()
            };
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
        if (!result.Succeeded)
        {
            var model = new AuthViewModel
            {
                SignInForm = form,
                ExternalLogins = await GetExternalLogins(),
                ExternalLoginSignUpForm = new ExternalLoginSignUpForm()
            };
            ModelState.AddModelError("SignInError", "Failed to sign in");
            return View(model);
        }

        if(Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult ExternalLogin(string provider, string returnUrl = "/")
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return RedirectToAction("SignIn", "Auth");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction("SignIn", "Auth");
        }
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        var model = new AuthViewModel
        {
            ExternalLogins = await GetExternalLogins(),
            ExternalLoginSignUpForm = new ExternalLoginSignUpForm(),
        };
        return View(model);
    }
    public async Task<IActionResult> AddUserFromExternalProvider(ExternalLoginSignUpForm form)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        var user = new UserEntity
        {
            Email = info?.Principal.FindFirstValue(ClaimTypes.Email),
            UserName = info?.Principal.FindFirstValue(ClaimTypes.Email),
        };
        var profile = new ProfileEntity
        {
            User = user,
            Firstname = form.Firstname,
            Lastname = form.Lastname
        };
        var result = await _userManager.CreateAsync(user);
        return RedirectToAction("Index", "Home");
    }
    private async Task<IList<AuthenticationScheme>> GetExternalLogins()
    {
        return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }
}
