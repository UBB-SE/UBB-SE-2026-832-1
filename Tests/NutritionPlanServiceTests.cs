using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class NutritionPlanServiceTests
{
    private readonly Mock<INutritionRepository> nutritionRepository = new();

    private NutritionPlanService CreateService() => new(this.nutritionRepository.Object);

    [Fact]
    public async Task CreatePlan_SetsStartDateToToday()
    {
        this.nutritionRepository
            .Setup(r => r.InsertNutritionPlanAsync(It.IsAny<NutritionPlan>()))
            .ReturnsAsync(1);

        var service = this.CreateService();
        var result = await service.CreatePlanAsync(clientId: 42);

        Assert.Equal(DateTime.Today, result.StartDate);
    }

    [Fact]
    public async Task CreatePlan_SetsEndDateToThirtyDaysFromToday()
    {
        this.nutritionRepository
            .Setup(r => r.InsertNutritionPlanAsync(It.IsAny<NutritionPlan>()))
            .ReturnsAsync(1);

        var service = this.CreateService();
        var result = await service.CreatePlanAsync(clientId: 42);

        Assert.Equal(DateTime.Today.AddDays(30), result.EndDate);
    }

    [Fact]
    public async Task CreatePlan_PersistsThroughRepository()
    {
        NutritionPlan? capturedPlan = null;
        this.nutritionRepository
            .Setup(r => r.InsertNutritionPlanAsync(It.IsAny<NutritionPlan>()))
            .Callback<NutritionPlan>(plan => capturedPlan = plan)
            .ReturnsAsync(7);

        var service = this.CreateService();
        await service.CreatePlanAsync(clientId: 42);

        Assert.NotNull(capturedPlan);
        Assert.Equal(DateTime.Today, capturedPlan.StartDate);
        Assert.Equal(DateTime.Today.AddDays(30), capturedPlan.EndDate);
        this.nutritionRepository.Verify(r => r.AssignNutritionPlanToClientAsync(42, 7), Times.Once);
    }

    [Fact]
    public async Task GetPlansForClient_ReturnsMappedDtos()
    {
        var plans = new List<NutritionPlan>
        {
            new()
            {
                NutritionPlanId = 1,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 1, 31),
                Meals = new List<Meal>
                {
                    new() { MealId = 10, Name = "Breakfast", Ingredients = new List<string> { "Eggs" }, Instructions = "Scramble" },
                },
            },
        };

        this.nutritionRepository
            .Setup(r => r.GetNutritionPlansForClientAsync(5))
            .ReturnsAsync(plans);

        var service = this.CreateService();
        var result = await service.GetPlansForClientAsync(5);

        Assert.Single(result);
        Assert.Equal(1, result[0].NutritionPlanId);
        Assert.Equal(new DateTime(2026, 1, 1), result[0].StartDate);
        Assert.Single(result[0].Meals);
        Assert.Equal("Breakfast", result[0].Meals[0].Name);
    }

    [Fact]
    public async Task GetPlansForClient_WithNoPlans_ReturnsEmptyList()
    {
        this.nutritionRepository
            .Setup(r => r.GetNutritionPlansForClientAsync(99))
            .ReturnsAsync(new List<NutritionPlan>());

        var service = this.CreateService();
        var result = await service.GetPlansForClientAsync(99);

        Assert.Empty(result);
    }
}
