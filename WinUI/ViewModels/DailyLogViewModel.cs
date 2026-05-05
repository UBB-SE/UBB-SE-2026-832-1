using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class DailyLogViewModel : ObservableObject
{
    private const string ERROR_NO_FOOD_SELECTED = "Please select a food item before logging.";
    private const string ERROR_LOADING_FORMAT = "Failed to load daily summary: {0}";
    private const string ERROR_SEARCH_FORMAT = "Food search failed: {0}";
    private const string ERROR_LOG_FORMAT = "Failed to log food item: {0}";

    private readonly IDailyLogService dailyLogService;

    [ObservableProperty]
    private DailyLogTotalsDto todayTotals = new();

    [ObservableProperty]
    private DailyLogTotalsDto weekTotals = new();

    [ObservableProperty]
    private UserDataDto? nutritionTargets;

    [ObservableProperty]
    private double burnedCalories;

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

    public DailyLogViewModel(IDailyLogService dailyLogService)
    {
        this.dailyLogService = dailyLogService ?? throw new ArgumentNullException(nameof(dailyLogService));
    }

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
}
