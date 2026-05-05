namespace WebAPI.Services.AchievementBus;

using WebAPI.Services.AchievementBus.Interfaces;
using ClassLibrary.Models;

public sealed class AchievementUnlockedBus : IAchievementUnlockedBus
{
    public event EventHandler<AchievementUnlockedEventArgs>? AchievementUnlocked;

    public void NotifyUnlocked(AchievementShowcaseItem achievement)
    {
        this.AchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs(achievement));
    }
}