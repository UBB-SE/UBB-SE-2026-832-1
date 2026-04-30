using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/trainer")]
public class TrainerController : ControllerBase
{
    private readonly ITrainerService trainerService;

    public TrainerController(ITrainerService trainerService)
    {
        this.trainerService = trainerService;
    }

    [HttpGet("{trainerId}/clients")]
    public async Task<IActionResult> GetAssignedClients(int trainerId)
    {
        var result = await trainerService.GetAssignedClientsAsync(trainerId);
        return Ok(result);
    }

    [HttpGet("client-history/{clientId}")]
    public async Task<IActionResult> GetClientWorkoutHistory(int clientId)
    {
        var result = await trainerService.GetClientWorkoutHistoryAsync(clientId);
        return Ok(result);
    }

    [HttpPost("workout-feedback")]
    public async Task<IActionResult> SaveWorkoutFeedback([FromBody] WorkoutFeedbackRequestDto request)
    {
        var result = await trainerService.SaveWorkoutFeedbackAsync(request);
        return Ok(result);
    }

    [HttpGet("available-workouts/{clientId}")]
    public async Task<IActionResult> GetAvailableWorkouts(int clientId)
    {
        var result = await trainerService.GetAvailableWorkoutsAsync(clientId);
        return Ok(result);
    }

    [HttpDelete("workout-template/{templateId}")]
    public async Task<IActionResult> DeleteWorkoutTemplate(int templateId)
    {
        var result = await trainerService.DeleteWorkoutTemplateAsync(templateId);
        return Ok(result);
    }

    [HttpPost("assign-routine")]
    public async Task<IActionResult> AssignNewRoutine([FromBody] RoutineRequestDto request)
    {
        var (success, errorMessage) = await trainerService.AssignNewRoutineAsync(request);
        if (!success)
        {
            return BadRequest(errorMessage);
        }

        return Ok(true);
    }

    [HttpGet("exercise-names")]
    public async Task<IActionResult> GetAllExerciseNames()
    {
        var result = await trainerService.GetAllExerciseNamesAsync();
        return Ok(result);
    }

    [HttpPost("assign-nutrition-plan")]
    public async Task<IActionResult> CreateAndAssignNutritionPlan([FromBody] NutritionPlanRequestDto request)
    {
        var result = await trainerService.CreateAndAssignNutritionPlanAsync(request);
        return Ok(result);
    }
}
