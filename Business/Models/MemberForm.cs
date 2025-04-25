using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Business.Models;

public class MemberForm
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Phone]
    [Display(Name = "Phone")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }
    public string ImageUrl { get; set; } = "/images/default-profile-picture.png";

    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Birth Date")]
    public DateTime? BirthDate { get; set; }

    public MemberAddressForm AddressForm { get; set; } = new();
}
public class MemberAddressForm
{
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Post Code")]
    public string? PostCode { get; set; }

    [Display(Name = "Street")]
    public string? Street { get; set; } 
}