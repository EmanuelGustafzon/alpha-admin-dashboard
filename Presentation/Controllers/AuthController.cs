using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using Presentation.Hubs;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers;

public class AuthController(IAuthService authService, IMemberService memberService, INotificationService notificationService, IHubContext<NotificationHub> hub) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IMemberService _memberService = memberService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _hub = hub;
    public async Task<IActionResult> SignUp()
    {
        var model = new AuthViewModel
        {
            ExternalLogins = await _authService.GetExternalLogins()
        };
        
        return View(model);
    }
    public IActionResult TermsAndConditions()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([Bind(Prefix = "SignUpForm")] SignUpForm form)
    {
        ViewBag.ErrorMessage = null;

        var model = new AuthViewModel
        {
            SignUpForm = form,
            ExternalLogins = await _authService.GetExternalLogins()
        };

        if (!ModelState.IsValid) return View(model);

        var result = await _authService.SignUpAsync(form);

        if(result.StatusCode == 201) return RedirectToAction("SignIn", "Auth");

        ViewBag.ErrorMessage = result.ErrorMessage;
        return View(model);  
    }

    public async Task<IActionResult> SignIn(string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;

        var model = new AuthViewModel
        {
            ExternalLogins = await _authService.GetExternalLogins()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([Bind(Prefix = "SignInForm")] SignInForm form, string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "Wrong email or password";
            var model = new AuthViewModel
            {
                SignInForm = form,
                ExternalLogins = await _authService.GetExternalLogins()
            };
            return View(model);
        }

        var result = await _authService.SignInAsync(form);
        if (result.StatusCode != 204)
        {
            var model = new AuthViewModel
            {
                SignInForm = form,
                ExternalLogins = await _authService.GetExternalLogins(),
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

        var result = _authService.ConfigureExternalAuthProps(provider, redirectUrl);
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
        // try to sign in account
        var result = await _authService.ExternalAuthSignInAsync();

        if (result.StatusCode == 204)
        {
            return LocalRedirect(returnUrl);
        }
        // try to create account
        if (result.StatusCode == 404)
        {
            var model = new AuthViewModel
            {
                ExternalLogins = await _authService.GetExternalLogins(),
                ExternalAuthSignUpForm = new ExternalAuthSignUpForm()
            };
            return View("ExternalAuthSignUp", model);
        }

        return RedirectToAction("SignIn", "Auth");
    }
    public async Task<IActionResult> AddUserFromExternalProvider([Bind(Prefix = "ExternalAuthSignUpForm")] ExternalAuthSignUpForm form)
    {
        var result = await _authService.ExternalAuthSignUpAsync(form);

        if (result.StatusCode == 201) return RedirectToAction("index", "Home");

        ViewBag.ErrorMessage = result.ErrorMessage;
        var model = new AuthViewModel
        {
            ExternalLogins = await _authService.GetExternalLogins()
        };
        return View("SignUp", model);
    }

    public new async Task<IActionResult> SignOut()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }
    public IActionResult ForgotPassword()
    {
        var model = new AuthViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword([Bind(Prefix = "ForgotPasswordForm")] ForgotPasswordForm form)
    {
        if (!ModelState.IsValid)
            return View(form);

        var memberResult = await _memberService.GetMemberByEmailAsync(form.Email);
        if (memberResult.Data is null) return RedirectToAction(nameof(ForgotPasswordConfirmation));

        var token = await _authService.GeneratePasswordResetTokenAsync(form.Email);
        if (token == null)
            return RedirectToAction(nameof(ForgotPasswordConfirmation));

        var callback = Url.Action(nameof(ResetPassword), "Auth", new { token, email = form.Email }, Request.Scheme);

        NotificationForm notificationForm = NotificationFactory.CreateForm($"{memberResult.Data.FirstName} {memberResult.Data.LastName} wants to reset password", "Admin", memberResult.Data.ImageUrl ?? "/images/default-profile-picture.png", callback);
        var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
        if (notificationResult.Data is not null)
        {
            var adminsResult = await _memberService.GetAllAdminsAsync();
            if(adminsResult.Data is not null)
            {
                foreach (var admin in adminsResult.Data)
                {
                    await _hub.Clients.User(admin.Id).SendAsync("adminNotifications", notificationResult.Data);
                }
            }
        }

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    public IActionResult ResetPassword(string token, string email)
    {
        var model = new AuthViewModel();
        model.ResetPasswordForm.Token = token;
        model.ResetPasswordForm.Email = email;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword([Bind(Prefix = "ResetPasswordForm")] ResetPasswordForm form)
    {
        if (!ModelState.IsValid)
        {
            var model = new AuthViewModel();
            model.ResetPasswordForm.Token = form.Token;
            model.ResetPasswordForm.Email = form.Email;
            return View(model);
        }

        bool result = await  _authService.RestorePasswordAsync(form.Email, form.Token, form.Password);

        return RedirectToAction("SignIn", "Auth");
    }
}
