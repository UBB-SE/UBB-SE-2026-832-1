using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class MealPlanViewModel : ObservableObject
{
    private readonly IMealPlanService mealPlanService;
    private readonly UserSession userSession;

    private const int InvalidId = 0;
    private const string StatusLoadingMealPlan = "Loading your meal plan...";
    private const string StatusLoadingTodayMealPlan = "Loading your meal plan for today...";
    private const string StatusGeneratingMealPlan = "Generating your personalized meal plan for today...";
    private const string StatusMealPlanExists = "Meal plan already generated for today. New plan tomorrow!";
    private const string StatusLoginRequired = "Please log in to view your meal plan.";
    private const string StatusMealPlanGenerated = "New meal plan generated for today!";
    private const string StatusRegeneratingTest = "Regenerating meal plan (test)...";
    private const string ErrorUserNotLoggedTitle = "User Not Logged In";
    private const string ErrorUserNotLoggedMessage = "You need to be logged in to view your meal plan.\n\nPlease create an account or log in to continue.";
    private const string ErrorMealPlanExistsTitle = "Meal Plan Already Exists";
    private const string ErrorMealPlanExistsMessage = "You already have a meal plan for today.\n\nYour meal plan will automatically regenerate tomorrow based on your latest preferences.\n\nIf you changed your settings, the new preferences will apply to tomorrow's meal plan.";
    private const string ErrorNoMealsFound = "No meals found in your plan. Please try regenerating tomorrow.";
    private const string ErrorGeneratingMealPlanTitle = "Error Generating Meal Plan";
    private const string ErrorNoMealPlanTitle = "No Meal Plan";
    private const string ErrorNoMealPlanMessage = "No meal plan is currently loaded. Please generate a meal plan first.";
    private const string GoalSuffix = " Goal";
    private const string StatusMealPlanTitleFormat = "Your meal plan for today ({0} goal)";
    private const string NutritionSummaryFormat = "Daily Total: {0} kcal | {1}g protein | {2}g carbohydrates | {3}g fat";
    private const string MealSavedSuccessFormat = "All {0} meals saved to daily log!";

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private ObservableCollection<MealViewModel> generatedMeals = new();

    private int currentMealPlanId;

    public int CurrentMealPlanId
    {
        get => currentMealPlanId;
        set => SetProperty(ref currentMealPlanId, value);
    }

    private int totalCalories;

    public int TotalCalories
    {
        get => totalCalories;
        set => SetProperty(ref totalCalories, value);
    }

    private int totalProtein;

    public int TotalProtein
    {
        get => totalProtein;
        set => SetProperty(ref totalProtein, value);
    }

    private int totalCarbohydrates;

    public int TotalCarbohydrates
    {
        get => totalCarbohydrates;
        set => SetProperty(ref totalCarbohydrates, value);
    }

    private int totalFat;

    public int TotalFat
    {
        get => totalFat;
        set => SetProperty(ref totalFat, value);
    }

    private bool hasMeals;

    public bool HasMeals
    {
        get => hasMeals;
        set => SetProperty(ref hasMeals, value);
    }

    [ObservableProperty]
    private string totalNutritionSummary = string.Empty;

    [ObservableProperty]
    private string goalDescription = string.Empty;

    [ObservableProperty]
    private bool showErrorDialog;

    [ObservableProperty]
    private string errorDialogTitle = string.Empty;

    [ObservableProperty]
    private string errorDialogMessage = string.Empty;

    public MealPlanViewModel(IMealPlanService mealPlanService, UserSession userSession)
    {
        GeneratedMeals = new ObservableCollection<MealViewModel>();
        this.mealPlanService = mealPlanService;
        this.userSession = userSession;
    }

    [RelayCommand]
    private async Task OnGenerateMealPlan()
    {
        int userId = userSession.CurrentClientId;

        if (userId <= InvalidId)
        {
            ErrorDialogTitle = ErrorUserNotLoggedTitle;
            ErrorDialogMessage = ErrorUserNotLoggedMessage;
            ShowErrorDialog = true;
            StatusMessage = StatusLoginRequired;
            return;
        }

        var todaysPlan = await mealPlanService.GetTodaysMealPlanAsync(userId);

        if (todaysPlan != null)
        {
            ErrorDialogTitle = ErrorMealPlanExistsTitle;
            ErrorDialogMessage = ErrorMealPlanExistsMessage;
            ShowErrorDialog = true;
            StatusMessage = StatusMealPlanExists;
        }
        else
        {
            await LoadOrGenerateTodaysMealPlanAsync();
        }
    }

    public async Task ForceRegenerateMealPlanAsync()
    {
        IsBusy = true;
        StatusMessage = StatusRegeneratingTest;
        GeneratedMeals.Clear();
        TotalNutritionSummary = string.Empty;
        GoalDescription = string.Empty;

        try
        {
            int userId = userSession.CurrentClientId;

            if (userId <= InvalidId)
            {
                StatusMessage = StatusLoginRequired;
                HasMeals = false;
                return;
            }

            await GenerateNewMealPlanAsync(userId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadOrGenerateTodaysMealPlanAsync()
    {
        IsBusy = true;
        StatusMessage = StatusLoadingMealPlan;
        GeneratedMeals.Clear();
        TotalNutritionSummary = string.Empty;
        GoalDescription = string.Empty;

        try
        {
            int userId = userSession.CurrentClientId;

            if (userId <= InvalidId)
            {
                StatusMessage = StatusLoginRequired;
                HasMeals = false;
                return;
            }

            var todaysPlan = await mealPlanService.GetTodaysMealPlanAsync(userId);

            if (todaysPlan != null)
            {
                StatusMessage = StatusLoadingTodayMealPlan;
                await LoadMealPlanByIdAsync(todaysPlan.MealPlanId, userId);
            }
            else
            {
                StatusMessage = StatusGeneratingMealPlan;
                await GenerateNewMealPlanAsync(userId);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMealPlanByIdAsync(int mealPlanId, int userId)
    {
        CurrentMealPlanId = mealPlanId;

        var plan = await mealPlanService.GetByIdAsync(mealPlanId);
        if (plan == null || plan.FoodItems.Count == 0)
        {
            StatusMessage = ErrorNoMealsFound;
            HasMeals = false;
            return;
        }

        string userGoal = plan.GoalType;
        string goalName = userGoal.Length > 0
            ? char.ToUpper(userGoal[0]) + userGoal[1..]
            : "Maintenance";

        int index = 0;
        foreach (var item in plan.FoodItems)
        {
            GeneratedMeals.Add(MealViewModel.FromFoodItemDto(item, index));
            index++;
        }

        CalculateTotals();

        TotalNutritionSummary = string.Format(
            NutritionSummaryFormat,
            TotalCalories,
            TotalProtein,
            TotalCarbohydrates,
            TotalFat);

        GoalDescription = goalName + GoalSuffix;
        StatusMessage = string.Format(StatusMealPlanTitleFormat, goalName);
        HasMeals = true;
    }

    private async Task GenerateNewMealPlanAsync(int userId)
    {
        try
        {
            int mealPlanId = await mealPlanService.GenerateMealPlanAsync(userId);
            await LoadMealPlanByIdAsync(mealPlanId, userId);
            StatusMessage = StatusMealPlanGenerated;
        }
        catch (Exception ex)
        {
            ErrorDialogTitle = ErrorGeneratingMealPlanTitle;
            ErrorDialogMessage = ex.Message;
            ShowErrorDialog = true;
            HasMeals = false;
        }
    }

    private void CalculateTotals()
    {
        TotalCalories = GeneratedMeals.Sum(m => m.Calories);
        TotalProtein = GeneratedMeals.Sum(m => m.Protein);
        TotalCarbohydrates = GeneratedMeals.Sum(m => m.Carbohydrates);
        TotalFat = GeneratedMeals.Sum(m => m.Fat);
    }

    [RelayCommand]
    private async Task SaveToDailyLog()
    {
        if (CurrentMealPlanId <= InvalidId)
        {
            ErrorDialogTitle = ErrorNoMealPlanTitle;
            ErrorDialogMessage = ErrorNoMealPlanMessage;
            ShowErrorDialog = true;
            return;
        }

        await mealPlanService.SaveMealsToDailyLogAsync(CurrentMealPlanId, userSession.CurrentClientId);
        StatusMessage = string.Format(MealSavedSuccessFormat, GeneratedMeals.Count);
    }

    internal async Task SaveToDailyLogAsync()
    {
        await SaveToDailyLog();
    }
}
