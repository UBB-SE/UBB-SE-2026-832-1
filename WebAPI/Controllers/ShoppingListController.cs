using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListService _shoppingListService;

        public ShoppingListController(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ShoppingItem>>> GetByUserId(int userId)
        {
            var items = await _shoppingListService.GetAllByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpPost("generate/{userId}")]
        public async Task<IActionResult> GenerateFromMealPlan(int userId)
        {
            
            await _shoppingListService.GenerateShoppingListFromMealPlanAsync(userId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shoppingListService.DeleteAsync(id);
            return NoContent();
        }
    }
}
