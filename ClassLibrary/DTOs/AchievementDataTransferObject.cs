namespace ClassLibrary.DTOs;

public sealed class AchievementDataTransferObject
{
    public int AchievementId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Criteria { get; set; } = string.Empty;

    public bool IsUnlocked { get; set; }

    public string Icon { get; set; } = string.Empty;
}
