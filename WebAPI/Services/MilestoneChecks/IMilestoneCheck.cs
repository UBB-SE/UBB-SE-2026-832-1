using ClassLibrary.IRepositories;

namespace WebAPI.Services.MilestoneChecks;

internal interface IMilestoneCheck
{
    string AchievementTitle { get; }

    Task<bool> IsMetAsync(int clientId, IAchievementsRepository achievementsRepository);
}
