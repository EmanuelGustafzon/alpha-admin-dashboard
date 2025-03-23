using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("MemberProfiles")]
public class MemberProfileEntity
{
    [Key, ForeignKey("Member")]
    public string MemberId { get; set; } = null!;
    public MemberEntity Member { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Firstname { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Lastname { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime? BirthDate { get; set; }

    public MemberAddressEntity? Address { get; set; }
}
