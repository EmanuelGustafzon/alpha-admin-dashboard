using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProfileEntity
{
    [Key]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public UserEntity User { get; set; } = null!;
    public byte[]? Image { get; set; }

    [Column(TypeName = "varchar(200)")]
    public string Firstname { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    public string Lastname { get; set; } = null!;
}
