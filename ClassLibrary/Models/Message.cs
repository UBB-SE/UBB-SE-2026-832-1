using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Models;

public sealed class Message
{
    public int Id { get; set; }

    public DateTime SentAt { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string SenderUsername { get; set; } = string.Empty;

    public string SenderRole { get; set; } = string.Empty;

    public string TextContent { get; set; } = string.Empty;

    [NotMapped]
    public bool IsFromCurrentUser { get; set; }

    [NotMapped]
    public string SentAtFormatted => SentAt.ToString("g");
}
