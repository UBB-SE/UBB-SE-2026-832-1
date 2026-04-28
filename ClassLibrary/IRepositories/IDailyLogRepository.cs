using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IDailyLogRepository
{
    Task AddAsync(DailyLog log, CancellationToken cancellationToken = default);
    Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default);
    Task<bool> HasAnyLogsAsync(int userId, CancellationToken cancellationToken = default);
}
