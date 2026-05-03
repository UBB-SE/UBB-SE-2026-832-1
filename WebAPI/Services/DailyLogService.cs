using ClassLibrary.DTOs;
using ClassLibrary.Filters;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebApi.IServices;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class DailyLogService : IDailyLogService
{
    private const int DAYS_IN_WEEK = 7;
    private const int ONE_DAY = 1;
    private const DayOfWeek START_OF_WEEK = DayOfWeek.Monday;
    private const double DEFAULT_BURNED_CALORIES = 500d;

    private readonly IDailyLogRepository dailyLogRepository;
    private readonly IUserRepository userRepository;
    private readonly IFoodItemRepository foodItemRepository;
    private readonly IFoodItemService foodItemService;

    public DailyLogService(
        IDailyLogRepository dailyLogRepository,
        IUserRepository userRepository,
        IFoodItemService foodItemService,
        IFoodItemRepository foodItemRepository)
    {
        this.dailyLogRepository = dailyLogRepository;
        this.userRepository = userRepository;
        this.foodItemService = foodItemService;
        this.foodItemRepository = foodItemRepository;
    }

    private static DailyLogTotalsDto MapToDailyLogTotalsDto(DailyLog? log)
    {
        if (log is null)
        {
            return new DailyLogTotalsDto();
        }

        return new DailyLogTotalsDto
        {
            TotalCalories = log.Calories,
            TotalProtein = log.Protein,
            TotalCarbohydrates = log.Carbohydrates,
            TotalFat = log.Fats
        };
    }

    private static UserDataDto MapToUserDataDto(UserData userData)
    {
        return new UserDataDto
        {
            UserDataId = userData.UserDataId,
            UserId = userData.User.UserId,
            Weight = userData.Weight,
            Height = userData.Height,
            Age = userData.Age,
            Gender = userData.Gender,
            Goal = userData.Goal,
            BodyMassIndex = userData.BodyMassIndex,
            CalorieNeeds = userData.CalorieNeeds,
            ProteinNeeds = userData.ProteinNeeds,
            CarbohydrateNeeds = userData.CarbohydrateNeeds,
            FatNeeds = userData.FatNeeds
        };
    }

    public async Task<bool> HasAnyLogsAsync(int userId)
    {
        return await this.dailyLogRepository.HasAnyLogsAsync(userId);
    }

    public async Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId)
    {
        var start = DateTime.Today;
        var end = start.AddDays(ONE_DAY);

        var log = await this.dailyLogRepository.GetNutritionTotalsForRangeAsync(userId, start, end);

        return MapToDailyLogTotalsDto(log);
    }

    public async Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId)
    {
        var today = DateTime.Today;
        int diff = (DAYS_IN_WEEK + (today.DayOfWeek - START_OF_WEEK)) % DAYS_IN_WEEK;

        var startOfWeek = today.AddDays(-diff);
        var endOfWeek = startOfWeek.AddDays(DAYS_IN_WEEK);

        var log = await this.dailyLogRepository.GetNutritionTotalsForRangeAsync(userId, startOfWeek, endOfWeek);

        return MapToDailyLogTotalsDto(log);
    }

    public async Task<UserDataDto?> GetCurrentUserNutritionTargetsAsync(int userId)
    {
        var userData = await this.userRepository.GetUserDataByUserIdAsync(userId);

        if (userData is null)
        {
            return null;
        }

        return MapToUserDataDto(userData);
    }

    public Task<double> GetTodayBurnedCaloriesAsync(int userId)
    {
        return Task.FromResult(DEFAULT_BURNED_CALORIES);
    }

    public async Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm)
    {
        var filter = new FoodItemFilter
        {
            SearchTerm = searchTerm ?? string.Empty
        };

        return await this.foodItemService.GetFilteredAsync(filter);
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForAutocompleteAsync()
    {
        return await this.foodItemService.GetFilteredAsync(new FoodItemFilter());
    }

    public async Task LogFoodItemAsync(int userId, LogMealRequestDto request)
    {
        var foodItem = await this.foodItemRepository.GetByIdAsync(request.MealId);

        if (foodItem is null)
        {
            throw new InvalidOperationException($"Food item with ID {request.MealId} not found.");
        }

        var user = await this.userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        var dailyLog = new DailyLog
        {
            User = user,
            FoodItem = foodItem,
            Calories = foodItem.Calories,
            Protein = foodItem.Protein,
            Carbohydrates = foodItem.Carbohydrates,
            Fats = foodItem.Fat,
            LoggedAt = DateTime.UtcNow
        };

        await this.dailyLogRepository.AddAsync(dailyLog);
    }
}