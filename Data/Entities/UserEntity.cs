using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    //public ProfileEntity Profile { get; set; } = null!;
    [Column(TypeName = "varchar(200)")]
    public string Firstname { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Lastname { get; set; } = null!;
}
