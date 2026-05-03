using ClassLibrary.IRepositories;

namespace WebAPI.Services.MilestoneChecks;

internal sealed class StreakCheck : IMilestoneCheck
{
    private readonly string achievementTitle;
    private readonly int requiredConsecutiveDays;

    public StreakCheck(string achievementTitle, int requiredConsecutiveDays)
    {
        this.achievementTitle = achievementTitle;
        this.requiredConsecutiveDays = requiredConsecutiveDays;
    }

    public string AchievementTitle => this.achievementTitle;

    public async Task<bool> IsMetAsync(int clientId, IAchievementsRepository achievementsRepository)
    {
        int streak = await achievementsRepository.GetConsecutiveWorkoutDayStreakAsync(clientId);
        return streak >= this.requiredConsecutiveDays;
    }
}
