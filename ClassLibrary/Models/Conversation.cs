namespace ClassLibrary.Models;

using System.ComponentModel.DataAnnotations;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool HasUnanswered { get; set; }

    [Required]
    public virtual User User { get; set; } = default!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
