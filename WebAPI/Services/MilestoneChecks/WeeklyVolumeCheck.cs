using ClassLibrary.IRepositories;

namespace WebAPI.Services.MilestoneChecks;

internal sealed class WeeklyVolumeCheck : IMilestoneCheck
{
    private readonly string achievementTitle;
    private readonly int requiredWorkoutsPerWeek;

    public WeeklyVolumeCheck(string achievementTitle, int requiredWorkoutsPerWeek)
    {
        this.achievementTitle = achievementTitle;
        this.requiredWorkoutsPerWeek = requiredWorkoutsPerWeek;
    }

    public string AchievementTitle => this.achievementTitle;

    public async Task<bool> IsMetAsync(int clientId, IAchievementsRepository achievementsRepository)
    {
        int count = await achievementsRepository.GetWorkoutsInLastSevenDaysAsync(clientId);
        return count >= this.requiredWorkoutsPerWeek;
    }
}
