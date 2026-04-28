using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class DailyLogRepository(AppDbContext dbContext) : IDailyLogRepository
{
    public async Task AddAsync(DailyLog log, CancellationToken cancellationToken = default)
    {
        await dbContext.DailyLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasAnyLogsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .AnyAsync(dl => dl.UserId == userId, cancellationToken);
    }

    public async Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default)
    {
        var logs = await dbContext.DailyLogs
            .AsNoTracking()
            .Where(dl => dl.UserId == userId && dl.LoggedAt >= startInclusive && dl.LoggedAt < endExclusive)
            .ToListAsync(cancellationToken);

        if (logs.Count == 0)
        {
            return null;
        }

        return new DailyLog
        {
            UserId = userId,
            LoggedAt = startInclusive,
            Calories = logs.Sum(l => l.Calories),
            Protein = logs.Sum(l => l.Protein),
            Carbohydrates = logs.Sum(l => l.Carbohydrates),
            Fats = logs.Sum(l => l.Fats),
        };
    }
}
