namespace ClassLibrary.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool HasUnanswered { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
