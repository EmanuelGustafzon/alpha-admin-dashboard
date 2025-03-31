using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

[Table("MemberAddresses")]
public class MemberAddressEntity()
{
    [Key, ForeignKey("Member")]
    public string MemberId { get; set; } = null!;

    public MemberEntity Member { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string City { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string PostCode { get; set; } = null!;

    [Column(TypeName = "varchar(300)")]
    public string Street { get; set; } = null!;
}
