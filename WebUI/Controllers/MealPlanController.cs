using ClassLibrary.DTOs;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers;

public class MealPlanController : Controller
{
    private const int InvalidId = 0;
    private const string GoalSuffix = " Goal";

    private readonly IMealPlanProxy mealPlanProxy;
    private readonly IDailyLogProxy dailyLogProxy;
    private readonly IUserSession userSession;

    public MealPlanController(IMealPlanProxy mealPlanProxy, IDailyLogProxy dailyLogProxy, IUserSession userSession)
    {
        this.mealPlanProxy = mealPlanProxy;
        this.dailyLogProxy = dailyLogProxy;
        this.userSession = userSession;
    }

    // GET: /MealPlan
    public async Task<IActionResult> Index()
    {
        var model = new MealPlanPageViewModel();

        int userId = userSession.CurrentClientId;
        if (userId <= InvalidId)
        {
            model.StatusMessage = "Please log in to view your meal plan.";
            model.HasMeals = false;
            return View(model);
        }

        try
        {
            var todaysPlan = await mealPlanProxy.GetTodaysMealPlanAsync(userId);

            if (todaysPlan != null)
            {
                PopulateMealPlanModel(model, todaysPlan);
            }
            else
            {
                model.StatusMessage = "No meal plan for today. Generate one below.";
                model.HasMeals = false;
            }
        }
        catch (Exception ex)
        {
            model.ErrorMessage = ex.Message;
            model.HasMeals = false;
        }

        return View(model);
    }

    // POST: /MealPlan/Generate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate()
    {
        int userId = userSession.CurrentClientId;
        if (userId <= InvalidId)
        {
            TempData["Error"] = "You must be logged in to generate a meal plan.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var existingPlan = await mealPlanProxy.GetTodaysMealPlanAsync(userId);
            if (existingPlan != null)
            {
                TempData["Error"] = "You already have a meal plan for today. A new one will be generated tomorrow.";
                return RedirectToAction(nameof(Index));
            }

            await mealPlanProxy.GenerateMealPlanAsync(userId);
            TempData["Success"] = "New meal plan generated for today!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /MealPlan/SaveToLog
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveToLog(int mealPlanId)
    {
        int userId = userSession.CurrentClientId;
        if (userId <= InvalidId)
        {
            TempData["Error"] = "You must be logged in to save meals.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await mealPlanProxy.SaveMealsToDailyLogAsync(mealPlanId, userId);
            TempData["Success"] = "All meals saved to your daily log!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(DailyLog));
    }

    // GET: /MealPlan/DailyLog
    public async Task<IActionResult> DailyLog(string? searchTerm)
    {
        var model = new DailyLogPageViewModel { SearchTerm = searchTerm };

        int userId = userSession.CurrentClientId;
        if (userId <= InvalidId)
        {
            model.ErrorMessage = "Please log in to view your daily log.";
            return View(model);
        }

        try
        {
            model.TodayTotals = await dailyLogProxy.GetTodayTotalsAsync(userId);
            model.WeekTotals = await dailyLogProxy.GetCurrentWeekTotalsAsync(userId);
            model.NutritionTargets = await dailyLogProxy.GetNutritionTargetsAsync(userId);
            model.TodayBurnedCalories = await dailyLogProxy.GetTodayBurnedCaloriesAsync(userId);
            model.WeekBurnedCalories = await dailyLogProxy.GetWeekBurnedCaloriesAsync(userId);
            model.FoodItems = await dailyLogProxy.SearchFoodItemsAsync(searchTerm);
        }
        catch (Exception ex)
        {
            model.ErrorMessage = ex.Message;
        }

        return View(model);
    }

    // POST: /MealPlan/LogFood
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogFood(int foodItemId, int calories)
    {
        int userId = userSession.CurrentClientId;
        if (userId <= InvalidId)
        {
            TempData["Error"] = "You must be logged in to log food.";
            return RedirectToAction(nameof(DailyLog));
        }

        try
        {
            await dailyLogProxy.LogFoodItemAsync(userId, new LogMealRequestDto { MealId = foodItemId, Calories = calories });
            TempData["Success"] = "Food item logged successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(DailyLog));
    }

    private void PopulateMealPlanModel(MealPlanPageViewModel model, ClassLibrary.DTOs.MealPlanDto plan)
    {
        model.CurrentMealPlanId = plan.MealPlanId;

        string goalName = plan.GoalType.Length > 0
            ? char.ToUpper(plan.GoalType[0]) + plan.GoalType[1..]
            : "Maintenance";

        int index = 0;
        foreach (var item in plan.FoodItems)
        {
            model.Meals.Add(MealItemViewModel.FromFoodItemDto(item, index));
            index++;
        }

        model.TotalCalories = model.Meals.Sum(m => m.Calories);
        model.TotalProtein = model.Meals.Sum(m => m.Protein);
        model.TotalCarbohydrates = model.Meals.Sum(m => m.Carbohydrates);
        model.TotalFat = model.Meals.Sum(m => m.Fat);
        model.GoalDescription = goalName + GoalSuffix;
        model.StatusMessage = $"Your meal plan for today ({goalName} goal)";
        model.HasMeals = model.Meals.Count > 0;
    }
}
