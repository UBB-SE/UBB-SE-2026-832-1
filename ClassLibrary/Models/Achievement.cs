using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Achievement
{
    public int AchievementId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Icon { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Criteria { get; set; } = string.Empty;

    public int? ThresholdWorkouts { get; set; }

    public ICollection<Client> Clients { get; set; } = [];
}
