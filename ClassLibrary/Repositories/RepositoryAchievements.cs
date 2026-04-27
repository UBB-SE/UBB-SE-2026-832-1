using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;


public sealed class RepositoryAchievements(AppDbContext dbContext) : IRepositoryAchievements
{
    public async Task<IReadOnlyList<Achievement>> GetAllAchievementsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Achievements
            .AsNoTracking()
            .OrderBy(a => a.AchievementId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetWorkoutCountAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .CountAsync(w => w.ClientId == clientId, cancellationToken);
    }

    public async Task<int> GetDistinctWorkoutDayCountAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.WorkoutLogs
            .Where(w => w.ClientId == clientId)
            .Select(w => w.Date.Date)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetWorkoutsInLastSevenDaysAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var cutoff = today.AddDays(-6);
        var tomorrow = today.AddDays(1);

        return await dbContext.WorkoutLogs
            .CountAsync(
                w => w.ClientId == clientId && w.Date >= cutoff && w.Date < tomorrow,
                cancellationToken);
    }

    public async Task<int> GetConsecutiveWorkoutDayStreakAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var dates = await dbContext.WorkoutLogs
            .Where(w => w.ClientId == clientId)
            .Select(w => w.Date.Date)
            .Distinct()
            .OrderByDescending(d => d)
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
        var items = await dbContext.Achievements
            .AsNoTracking()
            .Select(a => new AchievementShowcaseItem
            {
                AchievementId = a.AchievementId,
                Title = a.Name,
                Description = a.Description,
                Criteria = a.Criteria,
                IsUnlocked = a.ClientAchievements.Any(ca => ca.ClientId == clientId && ca.Unlocked),
            })
            .ToListAsync(cancellationToken);

        
        return items
            .GroupBy(x => x.Title, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderByDescending(x => x.IsUnlocked)
            .ThenBy(x => x.AchievementId)
            .ToList();
    }

    public async Task<AchievementShowcaseItem?> GetAchievementForClientAsync(int achievementId, int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Achievements
            .AsNoTracking()
            .Where(a => a.AchievementId == achievementId)
            .Select(a => new AchievementShowcaseItem
            {
                AchievementId = a.AchievementId,
                Title = a.Name,
                Description = a.Description,
                Criteria = a.Criteria,
                IsUnlocked = a.ClientAchievements.Any(ca => ca.ClientId == clientId && ca.Unlocked),
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AwardAchievementAsync(int clientId, int achievementId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.ClientAchievements
            .FirstOrDefaultAsync(
                ca => ca.ClientId == clientId && ca.AchievementId == achievementId,
                cancellationToken);

        if (existing is { Unlocked: true })
        {
            return false;
        }

        if (existing is null)
        {
            dbContext.ClientAchievements.Add(new ClientAchievement
            {
                ClientId = clientId,
                AchievementId = achievementId,
                Unlocked = true,
            });
        }
        else
        {
            existing.Unlocked = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
