using ClassLibrary.IRepositories;

namespace WebAPI.Services.MilestoneChecks;

internal sealed class WorkoutCountCheck : IMilestoneCheck
{
    private readonly string achievementTitle;
    private readonly int requiredCount;

    public WorkoutCountCheck(string achievementTitle, int requiredCount)
    {
        this.achievementTitle = achievementTitle;
        this.requiredCount = requiredCount;
    }

    public string AchievementTitle => this.achievementTitle;

    public async Task<bool> IsMetAsync(int clientId, IAchievementsRepository achievementsRepository)
    {
        int count = await achievementsRepository.GetWorkoutCountAsync(clientId);
        return count >= this.requiredCount;
    }
}
