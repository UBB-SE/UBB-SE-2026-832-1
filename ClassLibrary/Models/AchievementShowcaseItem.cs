using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class AchievementShowcaseItem
{
    public int AchievementId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Criteria { get; init; } = string.Empty;

    [Required]
    public bool IsUnlocked { get; init; }
}
