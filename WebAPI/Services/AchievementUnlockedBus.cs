namespace WebAPI.Services.AchievementBus;

using WebAPI.Services.AchievementBus.Interfaces;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

public sealed class AchievementUnlockedBus : IAchievementUnlockedBus
{
    public event EventHandler<AchievementUnlockedEventArgs>? AchievementUnlocked;

    public void NotifyUnlocked(AchievementShowcaseItemDto achievement)
    {
        var achievementModel = new AchievementShowcaseItem
        {
            AchievementId = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description,
            Criteria = achievement.Description
        };

        AchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs(achievementModel));
    }
}