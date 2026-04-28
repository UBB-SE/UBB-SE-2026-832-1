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

    [Required]
    public Client Client { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public NotificationType Type { get; set; }

    public int RelatedId { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }

    [Required]
    public bool IsRead { get; set; }
}
