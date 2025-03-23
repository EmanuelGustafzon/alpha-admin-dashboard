using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

[Table("MemberAddresses")]
public class MemberAddressEntity()
{
    [Key, ForeignKey("MemberProfile")]
    public string MemberProfileId { get; set; } = null!;

    public MemberProfileEntity MemberProfile { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string PostCode { get; set; } = null!;
}
