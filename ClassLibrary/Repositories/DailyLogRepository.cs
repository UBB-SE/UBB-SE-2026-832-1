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

    public async Task AddAsync(DailyLog log)
    {
        if (log.User != null)
            log.User = await this.databaseContext.Users.FindAsync(log.User.UserId) ?? log.User;

        if (log.FoodItem != null)
            log.FoodItem = await this.databaseContext.FoodItems.FindAsync(log.FoodItem.FoodItemId) ?? log.FoodItem;

        if (log.Meal != null)
            log.Meal = await this.databaseContext.Meals.FindAsync(log.Meal.MealId) ?? log.Meal;

        await this.databaseContext.DailyLogs.AddAsync(log);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task<bool> HasAnyLogsAsync(int userId)
    {
        return await this.databaseContext.DailyLogs
            .AsNoTracking()
            .AnyAsync(dailyLog => dailyLog.User.UserId == userId);
    }

    public async Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive)
    {
        var logs = await this.databaseContext.DailyLogs
            .AsNoTracking()
            .Where(dailyLog => dailyLog.User.UserId == userId && dailyLog.LoggedAt >= startInclusive && dailyLog.LoggedAt < endExclusive)
            .ToListAsync();

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
