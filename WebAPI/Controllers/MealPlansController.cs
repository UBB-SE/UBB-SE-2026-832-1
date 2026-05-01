using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

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
    public async Task<IActionResult> GetById(int id)
    {
        var mealPlan = await this.mealPlanService.GetByIdAsync(id);

        if (mealPlan is null)
        {
            return this.NotFound();
        }

        return this.Ok(mealPlan);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var mealPlans = await this.mealPlanService.GetByUserIdAsync(userId);
        return this.Ok(mealPlans);
    }

    [HttpPost("{mealPlanId:int}/fooditems/{foodItemId:int}")]
    public async Task<IActionResult> AddFoodItem(int mealPlanId, int foodItemId)
    {
        await this.mealPlanService.AddFoodItemToPlanAsync(mealPlanId, foodItemId);
        return this.NoContent();
    }

    [HttpDelete("{mealPlanId:int}/fooditems/{foodItemId:int}")]
    public async Task<IActionResult> RemoveFoodItem(int mealPlanId, int foodItemId)
    {
        await this.mealPlanService.RemoveFoodItemFromPlanAsync(mealPlanId, foodItemId);
        return this.NoContent();
    }

    [HttpGet("{mealPlanId:int}/fooditems")]
    public async Task<IActionResult> GetFoodItems(int mealPlanId)
    {
        var foodItems = await this.mealPlanService.GetFoodItemsForPlanAsync(mealPlanId);
        return this.Ok(foodItems);
    }
}
