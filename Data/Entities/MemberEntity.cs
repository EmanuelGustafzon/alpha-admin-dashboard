using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class MemberEntity
{
    public byte[] Image { get; set; } = [];

    [Column(TypeName = "varchar(200)")]
    public string FirstName { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string LastName { get; set; } = null!;

    public ICollection<MemberProjectEntity> MemberProjects { get; set; } = new List<MemberProjectEntity>();
}
