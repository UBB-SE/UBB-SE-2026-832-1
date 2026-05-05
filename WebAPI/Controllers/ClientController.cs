using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/client")]
public sealed class ClientController : ControllerBase
{
    private readonly IClientService clientService;

    public ClientController(IClientService clientService)
    {
        this.clientService = clientService;
    }

    [HttpGet("{clientId}/achievements")]
    public async Task<IActionResult> GetAchievements(int clientId)
    {
        var achievements = await this.clientService.GetAchievementsAsync(clientId);
        return this.Ok(achievements);
    }

    [HttpGet("{clientId}/notifications")]
    public async Task<IActionResult> GetNotifications(int clientId)
    {
        var notifications = await this.clientService.GetNotificationsAsync(clientId);
        return this.Ok(notifications);
    }

    [HttpGet("{clientId}/active-nutrition-plan")]
    public async Task<IActionResult> GetActiveNutritionPlan(int clientId)
    {
        var plan = await this.clientService.GetActiveNutritionPlanAsync(clientId);
        if (plan == null)
        {
            return this.NotFound();
        }

        return this.Ok(plan);
    }

    [HttpGet("{clientId}/workout-history")]
    public async Task<IActionResult> GetWorkoutHistory(int clientId)
    {
        var history = await this.clientService.GetWorkoutHistoryAsync(clientId);
        return this.Ok(history);
    }

    [HttpGet("{clientId}/available-workouts")]
    public async Task<IActionResult> GetAvailableWorkouts(int clientId)
    {
        var workouts = await this.clientService.GetAvailableWorkoutsAsync(clientId);
        return this.Ok(workouts);
    }

    [HttpGet("{clientId}/previous-best-weights")]
    public async Task<IActionResult> GetPreviousBestWeights(int clientId)
    {
        var weights = await this.clientService.GetPreviousBestWeightsAsync(clientId);
        return this.Ok(weights);
    }

    [HttpGet("{clientId}/profile-snapshot")]
    public async Task<IActionResult> GetClientProfileSnapshot(int clientId)
    {
        var snapshot = await this.clientService.GetClientProfileSnapshotAsync(clientId);
        return this.Ok(snapshot);
    }

    [HttpPost("finalize-workout")]
    public async Task<IActionResult> FinalizeWorkout([FromBody] FinalizeWorkoutRequestDataTransferObject request)
    {
        var success = await this.clientService.FinalizeWorkoutAsync(request);
        if (!success)
        {
            return this.BadRequest("Failed to finalize workout.");
        }

        return this.Ok();
    }

    [HttpPut("modify-workout")]
    public async Task<IActionResult> ModifyWorkout([FromBody] WorkoutLogDataTransferObject updatedWorkoutLog)
    {
        var success = await this.clientService.ModifyWorkoutAsync(updatedWorkoutLog);
        if (!success)
        {
            return this.BadRequest("Failed to modify workout.");
        }

        return this.Ok();
    }

    [HttpPost("confirm-deload")]
    public async Task<IActionResult> ConfirmDeload([FromBody] ConfirmDeloadRequestDataTransferObject request)
    {
        try
        {
            var success = await this.clientService.ConfirmDeloadAsync(request);
            if (!success)
            {
                return this.BadRequest("Failed to confirm deload.");
            }

            return this.Ok();
        }
        catch (NotImplementedException)
        {
            return this.StatusCode(StatusCodes.Status501NotImplemented, "Deload confirmation is not yet implemented.");
        }
    }

    [HttpPost("sync-nutrition")]
    public async Task<IActionResult> SyncNutrition([FromBody] NutritionSyncRequestDataTransferObject request)
    {
        var success = await this.clientService.SyncNutritionAsync(request);
        if (!success)
        {
            return this.BadRequest("Failed to sync nutrition data.");
        }

        return this.Ok();
    }
}
