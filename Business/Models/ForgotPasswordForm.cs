using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ForgotPasswordForm
{
    [Required]
    [EmailAddress]
    [Display (Name = "Email")]
    public string Email { get; set; } = null!;
}
