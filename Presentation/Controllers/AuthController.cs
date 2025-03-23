using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class AuthController(UserManager<MemberEntity> userManager, SignInManager<MemberEntity> signInManager, IUserService userService) : Controller
{
    private readonly UserManager<MemberEntity> _userManager = userManager;
    private readonly SignInManager<MemberEntity> _signInManager = signInManager;
    private readonly IUserService _userService = userService;
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
    public async Task<IActionResult> SignUp([Bind(Prefix = "SignUpForm")] SignUpForm form)
    {
        ViewBag.ErrorMessage = null;

        var model = new AuthViewModel
        {
            SignUpForm = form,
            ExternalLogins = await GetExternalLogins()
        };

        if (!ModelState.IsValid) return View(model);

        var result = await _userService.CreateUserAsync(form);

        if(result.StatusCode == 201) return RedirectToAction("SignIn", "Auth");

        ViewBag.ErrorMessage = result.ErrorMessage;
        return View(model);  
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

        var result = await _userService.SignInUserAsync(form.Email, form.Password);
        if (result.StatusCode != 204)
        {
            var model = new AuthViewModel
            {
                SignInForm = form,
                ExternalLogins = await GetExternalLogins(),
                ExternalAuthSignUpForm = new ExternalAuthSignUpForm()
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
        var result = _userService.ConfigureExternalAuthProps(provider, redirectUrl);
        if(result.Data is AuthenticationProperties)
        {
            return Challenge(result.Data, provider);
        }
        return RedirectToAction("SignIn", "Auth");
    }

    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(remoteError, $"Error from external provider: {remoteError}");
            return RedirectToAction("SignIn", "Auth");
        }
        // try to sign in user
        var result = await _userService.ExternalAuthSignIn();

        if (result.StatusCode == 204)
        {
            return LocalRedirect(returnUrl);
        }
        // try to create user
        if (result.StatusCode == 404)
        {
            var model = new AuthViewModel
            {
                ExternalLogins = await GetExternalLogins(),
                ExternalAuthSignUpForm = new ExternalAuthSignUpForm()
            };
            return View("ExternalAuthSignUp", model);
        }

        return RedirectToAction("SignIn", "Auth");
    }
    public async Task<IActionResult> AddUserFromExternalProvider([Bind(Prefix = "ExternalAuthSignUpForm")] ExternalAuthSignUpForm form)
    {
        var result = await _userService.ExternalAuthSignUp(form);

        if (result.StatusCode == 201) return RedirectToAction("index", "Home");

        ViewBag.ErrorMessage = result.ErrorMessage;
        var model = new AuthViewModel
        {
            ExternalLogins = await GetExternalLogins()
        };
        return View("SignUp", model);
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
