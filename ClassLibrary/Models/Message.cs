namespace ClassLibrary.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime SentAt { get; set; }

    [Required]
    [ForeignKey(nameof(Conversation))]
    public int ConversationId { get; set; }

    [Required]
    [ForeignKey(nameof(Sender))]
    public Guid SenderId { get; set; }

    [Required]
    [MaxLength(5000)]
    public string TextContent { get; set; } = string.Empty;

    public virtual Conversation? Conversation { get; set; }

    public virtual User? Sender { get; set; }
}
