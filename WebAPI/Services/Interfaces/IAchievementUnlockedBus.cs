namespace WebAPI.Services.AchievementBus.Interfaces;

using ClassLibrary.DTOs;

public interface IAchievementUnlockedBus
{
    event EventHandler<AchievementUnlockedEventArgs>? AchievementUnlocked;

    void NotifyUnlocked(AchievementShowcaseItemDto achievement);
}