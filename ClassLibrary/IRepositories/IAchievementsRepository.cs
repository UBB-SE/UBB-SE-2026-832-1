using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IAchievementsRepository
{
    Task<int> GetConsecutiveWorkoutDayStreakAsync(int clientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Achievement>> GetAllAchievementsAsync(CancellationToken cancellationToken = default);

    Task<int> GetWorkoutsInLastSevenDaysAsync(int clientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AchievementShowcaseItem>> GetAchievementShowcaseForClientAsync(int clientId, CancellationToken cancellationToken = default);

    Task<int> GetWorkoutCountAsync(int clientId, CancellationToken cancellationToken = default);

    Task<int> GetDistinctWorkoutDayCountAsync(int clientId, CancellationToken cancellationToken = default);

    Task<AchievementShowcaseItem?> GetAchievementForClientAsync(int achievementId, int clientId, CancellationToken cancellationToken = default);

    Task<bool> AwardAchievementAsync(int clientId, int achievementId, CancellationToken cancellationToken = default);
}
