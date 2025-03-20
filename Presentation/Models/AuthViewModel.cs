using Microsoft.AspNetCore.Authentication;

namespace Presentation.Models;

public class AuthViewModel
{
    public SignUpFormModel SignUpForm { get; set; } = new();
    public SignInFormModel SignInForm { get; set; } = new();

    public ExternalLoginSignUpForm ExternalLoginSignUpForm { get; set; } = new();
    public IList<AuthenticationScheme>? ExternalLogins { get; set; } 
}
