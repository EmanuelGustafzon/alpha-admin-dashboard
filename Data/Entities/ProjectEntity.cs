using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("Projects")]
public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "varchar(300)")]
    public string Owner { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "varchar(200)")]
    public string ProjectName { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Description { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "money")]
    public decimal Budget { get; set; }

    public ProjectStatuses Status { get; set; }

    public string ClientId { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;

    public ICollection<MemberProjectEntity> MemberProjects { get; set; } = [];
}
