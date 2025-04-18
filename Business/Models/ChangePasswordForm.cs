using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ChangePasswordForm
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;

    [Compare(nameof(NewPassword), ErrorMessage = "Passwords mismatch")]
    public string ConfirmPassword { get; set;} = null!;
}
