using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public enum NotificationType
{
    Info,
    Warning,
    Plateau,
    Overload
}

public class Notification
{
    public int NotificationId { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public int RelatedId { get; set; }

    public DateTime DateCreated { get; set; }

    public bool IsRead { get; set; }
}
