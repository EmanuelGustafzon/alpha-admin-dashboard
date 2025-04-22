namespace Business.Models;

public class NotificationForm
{
    public string? Icon { get; set; }

    public string Message { get; set; } = null!;

    public string? Action { get; set; }
}
