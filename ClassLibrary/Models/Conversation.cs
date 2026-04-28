namespace ClassLibrary.Models;

public class Conversation
{
    public int Id { get; set; }

    public bool HasUnanswered { get; set; }

    public virtual User User { get; set; } = default!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
