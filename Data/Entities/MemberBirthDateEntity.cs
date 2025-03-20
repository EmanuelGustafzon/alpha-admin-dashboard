using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class MemberBirthDateEntity
{
    [Key, ForeignKey("User")]
    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}
