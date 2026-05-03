using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DailyLogController : ControllerBase
{
    private readonly IDailyLogService dailyLogService;

    public DailyLogController(IDailyLogService dailyLogService)
    {
        this.dailyLogService = dailyLogService;
    }

    [HttpGet("user/{userId:int}/has-logs")]
    public async Task<IActionResult> HasAnyLogs(int userId)
    {
        var hasLogs = await this.dailyLogService.HasAnyLogsAsync(userId);
        return this.Ok(hasLogs);
    }

    [HttpGet("user/{userId:int}/today")]
    public async Task<IActionResult> GetTodayTotals(int userId)
    {
        var totals = await this.dailyLogService.GetTodayTotalsAsync(userId);
        return this.Ok(totals);
    }

    [HttpGet("user/{userId:int}/week")]
    public async Task<IActionResult> GetCurrentWeekTotals(int userId)
    {
        var totals = await this.dailyLogService.GetCurrentWeekTotalsAsync(userId);
        return this.Ok(totals);
    }

    [HttpGet("user/{userId:int}/targets")]
    public async Task<IActionResult> GetNutritionTargets(int userId)
    {
        var targets = await this.dailyLogService.GetCurrentUserNutritionTargetsAsync(userId);

        if (targets is null)
        {
            return this.NotFound();
        }

        return this.Ok(targets);
    }

    [HttpGet("user/{userId:int}/burned-calories")]
    public async Task<IActionResult> GetTodayBurnedCalories(int userId)
    {
        var burnedCalories = await this.dailyLogService.GetTodayBurnedCaloriesAsync(userId);
        return this.Ok(burnedCalories);
    }

    [HttpGet("fooditems/search")]
    public async Task<IActionResult> SearchFoodItems([FromQuery] string? searchTerm)
    {
        var items = await this.dailyLogService.SearchFoodItemsAsync(searchTerm);
        return this.Ok(items);
    }

    [HttpGet("fooditems/autocomplete")]
    public async Task<IActionResult> GetFoodItemsForAutocomplete()
    {
        var items = await this.dailyLogService.GetFoodItemsForAutocompleteAsync();
        return this.Ok(items);
    }

    [HttpPost("user/{userId:int}/log")]
    public async Task<IActionResult> LogFoodItem(int userId, [FromBody] LogMealRequestDto request)
    {
        await this.dailyLogService.LogFoodItemAsync(userId, request);
        return this.NoContent();
    }
}