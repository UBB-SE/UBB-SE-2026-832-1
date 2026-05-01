using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Services;
using ClassLibrary.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListService shoppingListService;

        public ShoppingListController(IShoppingListService shoppingListService)
        {
            this.shoppingListService = shoppingListService;
        }

        [HttpPost("user/{userId}")] 
        public async Task<IActionResult> AddItem(int userId, [FromBody] AddShoppingItemRequest request)
        {
            await shoppingListService.AddItemAsync(userId, request);
            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ShoppingItemDto>>> GetByUserId(int userId)
        {
            var items = await shoppingListService.GetShoppingItemsAsync(userId);
            return Ok(items);
        }

        [HttpPost("generate/{userId}")]
        public async Task<IActionResult> GenerateFromMealPlan(int userId)
        {
            await shoppingListService.GenerateShoppingListFromMealPlanAsync(userId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await shoppingListService.DeleteAsync(id);
            return NoContent();
        }
    }
}
