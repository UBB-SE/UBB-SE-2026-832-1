using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Filters;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using FluentAssertions;
using NSubstitute;
using WebAPI.IServices;
using WebAPI.Services;
using Xunit;

namespace WebAPI.Tests.Services
{
    public class DailyLogServiceTests
    {
        private readonly IDailyLogRepository _mockDailyLogRepo;
        private readonly IUserRepository _mockUserRepo;
        private readonly IFoodItemService _mockFoodItemService;
        private readonly IFoodItemRepository _mockFoodItemRepository;
        private readonly IWorkoutLogRepository _mockWorkoutLogRepo;
        private readonly DailyLogService _service;

        public DailyLogServiceTests()
        {
            _mockDailyLogRepo = Substitute.For<IDailyLogRepository>();
            _mockUserRepo = Substitute.For<IUserRepository>();
            _mockFoodItemService = Substitute.For<IFoodItemService>();
            _mockFoodItemRepository = Substitute.For<IFoodItemRepository>();
            _mockWorkoutLogRepo = Substitute.For<IWorkoutLogRepository>();

            _service = new DailyLogService(
                _mockDailyLogRepo,
                _mockUserRepo,
                _mockFoodItemService,
                _mockFoodItemRepository,
                _mockWorkoutLogRepo);
        }

        [Fact]
        public async Task SearchFoodItemsAsync_WithSearchTerm_CallsFoodItemService()
        {
            var expectedItems = new List<FoodItemDto>
            {
                new FoodItemDto { FoodItemId = 1, Name = "Salata de pui", Calories = 500 }
            };
            _mockFoodItemService.GetFilteredAsync(Arg.Any<FoodItemFilter>())
                .Returns(expectedItems);

            var result = await _service.SearchFoodItemsAsync("pui");

            result.Should().HaveCount(1);
            await _mockFoodItemService.Received(1).GetFilteredAsync(
                Arg.Is<FoodItemFilter>(f => f.SearchTerm == "pui"));
        }

        [Fact]
        public async Task SearchFoodItemsAsync_WithNull_UsesEmptyString()
        {
            _mockFoodItemService.GetFilteredAsync(Arg.Any<FoodItemFilter>())
                .Returns(new List<FoodItemDto>());

            await _service.SearchFoodItemsAsync(null);

            await _mockFoodItemService.Received(1).GetFilteredAsync(
                Arg.Is<FoodItemFilter>(f => f.SearchTerm == string.Empty));
        }

        [Fact]
        public async Task LogFoodItemAsync_WithValidNewMeal_CallsRepoWithCorrectData()
        {
   
            int userId = 1;
            var request = new LogMealRequestDto { MealId = 7 };
            var foodItem = new FoodItem { FoodItemId = 7, Name = "Ciorba", Calories = 350 };
            var user = new User { UserId = userId };

            _mockDailyLogRepo.HasFoodItemLoggedTodayAsync(userId, request.MealId).Returns(false);
            _mockFoodItemRepository.GetByIdAsync(request.MealId).Returns(foodItem);
            _mockUserRepo.GetByIdAsync(userId).Returns(user);


            await _service.LogFoodItemAsync(userId, request);

            await _mockDailyLogRepo.Received(1).AddAsync(
                Arg.Is<DailyLog>(d => d.User.UserId == userId && d.FoodItem.FoodItemId == 7 && d.Calories == 350));
        }

        [Fact]
        public async Task LogFoodItemAsync_WhenAlreadyLoggedToday_ThrowsInvalidOperationException()
        {
            int userId = 1;
            var request = new LogMealRequestDto { MealId = 7 };
            _mockDailyLogRepo.HasFoodItemLoggedTodayAsync(userId, request.MealId).Returns(true);

            Func<Task> act = async () => await _service.LogFoodItemAsync(userId, request);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("This meal has already been saved to your daily log today.");
        }

        [Fact]
        public async Task GetTodayTotalsAsync_ReturnsCorrectTotalsFromRepo()
        {
            int userId = 5;
            var expectedLog = new DailyLog { Calories = 1200, Protein = 80, Carbohydrates = 150, Fats = 40 };
            _mockDailyLogRepo.GetNutritionTotalsForRangeAsync(userId, Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(expectedLog);

            var result = await _service.GetTodayTotalsAsync(userId);

            result.TotalCalories.Should().Be(1200);
            result.TotalProtein.Should().Be(80);
        }
    }
}