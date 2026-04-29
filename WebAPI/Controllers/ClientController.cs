using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;

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
    public async Task<IActionResult> GetAchievements(int clientId, CancellationToken cancellationToken)
    {
        var achievements = await this.clientService.GetAchievementsAsync(clientId, cancellationToken);
        return this.Ok(achievements);
    }

    [HttpGet("{clientId}/notifications")]
    public async Task<IActionResult> GetNotifications(int clientId, CancellationToken cancellationToken)
    {
        var notifications = await this.clientService.GetNotificationsAsync(clientId, cancellationToken);
        return this.Ok(notifications);
    }

    [HttpGet("{clientId}/active-nutrition-plan")]
    public async Task<IActionResult> GetActiveNutritionPlan(int clientId, CancellationToken cancellationToken)
    {
        var plan = await this.clientService.GetActiveNutritionPlanAsync(clientId, cancellationToken);
        if (plan == null)
        {
            return this.NotFound();
        }

        return this.Ok(plan);
    }

    [HttpGet("{clientId}/workout-history")]
    public async Task<IActionResult> GetWorkoutHistory(int clientId, CancellationToken cancellationToken)
    {
        var history = await this.clientService.GetWorkoutHistoryAsync(clientId, cancellationToken);
        return this.Ok(history);
    }

    [HttpGet("{clientId}/available-workouts")]
    public async Task<IActionResult> GetAvailableWorkouts(int clientId, CancellationToken cancellationToken)
    {
        var workouts = await this.clientService.GetAvailableWorkoutsAsync(clientId, cancellationToken);
        return this.Ok(workouts);
    }

    [HttpGet("{clientId}/previous-best-weights")]
    public async Task<IActionResult> GetPreviousBestWeights(int clientId, CancellationToken cancellationToken)
    {
        var weights = await this.clientService.GetPreviousBestWeightsAsync(clientId, cancellationToken);
        return this.Ok(weights);
    }

    [HttpGet("{clientId}/profile-snapshot")]
    public async Task<IActionResult> GetClientProfileSnapshot(int clientId, CancellationToken cancellationToken)
    {
        var snapshot = await this.clientService.GetClientProfileSnapshotAsync(clientId, cancellationToken);
        return this.Ok(snapshot);
    }

    [HttpPost("finalize-workout")]
    public async Task<IActionResult> FinalizeWorkout([FromBody] FinalizeWorkoutRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        var success = await this.clientService.FinalizeWorkoutAsync(request, cancellationToken);
        if (!success)
        {
            return this.BadRequest("Failed to finalize workout.");
        }

        return this.Ok();
    }

    [HttpPost("confirm-deload")]
    public async Task<IActionResult> ConfirmDeload([FromBody] ConfirmDeloadRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        var success = await this.clientService.ConfirmDeloadAsync(request, cancellationToken);
        if (!success)
        {
            return this.BadRequest("Failed to confirm deload.");
        }

        return this.Ok();
    }

    [HttpPost("sync-nutrition")]
    public async Task<IActionResult> SyncNutrition([FromBody] NutritionSyncRequestDataTransferObject request, CancellationToken cancellationToken)
    {
        var success = await this.clientService.SyncNutritionAsync(request, cancellationToken);
        if (!success)
        {
            return this.BadRequest("Failed to sync nutrition data.");
        }

        return this.Ok();
    }
}
