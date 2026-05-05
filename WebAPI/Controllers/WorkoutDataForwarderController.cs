using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;
using ClassLibrary.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/workout-data-forwarder")]
public class WorkoutDataForwarderController : ControllerBase
{
    private readonly IWorkoutDataForwarder workoutDataForwarder;

    public WorkoutDataForwarderController(IWorkoutDataForwarder workoutDataForwarder)
    {
        this.workoutDataForwarder = workoutDataForwarder;
    }

    [HttpPost("forward-completed-workout")]
    public async Task<IActionResult> ForwardWorkout([FromBody] WorkoutLogRequestDto request)
    {
        var result = await workoutDataForwarder.ForwardCompletedWorkoutAsync(request);
        return Ok(result);
    }
}