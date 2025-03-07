using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
}
