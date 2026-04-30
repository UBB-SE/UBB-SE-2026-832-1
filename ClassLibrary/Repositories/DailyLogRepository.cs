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
            .AnyAsync(dl => dl.User.UserId == userId, cancellationToken);
    }

    public async Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default)
    {
        var logs = await this.databaseContext.DailyLogs
            .AsNoTracking()
            .Where(dl => dl.User.UserId == userId && dl.LoggedAt >= startInclusive && dl.LoggedAt < endExclusive)
            .ToListAsync(cancellationToken);

        if (logs.Count == 0)
        {
            return null;
        }

        return new DailyLog
        {
            User = new User { UserId = userId },
            LoggedAt = startInclusive,
            Calories = logs.Sum(l => l.Calories),
            Protein = logs.Sum(l => l.Protein),
            Carbohydrates = logs.Sum(l => l.Carbohydrates),
            Fats = logs.Sum(l => l.Fats),
        };
    }
}
