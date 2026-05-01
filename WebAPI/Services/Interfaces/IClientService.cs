using ClassLibrary.DTOs;

namespace WebAPI.Services.Interfaces;

public interface IClientService
{
    Task<IReadOnlyList<AchievementDataTransferObject>> GetAchievementsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationDataTransferObject>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<NutritionPlanDataTransferObject?> GetActiveNutritionPlanAsync(int clientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default);

    Task<bool> FinalizeWorkoutAsync(FinalizeWorkoutRequestDataTransferObject request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<PreviousBestWeightsDataTransferObject> GetPreviousBestWeightsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<ClientProfileSnapshotDataTransferObject> GetClientProfileSnapshotAsync(int clientId, CancellationToken cancellationToken = default);

    Task<bool> ConfirmDeloadAsync(ConfirmDeloadRequestDataTransferObject request, CancellationToken cancellationToken = default);

    Task<bool> SyncNutritionAsync(NutritionSyncRequestDataTransferObject request, CancellationToken cancellationToken = default);
}
