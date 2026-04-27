namespace ClassLibrary.Models;

public sealed class Conversation
{
    public int Id { get; set; }

    public bool HasUnanswered { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;
}
