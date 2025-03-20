using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

[PrimaryKey(nameof(MemberId), nameof(ProjectId))]
public class MemberProjectEntity
{
    public int MemberId { get; set; }

    public int ProjectId { get; set; }

    public MemberEntity Member { get; set; } = null!;

    public ProjectEntity Project { get; set; } = null!;
}
