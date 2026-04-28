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
    public virtual Conversation Conversation { get; set; } = default!;

    [Required]
    public virtual User Sender { get; set; } = default!;

    [Required]
    [MaxLength(5000)]
    public string TextContent { get; set; } = string.Empty;
}
