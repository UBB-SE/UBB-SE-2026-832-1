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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var foodItems = await this.foodItemService.GetAllAsync(cancellationToken);
        return this.Ok(foodItems);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var foodItem = await this.foodItemService.GetByIdAsync(id, cancellationToken);

        if (foodItem is null)
        {
            return this.NotFound();
        }

        return this.Ok(foodItem);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered([FromQuery] FoodItemFilter filter, CancellationToken cancellationToken)
    {
        var foodItems = await this.foodItemService.GetFilteredAsync(filter, cancellationToken);
        return this.Ok(foodItems);
    }
}
