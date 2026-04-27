namespace ClassLibrary.Models;

public class ClientAchievement
{
    public int ClientId { get; set; }

    public int AchievementId { get; set; }

    public bool Unlocked { get; set; }

    public Achievement Achievement { get; set; } = null!;
}
