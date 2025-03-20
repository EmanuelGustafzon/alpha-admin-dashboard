using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class MemberAddressEntity()
{
    [Key, ForeignKey("User")]
    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostCode { get; set; } = null!;
}
