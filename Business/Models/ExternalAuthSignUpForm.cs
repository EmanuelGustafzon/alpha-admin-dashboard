using System.ComponentModel.DataAnnotations;


namespace Business.Models
{
    public class ExternalAuthSignUpForm
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;
    }
}
