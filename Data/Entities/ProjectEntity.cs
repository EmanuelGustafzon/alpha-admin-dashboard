using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{      
    public int Id { get; set; }

    [Column(TypeName = "varchar(200)")]
    public string ProjectName { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string ClientName { get; set; } = null!;

    [Column(TypeName = "varchar(800)")]
    public string Description { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Budget { get; set; }

    public ProjectStatus status { get; set; }

    public int ClientId { get; set; }

    public ClientEntity Client { get; set; } = null!;

    public ICollection<MemberProjectEntity> MemberProjects { get; set; } = new List<MemberProjectEntity>();

}

public enum ProjectStatus
{
    NotStarted,
    Started,
    Completed
}