using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignInFormModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
