namespace ClassLibrary.Models;

public sealed class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}

