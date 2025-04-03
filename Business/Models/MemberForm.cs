using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Business.Models;

public class MemberForm
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; }
    public string? JobTitle { get; set; }

    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    public MemberAddressForm AddressForm { get; set; } = new();
}
public class MemberAddressForm
{
    public string City { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string Street { get; set; } = null!;
}