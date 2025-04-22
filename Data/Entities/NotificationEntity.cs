using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();


    [Column(TypeName = "varchar(400)")]

    public string? Icon { get; set; }


    [Column(TypeName = "varchar(100)")]

    public string Message { get; set; } = null!;


    [Column(TypeName = "varchar(100)")]

    public string? Action { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public ICollection<NotificationDissmissEntity> DissmissedNotifications { get; set; } = [];
}
