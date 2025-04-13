using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("Clients")]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "varchar(200)")]
    public string ClientName { get; set; } = null!;

    public ICollection<ProjectEntity>? Projects { get; set; }
}
