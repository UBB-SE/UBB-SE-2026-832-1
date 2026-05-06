using ClassLibrary.Models;

namespace WebAPI.Services;

public sealed class AchievementUnlockedEventArgs : EventArgs
{
    public AchievementUnlockedEventArgs(AchievementShowcaseItem achievement)
    {
        this.Achievement = achievement;
    }

    public AchievementShowcaseItem Achievement { get; }
}
