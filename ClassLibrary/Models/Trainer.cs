using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Trainer : User
{
    [Required]
    public ICollection<Client> Clients { get; set; } = [];
}
