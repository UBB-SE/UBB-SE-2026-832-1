using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebApi.Services.Controllers;

[ApiController]
[Route("api/calendar-workout-catalog")]
public sealed class CalendarWorkoutCatalogController : ControllerBase
{
    private readonly ICalendarWorkoutCatalogService calendarWorkoutCatalogService;

    public CalendarWorkoutCatalogController(ICalendarWorkoutCatalogService calendarWorkoutCatalogService)
    {
        this.calendarWorkoutCatalogService = calendarWorkoutCatalogService;
    }

    [HttpPost("available-workouts")]
    public async Task<IActionResult> GetAvailableWorkouts([FromBody] GetAvailableWorkoutsRequestDataTransferObject request)
    {
        var response = await this.calendarWorkoutCatalogService.GetAvailableWorkoutsAsync(request.ClientId, request.Timeout);
        return this.Ok(response);
    }
}
