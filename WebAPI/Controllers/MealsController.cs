using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly IMealService _mealService;

        
        public MealsController(IMealService mealService)
        {
            _mealService = mealService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFiltered([FromQuery] MealFilterDto filter)
        {
           
            var results = await _mealService.GetFilteredMealsAsync(filter);
            return Ok(results);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItemDto>> GetById(int id)
        {
            var meal = await _mealService.GetByIdAsync(id);
            if (meal == null) return NotFound();

            return Ok(meal);
        }

       
        [HttpGet("{id}/ingredients")]
        public async Task<ActionResult<string>> GetIngredients(int id)
        {
            
            var ingredients = await _mealService.GetFormattedIngredientsAsync(id);
            return Ok(ingredients);
        }

        
        [HttpPost("favorite")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequestDto request)
        {
            
            await _mealService.ToggleFavoriteAsync(request.UserId, request.MealId, request.IsFavorite);
            return Ok();
        }
    }
}