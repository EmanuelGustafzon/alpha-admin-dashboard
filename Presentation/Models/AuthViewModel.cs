using Business.Models;
using Microsoft.AspNetCore.Authentication;

namespace Presentation.Models;

public class AuthViewModel
{
    public SignUpForm SignUpForm { get; set; } = new();
    public SignInForm SignInForm { get; set; } = new();
    public ExternalAuthSignUpForm ExternalAuthSignUpForm { get; set; } = new();
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    public ForgotPasswordForm ForgotPasswordForm { get; set; } = new();
    public ResetPasswordForm ResetPasswordForm { get; set; } = new();
}
