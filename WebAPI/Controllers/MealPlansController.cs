using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MealPlansController : ControllerBase
{
    private readonly IMealPlanService mealPlanService;

    public MealPlansController(IMealPlanService mealPlanService)
    {
        this.mealPlanService = mealPlanService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var mealPlan = await this.mealPlanService.GetByIdAsync(id, cancellationToken);

        if (mealPlan is null)
        {
            return this.NotFound();
        }

        return this.Ok(mealPlan);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var mealPlans = await this.mealPlanService.GetByUserIdAsync(userId, cancellationToken);
        return this.Ok(mealPlans);
    }

    [HttpPost("{mealPlanId:int}/fooditems/{foodItemId:int}")]
    public async Task<IActionResult> AddFoodItem(int mealPlanId, int foodItemId, CancellationToken cancellationToken)
    {
        await this.mealPlanService.AddFoodItemToPlanAsync(mealPlanId, foodItemId, cancellationToken);
        return this.NoContent();
    }

    [HttpDelete("{mealPlanId:int}/fooditems/{foodItemId:int}")]
    public async Task<IActionResult> RemoveFoodItem(int mealPlanId, int foodItemId, CancellationToken cancellationToken)
    {
        await this.mealPlanService.RemoveFoodItemFromPlanAsync(mealPlanId, foodItemId, cancellationToken);
        return this.NoContent();
    }

    [HttpGet("{mealPlanId:int}/fooditems")]
    public async Task<IActionResult> GetFoodItems(int mealPlanId, CancellationToken cancellationToken)
    {
        var foodItems = await this.mealPlanService.GetFoodItemsForPlanAsync(mealPlanId, cancellationToken);
        return this.Ok(foodItems);
    }
}
