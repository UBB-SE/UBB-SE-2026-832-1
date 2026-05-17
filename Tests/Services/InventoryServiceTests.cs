using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using FluentAssertions;
using NSubstitute;
using WebAPI.Services;
using Xunit;

namespace WebAPI.Tests.Services
{
    public class InventoryServiceTests
    {
        private readonly IInventoryRepository _mockInventoryRepo;
        private readonly IMealPlanRepository _mockMealPlanRepo;
        private readonly IIngredientRepository _mockIngredientRepo;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _mockInventoryRepo = Substitute.For<IInventoryRepository>();
            _mockMealPlanRepo = Substitute.For<IMealPlanRepository>();
            _mockIngredientRepo = Substitute.For<IIngredientRepository>();
            _service = new InventoryService(_mockIngredientRepo, _mockInventoryRepo, _mockMealPlanRepo);
        }

        [Fact]
        public async Task ConsumeMealAsync_WhenNotEnoughStock_ReturnsFalse()
        {
            var request = new ConsumeMealRequestDataTransferObject { UserId = 1, MealPlanId = 10 };

            _mockMealPlanRepo.GetIngredientIdsForMealPlanAsync(request.MealPlanId)
                .Returns(new List<int> { 5 });

            var currentInventory = new List<Inventory>
            {
                new Inventory { InventoryId = 1, Ingredient = new Ingredient { IngredientId = 5 }, QuantityGrams = 50 }
            };
            _mockInventoryRepo.GetAllByUserIdAsync(request.UserId).Returns(currentInventory);

            var result = await _service.ConsumeMealAsync(request);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ConsumeMealAsync_WhenExactAmount_DeletesInventoryRow()
        {
            var request = new ConsumeMealRequestDataTransferObject { UserId = 1, MealPlanId = 10 };
            _mockMealPlanRepo.GetIngredientIdsForMealPlanAsync(request.MealPlanId)
                .Returns(new List<int> { 5 }); 

            var currentInventory = new List<Inventory>
            {
                new Inventory { InventoryId = 100, Ingredient = new Ingredient { IngredientId = 5 }, QuantityGrams = 100 }
            };
            _mockInventoryRepo.GetAllByUserIdAsync(request.UserId).Returns(currentInventory);

            var result = await _service.ConsumeMealAsync(request);

            result.Should().BeTrue();
            await _mockInventoryRepo.Received(1).DeleteAsync(100);
        }

        [Fact]
        public async Task ConsumeMealAsync_WhenExcessStock_UpdatesQuantity()
        {
            var request = new ConsumeMealRequestDataTransferObject { UserId = 1, MealPlanId = 10 };
            _mockMealPlanRepo.GetIngredientIdsForMealPlanAsync(request.MealPlanId)
                .Returns(new List<int> { 5 }); 

            var currentInventory = new List<Inventory>
            {
                new Inventory { InventoryId = 100, Ingredient = new Ingredient { IngredientId = 5 }, QuantityGrams = 250 }
            };
            _mockInventoryRepo.GetAllByUserIdAsync(request.UserId).Returns(currentInventory);

            var result = await _service.ConsumeMealAsync(request);

            result.Should().BeTrue();
            await _mockInventoryRepo.Received(1).UpdateAsync(Arg.Is<Inventory>(i => i.QuantityGrams == 150));
        }

        [Fact]
        public async Task AddToPantryAsync_CreatesInventoryWithCorrectFields()
        {
            var request = new AddToPantryRequestDataTransferObject
            {
                UserId = 1,
                IngredientId = 5,
                QuantityGrams = 250
            };

            await _service.AddToPantryAsync(request);

            await _mockInventoryRepo.Received(1).AddAsync(Arg.Is<Inventory>(i =>
                i.UserId == 1 &&
                i.IngredientId == 5 &&
                i.QuantityGrams == 250
            ));
        }
    }
}