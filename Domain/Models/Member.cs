namespace Domain.Models;

public class Member
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? JobTitle { get; set; }

    public MemberAddress Address { get; set; } = new();
}
