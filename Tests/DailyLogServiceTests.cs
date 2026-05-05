using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebApi.IServices;
using WebAPI.IServices;
using WebAPI.Services;

namespace Tests;

public sealed class DailyLogServiceTests
{
    private readonly Mock<IDailyLogRepository> mockDailyLogRepository;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IFoodItemService> mockFoodItemService;
    private readonly Mock<IFoodItemRepository> mockFoodItemRepository;
    private readonly DailyLogService dailyLogService;

    public DailyLogServiceTests()
    {
        this.mockDailyLogRepository = new Mock<IDailyLogRepository>();
        this.mockUserRepository = new Mock<IUserRepository>();
        this.mockFoodItemService = new Mock<IFoodItemService>();
        this.mockFoodItemRepository = new Mock<IFoodItemRepository>();
        this.dailyLogService = new DailyLogService(
            this.mockDailyLogRepository.Object,
            this.mockUserRepository.Object,
            this.mockFoodItemService.Object,
            this.mockFoodItemRepository.Object);
    }

    [Fact]
    public async Task GetTodayTotalsAsync_WhenRepositoryReturnsLog_ShouldMapCorrectly()
    {
        var dailyLog = new DailyLog
        {
            Calories = 2100,
            Protein = 150,
            Carbohydrates = 250,
            Fats = 70,
        };

        this.mockDailyLogRepository
            .Setup(repository => repository.GetNutritionTotalsForRangeAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(dailyLog);

        var result = await this.dailyLogService.GetTodayTotalsAsync(1);

        Assert.Equal(2100, result.TotalCalories);
        Assert.Equal(150, result.TotalProtein);
        Assert.Equal(250, result.TotalCarbohydrates);
        Assert.Equal(70, result.TotalFat);
    }

    [Fact]
    public async Task GetTodayTotalsAsync_WhenRepositoryReturnsNull_ShouldReturnEmptyTotals()
    {
        this.mockDailyLogRepository
            .Setup(repository => repository.GetNutritionTotalsForRangeAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((DailyLog?)null);

        var result = await this.dailyLogService.GetTodayTotalsAsync(1);

        Assert.Equal(0, result.TotalCalories);
        Assert.Equal(0, result.TotalProtein);
        Assert.Equal(0, result.TotalCarbohydrates);
        Assert.Equal(0, result.TotalFat);
    }

    [Fact]
    public async Task GetCurrentWeekTotalsAsync_ShouldQuerySevenDayRange()
    {
        this.mockDailyLogRepository
            .Setup(repository => repository.GetNutritionTotalsForRangeAsync(
                1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((DailyLog?)null);

        await this.dailyLogService.GetCurrentWeekTotalsAsync(1);

        this.mockDailyLogRepository.Verify(
            repository => repository.GetNutritionTotalsForRangeAsync(
                1,
                It.Is<DateTime>(start => start.DayOfWeek == DayOfWeek.Monday),
                It.Is<DateTime>(end => (end - DateTime.Today).TotalDays <= 7)),
            Times.Once);
    }

    [Fact]
    public async Task GetTodayBurnedCaloriesAsync_ShouldReturnDefaultValue()
    {
        var result = await this.dailyLogService.GetTodayBurnedCaloriesAsync(1);

        Assert.Equal(500d, result);
    }

    [Fact]
    public async Task GetCurrentUserNutritionTargetsAsync_WhenUserDataExists_ShouldReturnMappedDto()
    {
        var userData = new UserData
        {
            UserDataId = 10,
            Weight = 75,
            Height = 180,
            Age = 25,
            Gender = "Male",
            Goal = "Maintain",
            CalorieNeeds = 2200,
            ProteinNeeds = 160,
            CarbohydrateNeeds = 260,
            FatNeeds = 70,
            User = new User { UserId = 1 },
        };

        this.mockUserRepository
            .Setup(repository => repository.GetUserDataByUserIdAsync(1))
            .ReturnsAsync(userData);

        var result = await this.dailyLogService.GetCurrentUserNutritionTargetsAsync(1);

        Assert.NotNull(result);
        Assert.Equal(75, result.Weight);
        Assert.Equal(2200, result.CalorieNeeds);
    }

    [Fact]
    public async Task GetCurrentUserNutritionTargetsAsync_WhenUserDataDoesNotExist_ShouldReturnNull()
    {
        this.mockUserRepository
            .Setup(repository => repository.GetUserDataByUserIdAsync(999))
            .ReturnsAsync((UserData?)null);

        var result = await this.dailyLogService.GetCurrentUserNutritionTargetsAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task LogFoodItemAsync_WhenFoodItemNotFound_ShouldThrowInvalidOperationException()
    {
        this.mockFoodItemRepository
            .Setup(repository => repository.GetByIdAsync(42))
            .ReturnsAsync((FoodItem?)null);

        var request = new LogMealRequestDto { MealId = 42, Calories = 300 };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => this.dailyLogService.LogFoodItemAsync(1, request));
    }

    [Fact]
    public async Task LogFoodItemAsync_WhenUserNotFound_ShouldThrowInvalidOperationException()
    {
        this.mockFoodItemRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new FoodItem { FoodItemId = 1, Calories = 200 });

        this.mockUserRepository
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var request = new LogMealRequestDto { MealId = 1, Calories = 200 };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => this.dailyLogService.LogFoodItemAsync(999, request));
    }

    [Fact]
    public async Task LogFoodItemAsync_WhenValid_ShouldAddDailyLogToRepository()
    {
        var foodItem = new FoodItem
        {
            FoodItemId = 1,
            Calories = 350,
            Protein = 25,
            Carbohydrates = 40,
            Fat = 10,
        };

        var user = new User { UserId = 1, Username = "testuser" };

        this.mockFoodItemRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(foodItem);

        this.mockUserRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(user);

        var request = new LogMealRequestDto { MealId = 1, Calories = 350 };

        await this.dailyLogService.LogFoodItemAsync(1, request);

        this.mockDailyLogRepository.Verify(
            repository => repository.AddAsync(It.Is<DailyLog>(
                dailyLog => dailyLog.Calories == 350
                    && dailyLog.Protein == 25
                    && dailyLog.Carbohydrates == 40
                    && dailyLog.Fats == 10)),
            Times.Once);
    }
}
