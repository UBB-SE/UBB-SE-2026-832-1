using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public partial class DailyLogViewModel : ObservableObject
{
    private const string ERROR_NO_FOOD_SELECTED = "Please select a food item before logging.";
    private const string ERROR_LOADING_FORMAT = "Failed to load daily summary: {0}";
    private const string ERROR_SEARCH_FORMAT = "Food search failed: {0}";
    private const string ERROR_LOG_FORMAT = "Failed to log food item: {0}";

    private readonly IDailyLogProxy dailyLogService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DailyCaloriesDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyProteinDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyCarbohydratesDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyFatDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyCaloriesPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyProteinPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyCarbohydratesPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyFatPercentage))]
    private DailyLogTotalsDto todayTotals = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeeklyCaloriesDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyProteinDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyCarbohydratesDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyFatDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyCaloriesPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyProteinPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyCarbohydratesPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyFatPercentage))]
    private DailyLogTotalsDto weekTotals = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DailyCaloriesDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyProteinDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyCarbohydratesDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyFatDisplay))]
    [NotifyPropertyChangedFor(nameof(DailyCaloriesPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyProteinPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyCarbohydratesPercentage))]
    [NotifyPropertyChangedFor(nameof(DailyFatPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyCaloriesDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyProteinDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyCarbohydratesDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyFatDisplay))]
    [NotifyPropertyChangedFor(nameof(WeeklyCaloriesPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyProteinPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyCarbohydratesPercentage))]
    [NotifyPropertyChangedFor(nameof(WeeklyFatPercentage))]
    private UserDataDto? nutritionTargets;

    [ObservableProperty]
    private double burnedCalories;

    [ObservableProperty]
    private double weekBurnedCalories;

    [ObservableProperty]
    private ObservableCollection<FoodItemDto> foodSearchResults = new();

    [ObservableProperty]
    private FoodItemDto? selectedFoodItem;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool hasMealLogged;

    public DailyLogViewModel(IDailyLogProxy dailyLogService)
    {
        this.dailyLogService = dailyLogService ?? throw new ArgumentNullException(nameof(dailyLogService));
    }

    public string DailyCaloriesDisplay => FormatCalorieDisplay(TodayTotals?.TotalCalories ?? 0, NutritionTargets?.CalorieNeeds ?? 0);

    public string DailyProteinDisplay => FormatMacroDisplay(TodayTotals?.TotalProtein ?? 0, NutritionTargets?.ProteinNeeds ?? 0, "g");

    public string DailyCarbohydratesDisplay => FormatMacroDisplay(TodayTotals?.TotalCarbohydrates ?? 0, NutritionTargets?.CarbohydrateNeeds ?? 0, "g");

    public string DailyFatDisplay => FormatMacroDisplay(TodayTotals?.TotalFat ?? 0, NutritionTargets?.FatNeeds ?? 0, "g");

    public double DailyCaloriesPercentage => CalculatePercentage(TodayTotals?.TotalCalories ?? 0, NutritionTargets?.CalorieNeeds ?? 0);

    public double DailyProteinPercentage => CalculatePercentage(TodayTotals?.TotalProtein ?? 0, NutritionTargets?.ProteinNeeds ?? 0);

    public double DailyCarbohydratesPercentage => CalculatePercentage(TodayTotals?.TotalCarbohydrates ?? 0, NutritionTargets?.CarbohydrateNeeds ?? 0);

    public double DailyFatPercentage => CalculatePercentage(TodayTotals?.TotalFat ?? 0, NutritionTargets?.FatNeeds ?? 0);

    public string WeeklyCaloriesDisplay => FormatCalorieDisplay(WeekTotals?.TotalCalories ?? 0, (NutritionTargets?.CalorieNeeds ?? 0) * 7);

    public string WeeklyProteinDisplay => FormatMacroDisplay(WeekTotals?.TotalProtein ?? 0, (NutritionTargets?.ProteinNeeds ?? 0) * 7, "g");

    public string WeeklyCarbohydratesDisplay => FormatMacroDisplay(WeekTotals?.TotalCarbohydrates ?? 0, (NutritionTargets?.CarbohydrateNeeds ?? 0) * 7, "g");

    public string WeeklyFatDisplay => FormatMacroDisplay(WeekTotals?.TotalFat ?? 0, (NutritionTargets?.FatNeeds ?? 0) * 7, "g");

    public double WeeklyCaloriesPercentage => CalculatePercentage(WeekTotals?.TotalCalories ?? 0, (NutritionTargets?.CalorieNeeds ?? 0) * 7);

    public double WeeklyProteinPercentage => CalculatePercentage(WeekTotals?.TotalProtein ?? 0, (NutritionTargets?.ProteinNeeds ?? 0) * 7);

    public double WeeklyCarbohydratesPercentage => CalculatePercentage(WeekTotals?.TotalCarbohydrates ?? 0, (NutritionTargets?.CarbohydrateNeeds ?? 0) * 7);

    public double WeeklyFatPercentage => CalculatePercentage(WeekTotals?.TotalFat ?? 0, (NutritionTargets?.FatNeeds ?? 0) * 7);

    public async Task LoadDailySummaryAsync(int userId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            TodayTotals = await this.dailyLogService.GetTodayTotalsAsync(userId);
            WeekTotals = await this.dailyLogService.GetCurrentWeekTotalsAsync(userId);
            NutritionTargets = await this.dailyLogService.GetNutritionTargetsAsync(userId);
            BurnedCalories = await this.dailyLogService.GetTodayBurnedCaloriesAsync(userId);
            WeekBurnedCalories = await this.dailyLogService.GetWeekBurnedCaloriesAsync(userId);
            HasMealLogged = TodayTotals.TotalCalories > 0;
        }
        catch (Exception exception)
        {
            ErrorMessage = string.Format(ERROR_LOADING_FORMAT, exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchFoodItemsAsync(string searchTerm)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var results = await this.dailyLogService.SearchFoodItemsAsync(searchTerm);

            this.FoodSearchResults.Clear();
            foreach (var foodItem in results)
            {
                this.FoodSearchResults.Add(foodItem);
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = string.Format(ERROR_SEARCH_FORMAT, exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LogSelectedFoodItemAsync(int userId)
    {
        if (this.SelectedFoodItem == null)
        {
            ErrorMessage = ERROR_NO_FOOD_SELECTED;
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var request = new LogMealRequestDto
            {
                MealId = this.SelectedFoodItem.FoodItemId,
                Calories = this.SelectedFoodItem.Calories,
            };

            await this.dailyLogService.LogFoodItemAsync(userId, request);
            StatusMessage = $"Logged {this.SelectedFoodItem.Name} successfully.";
            HasMealLogged = true;
            await LoadDailySummaryAsync(userId);
        }
        catch (Exception exception)
        {
            ErrorMessage = string.Format(ERROR_LOG_FORMAT, exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string FormatCalorieDisplay(double consumed, int goal)
    {
        int consumedInt = (int)Math.Round(consumed);
        int percentage = (int)CalculatePercentage(consumed, goal);
        return $"{consumedInt} / {goal} kcal ({percentage}%)";
    }

    private string FormatMacroDisplay(double consumed, int goal, string unit)
    {
        int consumedInt = (int)Math.Round(consumed);
        int percentage = (int)CalculatePercentage(consumed, goal);
        return $"{consumedInt} / {goal} {unit} ({percentage}%)";
    }

    private double CalculatePercentage(double consumed, int goal)
    {
        if (goal <= 0)
        {
            return 0.0;
        }

        return Math.Min((consumed / goal) * 100, 100);
    }
}

