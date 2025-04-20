using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ChangePasswordForm
{
    [Display(Name = "Old Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = null!;

    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!;

    [Compare(nameof(NewPassword), ErrorMessage = "Passwords mismatch")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set;} = null!;
}
