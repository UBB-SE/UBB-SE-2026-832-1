using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IDailyLogRepository
{
    Task AddAsync(DailyLog log, CancellationToken cancellationToken = default);
    Task<DailyLog?> GetNutritionTotalsForRangeAsync(User user, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default);
    Task<bool> HasAnyLogsAsync(User user, CancellationToken cancellationToken = default);
}
