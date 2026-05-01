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
    public async Task<IActionResult> GetUserInventory(int userId)
    {
        var inventory = await this.inventoryService.GetUserInventoryAsync(userId);
        return this.Ok(inventory);
    }

    [HttpGet("ingredients")]
    public async Task<IActionResult> GetAllIngredients()
    {
        var ingredients = await this.inventoryService.GetAllIngredientsAsync();
        return this.Ok(ingredients);
    }

    [HttpPost("add-to-pantry")]
    public async Task<IActionResult> AddToPantry([FromBody] AddToPantryRequestDataTransferObject request)
    {
        await this.inventoryService.AddToPantryAsync(request);
        return this.Ok();
    }

    [HttpPost("add-ingredient-by-name")]
    public async Task<IActionResult> AddIngredientByName([FromBody] AddIngredientByNameRequestDataTransferObject request)
    {
        await this.inventoryService.AddIngredientByNameToPantryAsync(request);
        return this.Ok();
    }

    [HttpPost("consume-meal")]
    public async Task<IActionResult> ConsumeMeal([FromBody] ConsumeMealRequestDataTransferObject request)
    {
        var success = await this.inventoryService.ConsumeMealAsync(request);
        if (!success)
        {
            return this.BadRequest("Insufficient ingredients in inventory to consume meal.");
        }

        return this.Ok();
    }

    [HttpDelete("{inventoryId:int}")]
    public async Task<IActionResult> RemoveItem(int inventoryId)
    {
        await this.inventoryService.RemoveItemAsync(inventoryId);
        return this.Ok();
    }
}
