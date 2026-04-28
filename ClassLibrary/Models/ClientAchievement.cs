using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Models;

[PrimaryKey(nameof(ClientId), nameof(AchievementId))]
public class ClientAchievement
{
    [Required]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Required]
    public int AchievementId { get; set; }

    [Required]
    public bool Unlocked { get; set; }

    public Achievement Achievement { get; set; } = null!;
}
