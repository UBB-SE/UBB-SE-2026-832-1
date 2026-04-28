using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class ClientAchievement
{
    [Required]
    public Client Client { get; set; } = null!;

    [Required]
    public Achievement Achievement { get; set; } = null!;

    [Required]
    public bool Unlocked { get; set; }
}
