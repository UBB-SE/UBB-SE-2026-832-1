using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IAchievementsRepository
{
    Task<int> GetConsecutiveWorkoutDayStreakAsync(int clientId);

    Task<IReadOnlyList<Achievement>> GetAllAchievementsAsync();

    Task<int> GetWorkoutsInLastSevenDaysAsync(int clientId);

    Task<IReadOnlyList<AchievementShowcaseItem>> GetAchievementShowcaseForClientAsync(int clientId);

    Task<int> GetWorkoutCountAsync(int clientId);

    Task<int> GetDistinctWorkoutDayCountAsync(int clientId);

    Task<AchievementShowcaseItem?> GetAchievementForClientAsync(int achievementId, int clientId);

    Task<bool> AwardAchievementAsync(int clientId, int achievementId);
}
