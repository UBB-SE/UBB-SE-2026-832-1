namespace ClassLibrary.Models;

public class Trainer : User
{
    public ICollection<Client> Clients { get; set; } = [];
}
