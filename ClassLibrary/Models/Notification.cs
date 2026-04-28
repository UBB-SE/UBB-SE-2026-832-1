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
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

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
