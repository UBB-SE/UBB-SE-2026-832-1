using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IDailyLogRepository
{
    Task AddAsync(DailyLog log, CancellationToken cancellationToken = default);
    Task<DailyLog?> GetNutritionTotalsForRangeAsync(Guid userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default);
    Task<bool> HasAnyLogsAsync(Guid userId, CancellationToken cancellationToken = default);
}
