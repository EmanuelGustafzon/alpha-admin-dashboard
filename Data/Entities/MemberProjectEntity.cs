using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("MemberProjects")]
[PrimaryKey("ProjectId", "MemberId")]
public class MemberProjectEntity
{
    public string MemberId { get; set; } = null!;

    public string ProjectId { get; set; } = null!;

    public MemberEntity Member { get; set; } = null!;

    public ProjectEntity Project { get; set; } = null!;
}
