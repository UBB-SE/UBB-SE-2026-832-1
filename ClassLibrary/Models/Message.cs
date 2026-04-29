using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Message
{
    public int Id { get; set; }

    public DateTime SentAt { get; set; }

    public virtual Conversation Conversation { get; set; } = default!;

    public virtual User Sender { get; set; } = default!;

    [MaxLength(5000)]
    public string TextContent { get; set; } = string.Empty;
}
