using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ExternalLoginSignUpForm
{
    [Required]
    public string Firstname { get; set; } = null!;

    [Required]
    public string Lastname { get; set; } = null!;
}
