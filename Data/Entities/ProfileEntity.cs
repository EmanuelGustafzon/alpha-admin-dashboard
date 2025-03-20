using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Table("Profiles")]
public class ProfileEntity
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public string? ImageUrl { get; set; }

    [Column(TypeName = "varchar(200)")]
    public string Firstname { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Lastname { get; set; } = null!;
}
