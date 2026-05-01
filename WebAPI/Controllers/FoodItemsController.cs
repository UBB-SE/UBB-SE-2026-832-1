using ClassLibrary.Filters;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FoodItemsController : ControllerBase
{
    private readonly IFoodItemService foodItemService;

    public FoodItemsController(IFoodItemService foodItemService)
    {
        this.foodItemService = foodItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var foodItems = await this.foodItemService.GetAllAsync();
        return this.Ok(foodItems);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var foodItem = await this.foodItemService.GetByIdAsync(id);

        if (foodItem is null)
        {
            return this.NotFound();
        }

        return this.Ok(foodItem);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered([FromQuery] FoodItemFilter filter)
    {
        var foodItems = await this.foodItemService.GetFilteredAsync(filter);
        return this.Ok(foodItems);
    }
}
