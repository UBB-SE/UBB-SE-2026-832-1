using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class DailyLogRepository(AppDbContext dbContext) : IDailyLogRepository
{
    public async Task AddAsync(DailyLog log, CancellationToken cancellationToken = default)
    {
        dbContext.DailyLogs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasAnyLogsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AnyAsync(dl => dl.UserId == userId, cancellationToken);
    }

    public async Task<DailyLog> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive, CancellationToken cancellationToken = default)
    {
        var logs = await dbContext.DailyLogs
            .AsNoTracking()
            .Where(dl => dl.UserId == userId
                      && dl.LoggedAt >= startInclusive
                      && dl.LoggedAt < endExclusive)
            .ToListAsync(cancellationToken);

        var mealIds = logs.Select(l => l.MealId).Distinct().ToList();

        var ingredientTotals = await dbContext.MealsIngredients
            .AsNoTracking()
            .Where(mi => mealIds.Contains(mi.MealId))
            .Join(dbContext.Ingredients.AsNoTracking(),
                mi => mi.FoodId,
                i => i.FoodId,
                (mi, i) => new { mi.Quantity, i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g })
            .ToListAsync(cancellationToken);

        return new DailyLog
        {
            UserId = userId,
            LoggedAt = startInclusive,
            Calories = ingredientTotals.Sum(x => x.CaloriesPer100g * x.Quantity / 100.0),
            Protein = ingredientTotals.Sum(x => x.ProteinPer100g * x.Quantity / 100.0),
            Carbs = ingredientTotals.Sum(x => x.CarbsPer100g * x.Quantity / 100.0),
            Fats = ingredientTotals.Sum(x => x.FatPer100g * x.Quantity / 100.0),
        };
    }
}
