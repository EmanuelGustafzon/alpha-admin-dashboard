using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class MemberContactEntity
{
    [Key, ForeignKey("User")]
    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }
}
