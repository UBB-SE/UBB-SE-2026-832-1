using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/trainer")]
public sealed class TrainerController : ControllerBase
{
    private readonly ITrainerService trainerService;

    public TrainerController(ITrainerService trainerService)
    {
        this.trainerService = trainerService;
    }

    [HttpGet("{trainerId}/assigned-clients")]
    public async Task<IActionResult> GetAssignedClients(int trainerId)
    {
        try
        {
            var clients = await this.trainerService.GetAssignedClientsAsync(trainerId);
            return this.Ok(clients);
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{clientId}/workout-history")]
    public async Task<IActionResult> GetClientWorkoutHistory(int clientId)
    {
        try
        {
            var history = await this.trainerService.GetClientWorkoutHistoryAsync(clientId);
            return this.Ok(history);
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("save-workout-feedback")]
    public async Task<IActionResult> SaveWorkoutFeedback([FromBody] SaveWorkoutFeedbackRequestDataTransferObject request)
    {
        var success = await this.trainerService.SaveWorkoutFeedbackAsync(request);
        if (!success)
        {
            return this.NotFound();
        }

        return this.Ok();
    }

    [HttpPost("assign-workout")]
    public IActionResult AssignWorkout()
    {
        return this.StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("{clientId}/available-workouts")]
    public async Task<IActionResult> GetAvailableWorkouts(int clientId)
    {
        try
        {
            var workouts = await this.trainerService.GetAvailableWorkoutsAsync(clientId);
            return this.Ok(workouts);
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("workout-template/{templateId}")]
    public async Task<IActionResult> DeleteWorkoutTemplate(int templateId)
    {
        try
        {
            var success = await this.trainerService.DeleteWorkoutTemplateAsync(templateId);
            if (!success)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("save-workout")]
    public async Task<IActionResult> SaveWorkout([FromBody] WorkoutTemplateDataTransferObject templateDto)
    {
        try
        {
            var success = await this.trainerService.SaveTrainerWorkoutAsync(templateDto);
            if (!success)
            {
                return this.BadRequest("Failed to save workout.");
            }

            return this.Ok();
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("assign-new-routine")]
    public async Task<IActionResult> AssignNewRoutine([FromBody] AssignNewRoutineRequestDataTransferObject request)
    {
        var result = await this.trainerService.AssignNewRoutineAsync(request);
        if (!result.Success)
        {
            return this.BadRequest(result.ErrorMessage);
        }

        return this.Ok();
    }

    [HttpGet("exercise-names")]
    public async Task<IActionResult> GetExerciseNames()
    {
        try
        {
            var names = await this.trainerService.GetAllExerciseNamesAsync();
            return this.Ok(names);
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("assign-nutrition-plan")]
    public async Task<IActionResult> AssignNutritionPlan([FromBody] AssignNutritionPlanRequestDataTransferObject request)
    {
        var success = await this.trainerService.AssignNutritionPlanAsync(request);
        if (!success)
        {
            return this.BadRequest("Failed to assign nutrition plan.");
        }

        return this.Ok();
    }

    [HttpPost("create-assign-nutrition-plan")]
    public async Task<IActionResult> CreateAndAssignNutritionPlan([FromBody] CreateNutritionPlanRequestDataTransferObject request)
    {
        var success = await this.trainerService.CreateAndAssignNutritionPlanAsync(request);
        if (!success)
        {
            return this.BadRequest("Failed to create and assign nutrition plan.");
        }

        return this.Ok();
    }
}
