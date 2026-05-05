namespace WebAPI.Services.AchievementBus.Interfaces;

using ClassLibrary.Models;

public interface IAchievementUnlockedBus
{
    event EventHandler<AchievementUnlockedEventArgs>? AchievementUnlocked;

    void NotifyUnlocked(AchievementShowcaseItem achievement);
}