using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class MealPlanRepository(AppDbContext dbContext) : IMealPlanRepository
{
    public async Task<MealPlan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MealPlan>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<MealPlan?> GetLatestMealPlanAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .Where(mp => mp.UserId == userId)
            .OrderByDescending(mp => mp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MealPlan?> GetTodaysMealPlanAsync(int userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await dbContext.MealPlans
            .AsNoTracking()
            .Where(mp => mp.UserId == userId && mp.CreatedAt >= today && mp.CreatedAt < tomorrow)
            .OrderByDescending(mp => mp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(MealPlan entity, CancellationToken cancellationToken = default)
    {
        dbContext.MealPlans.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MealPlan entity, CancellationToken cancellationToken = default)
    {
        dbContext.MealPlans.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.MealPlans.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.MealPlans.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GeneratePersonalizedDailyMealPlanAsync(int userId, CancellationToken cancellationToken = default)
    {
        var meals = await dbContext.Meals.AsNoTracking().ToListAsync(cancellationToken);
        if (meals.Count == 0)
        {
            throw new InvalidOperationException("No meals found in database.");
        }

        var userData = await dbContext.UserData
            .AsNoTracking()
            .FirstOrDefaultAsync(ud => ud.UserId == userId, cancellationToken);

        int calorieNeeds = userData?.CalorieNeeds > 0 ? userData.CalorieNeeds : 2000;
        int proteinNeeds = userData?.ProteinNeeds > 0 ? userData.ProteinNeeds : 150;
        int carbNeeds = userData?.CarbNeeds > 0 ? userData.CarbNeeds : 200;
        int fatNeeds = userData?.FatNeeds > 0 ? userData.FatNeeds : 65;
        string goal = userData?.Goal ?? "general";

        var mealPlan = new MealPlan
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            GoalType = goal
        };

        dbContext.MealPlans.Add(mealPlan);
        await dbContext.SaveChangesAsync(cancellationToken);

        var favouriteIds = await dbContext.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.MealId)
            .ToListAsync(cancellationToken);

        var pool = new List<(int id, int cal, int pro, int carb, int fat)>();

        foreach (var meal in meals)
        {
            var ingredients = await dbContext.MealsIngredients
                .AsNoTracking()
                .Where(mi => mi.MealId == meal.Id)
                .Join(dbContext.Ingredients.AsNoTracking(),
                    mi => mi.FoodId,
                    i => i.FoodId,
                    (mi, i) => new { mi.Quantity, i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g })
                .ToListAsync(cancellationToken);

            pool.Add((
                meal.Id,
                (int)ingredients.Sum(x => x.CaloriesPer100g * x.Quantity / 100.0),
                (int)ingredients.Sum(x => x.ProteinPer100g * x.Quantity / 100.0),
                (int)ingredients.Sum(x => x.CarbsPer100g * x.Quantity / 100.0),
                (int)ingredients.Sum(x => x.FatPer100g * x.Quantity / 100.0)));
        }

        if (pool.Count < 3)
        {
            throw new InvalidOperationException("Not enough meals in the database to generate a plan.");
        }

        int bi = 0, bj = 1, bk = 2;
        int bestScore = int.MaxValue;
        bool bestHasFavourite = false;

        for (int i = 0; i < pool.Count - 2; i++)
        {
            for (int j = i + 1; j < pool.Count - 1; j++)
            {
                for (int k = j + 1; k < pool.Count; k++)
                {
                    int score = Math.Abs(pool[i].cal + pool[j].cal + pool[k].cal - calorieNeeds);
                    bool hasFav = favouriteIds.Contains(pool[i].id)
                               || favouriteIds.Contains(pool[j].id)
                               || favouriteIds.Contains(pool[k].id);

                    bool better = score < bestScore
                               || (hasFav && !bestHasFavourite && score <= bestScore + 100);

                    if (better)
                    {
                        bestScore = score;
                        bestHasFavourite = hasFav;
                        bi = i;
                        bj = j;
                        bk = k;
                    }
                }
            }
        }

        var selected = new[] { pool[bi], pool[bj], pool[bk] };
        var mealTypes = new[] { "breakfast", "lunch", "dinner" };

        for (int i = 0; i < selected.Length; i++)
        {
            dbContext.MealPlanMeals.Add(new MealPlanMeal
            {
                MealPlanId = mealPlan.Id,
                MealId = selected[i].id,
                MealType = mealTypes[i],
                AssignedAt = DateTime.Now,
                IsConsumed = false
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return mealPlan.Id;
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForMealPlanAsync(int mealPlanId, CancellationToken cancellationToken = default)
    {
        var mealPlanMeals = await dbContext.MealPlanMeals
            .AsNoTracking()
            .Where(mpm => mpm.MealPlanId == mealPlanId)
            .OrderBy(mpm => mpm.MealType == "breakfast" ? 1 : mpm.MealType == "lunch" ? 2 : mpm.MealType == "dinner" ? 3 : 4)
            .ToListAsync(cancellationToken);

        var mealIds = mealPlanMeals.Select(mpm => mpm.MealId).ToList();
        var meals = await dbContext.Meals
            .AsNoTracking()
            .Where(m => mealIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        foreach (var meal in meals)
        {
            var ingredients = await dbContext.MealsIngredients
                .AsNoTracking()
                .Where(mi => mi.MealId == meal.Id)
                .Join(dbContext.Ingredients.AsNoTracking(),
                    mi => mi.FoodId,
                    i => i.FoodId,
                    (mi, i) => new { mi.Quantity, i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g })
                .ToListAsync(cancellationToken);

            meal.Calories = (int)ingredients.Sum(x => x.CaloriesPer100g * x.Quantity / 100.0);
            meal.Protein = (int)ingredients.Sum(x => x.ProteinPer100g * x.Quantity / 100.0);
            meal.Carbs = (int)ingredients.Sum(x => x.CarbsPer100g * x.Quantity / 100.0);
            meal.Fat = (int)ingredients.Sum(x => x.FatPer100g * x.Quantity / 100.0);
        }

        return meals;
    }

    public async Task<IReadOnlyList<IngredientViewModel>> GetIngredientsForMealAsync(int mealId, CancellationToken cancellationToken = default)
    {
        var ingredients = await dbContext.MealsIngredients
            .AsNoTracking()
            .Where(mi => mi.MealId == mealId)
            .Join(dbContext.Ingredients.AsNoTracking(),
                mi => mi.FoodId,
                i => i.FoodId,
                (mi, i) => new { mi.FoodId, i.Name, mi.Quantity, i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g })
            .OrderByDescending(x => x.Quantity)
            .ToListAsync(cancellationToken);

        return ingredients.Select(x => new IngredientViewModel
        {
            IngredientId = x.FoodId,
            Name = x.Name,
            Quantity = x.Quantity,
            Calories = Math.Round(x.CaloriesPer100g * x.Quantity / 100, 1),
            Protein = Math.Round(x.ProteinPer100g * x.Quantity / 100, 1),
            Carbs = Math.Round(x.CarbsPer100g * x.Quantity / 100, 1),
            Fat = Math.Round(x.FatPer100g * x.Quantity / 100, 1),
        }).ToList();
    }

    public async Task SaveMealsToDailyLogAsync(int userId, IReadOnlyList<Meal> meals, CancellationToken cancellationToken = default)
    {
        foreach (var meal in meals)
        {
            dbContext.DailyLogs.Add(new DailyLog
            {
                UserId = userId,
                MealId = meal.Id,
                Calories = meal.Calories,
                LoggedAt = DateTime.Now
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveMealToDailyLogAsync(int userId, int mealId, int calories, CancellationToken cancellationToken = default)
    {
        dbContext.DailyLogs.Add(new DailyLog
        {
            UserId = userId,
            MealId = mealId,
            Calories = calories,
            LoggedAt = DateTime.Now
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
