namespace ClassLibrary.DTOs;

public sealed class NotificationDataTransferObject
{
    public int NotificationId { get; set; }

    public ClientDataTransferObject Client { get; set; } = new();

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int RelatedId { get; set; }

    public DateTime DateCreated { get; set; }

    public bool IsRead { get; set; }
}
