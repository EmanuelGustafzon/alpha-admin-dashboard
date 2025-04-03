using System.ComponentModel.DataAnnotations;


namespace Business.Models
{
    public class ExternalAuthSignUpForm
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
    }
}
