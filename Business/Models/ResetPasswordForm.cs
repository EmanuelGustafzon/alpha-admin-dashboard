using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ResetPasswordForm
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;
}
