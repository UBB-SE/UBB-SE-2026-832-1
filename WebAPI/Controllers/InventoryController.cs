using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInventory(int userId, CancellationToken cancellationToken)
    {
        var inventory = await this.inventoryService.GetUserInventoryAsync(userId, cancellationToken);
        return this.Ok(inventory);
    }

    [HttpGet("ingredients")]
    public async Task<IActionResult> GetAllIngredients(CancellationToken cancellationToken)
    {
        var ingredients = await this.inventoryService.GetAllIngredientsAsync(cancellationToken);
        return this.Ok(ingredients);
    }

    [HttpPost("add-to-pantry")]
    public async Task<IActionResult> AddToPantry([FromBody] AddToPantryRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        await this.inventoryService.AddToPantryAsync(request, cancellationToken);
        return this.Ok();
    }

    [HttpPost("add-ingredient-by-name")]
    public async Task<IActionResult> AddIngredientByName([FromBody] AddIngredientByNameRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        await this.inventoryService.AddIngredientByNameToPantryAsync(request, cancellationToken);
        return this.Ok();
    }

    [HttpPost("consume-meal")]
    public async Task<IActionResult> ConsumeMeal([FromBody] ConsumeMealRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        var success = await this.inventoryService.ConsumeMealAsync(request, cancellationToken);
        if (!success)
        {
            return this.BadRequest("Insufficient ingredients in inventory to consume meal.");
        }

        return this.Ok();
    }

    [HttpDelete("{inventoryId:int}")]
    public async Task<IActionResult> RemoveItem(int inventoryId, CancellationToken cancellationToken)
    {
        await this.inventoryService.RemoveItemAsync(inventoryId, cancellationToken);
        return this.Ok();
    }
}
