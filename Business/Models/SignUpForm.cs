using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SignUpForm
{
    [Required]
    [EmailAddress]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid Email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "Must be 6 characters with at least one uppercase letter and one special character.")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords mismatch")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "You must accept the terms.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]
    public bool AcceptTerms { get; set; } = false;
}
