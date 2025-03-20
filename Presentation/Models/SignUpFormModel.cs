using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignUpFormModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "First Name")]
    public string Firstname { get; set; } = null!;

    [Required]
    [Display(Name = "Last Name")]
    public string Lastname { get; set; } = null!;

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords mismatch")]
    public string ConfirmPassword { get; set; } = null!;
}
