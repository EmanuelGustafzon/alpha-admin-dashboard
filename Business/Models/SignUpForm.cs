using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SignUpForm
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email", Prompt = "Write your Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords mismatch")]
    public string ConfirmPassword { get; set; } = null!;
}
