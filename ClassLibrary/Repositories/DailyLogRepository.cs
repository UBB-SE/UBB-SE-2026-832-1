using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class DailyLogRepository : IDailyLogRepository
{
    private readonly AppDbContext databaseContext;

    public DailyLogRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task AddAsync(DailyLog log, CancellationToken cancellationToken = default)
    {
        await this.databaseContext.DailyLogs.AddAsync(log, cancellationToken);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasAnyLogsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.DailyLogs
            .AsNoTracking()
            .AnyAsync(dailyLog => dailyLog.User.UserId == userId, cancellationToken);
    }

    public async Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default)
    {
        var logs = await this.databaseContext.DailyLogs
            .AsNoTracking()
            .Where(dailyLog => dailyLog.User.UserId == userId && dailyLog.LoggedAt >= startInclusive && dailyLog.LoggedAt < endExclusive)
            .ToListAsync(cancellationToken);

        if (logs.Count == 0)
        {
            return null;
        }

        return new DailyLog
        {
            User = new User { UserId = userId },
            LoggedAt = startInclusive,
            Calories = logs.Sum(dailyLog => dailyLog.Calories),
            Protein = logs.Sum(dailyLog => dailyLog.Protein),
            Carbohydrates = logs.Sum(dailyLog => dailyLog.Carbohydrates),
            Fats = logs.Sum(dailyLog => dailyLog.Fats),
        };
    }
}
