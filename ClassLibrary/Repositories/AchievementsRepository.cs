using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;


public sealed class AchievementsRepository : IAchievementsRepository
{
    private readonly AppDbContext databaseContext;

    public AchievementsRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<Achievement>> GetAllAchievementsAsync(CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Achievements
            .AsNoTracking()
            .OrderBy(achievement => achievement.AchievementId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetWorkoutCountAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.WorkoutLogs
            .CountAsync(workoutLog => workoutLog.Client.ClientId == clientId, cancellationToken);
    }

    public async Task<int> GetDistinctWorkoutDayCountAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.WorkoutLogs
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .Select(workoutLog => workoutLog.Date.Date)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetWorkoutsInLastSevenDaysAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var cutoff = today.AddDays(-6);
        var tomorrow = today.AddDays(1);

        return await this.databaseContext.WorkoutLogs
            .CountAsync(
                workoutLog => workoutLog.Client.ClientId == clientId && workoutLog.Date >= cutoff && workoutLog.Date < tomorrow,
                cancellationToken);
    }

    public async Task<int> GetConsecutiveWorkoutDayStreakAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var dates = await this.databaseContext.WorkoutLogs
            .Where(workoutLog => workoutLog.Client.ClientId == clientId)
            .Select(workoutLog => workoutLog.Date.Date)
            .Distinct()
            .OrderByDescending(date => date)
            .ToListAsync(cancellationToken);

        if (dates.Count == 0)
        {
            return 0;
        }

        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i - 1] - dates[i]).TotalDays == 1)
            {
                currentStreak++;
                if (currentStreak > maxStreak)
                {
                    maxStreak = currentStreak;
                }
            }
            else
            {
                currentStreak = 1;
            }
        }

        return maxStreak;
    }

    public async Task<IReadOnlyList<AchievementShowcaseItem>> GetAchievementShowcaseForClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var items = await this.databaseContext.Achievements
            .AsNoTracking()
            .Select(achievement => new AchievementShowcaseItem
            {
                AchievementId = achievement.AchievementId,
                Title = achievement.Name,
                Description = achievement.Description,
                Criteria = achievement.Criteria,
                IsUnlocked = achievement.Clients.Any(client => client.ClientId == clientId),
            })
            .ToListAsync(cancellationToken);

        return items
            .GroupBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderByDescending(item => item.IsUnlocked)
            .ThenBy(item => item.AchievementId)
            .ToList();
    }

    public async Task<AchievementShowcaseItem?> GetAchievementForClientAsync(int achievementId, int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Achievements
            .AsNoTracking()
            .Where(achievement => achievement.AchievementId == achievementId)
            .Select(achievement => new AchievementShowcaseItem
            {
                AchievementId = achievement.AchievementId,
                Title = achievement.Name,
                Description = achievement.Description,
                Criteria = achievement.Criteria,
                IsUnlocked = achievement.Clients.Any(client => client.ClientId == clientId),
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AwardAchievementAsync(int clientId, int achievementId, CancellationToken cancellationToken = default)
    {
        var client = await this.databaseContext.Clients
            .Include(client => client.UnlockedAchievements)
            .FirstOrDefaultAsync(client => client.ClientId == clientId, cancellationToken);

        if (client is null)
        {
            return false;
        }

        bool alreadyUnlocked = client.UnlockedAchievements.Any(achievement => achievement.AchievementId == achievementId);
        if (alreadyUnlocked)
        {
            return false;
        }

        var achievement = await this.databaseContext.Achievements
            .FirstOrDefaultAsync(achievement => achievement.AchievementId == achievementId, cancellationToken);

        if (achievement is null)
        {
            return false;
        }

        client.UnlockedAchievements.Add(achievement);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
