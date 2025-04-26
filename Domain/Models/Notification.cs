namespace Domain.Models;

public class Notification
{
    public string Id { get; set; } = null!;

    public string? Icon { get; set; }

    public string Message { get; set; } = null!;

    public string? Action { get; set; }

    public DateTime Created { get; set; } 
}
