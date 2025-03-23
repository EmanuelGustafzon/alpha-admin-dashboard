using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class MemberEntity : IdentityUser
{
    public MemberProfileEntity? Profile { get; set; }
}
