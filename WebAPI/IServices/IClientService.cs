using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IClientService
{
    Task<IReadOnlyList<AchievementDataTransferObject>> GetAchievementsAsync(int clientId);

    Task<IReadOnlyList<NotificationDataTransferObject>> GetNotificationsAsync(int clientId);

    Task<NutritionPlanDataTransferObject?> GetActiveNutritionPlanAsync(int clientId);

    Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId);

    Task<bool> FinalizeWorkoutAsync(FinalizeWorkoutRequestDataTransferObject request);

    Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId);

    Task<PreviousBestWeightsDataTransferObject> GetPreviousBestWeightsAsync(int clientId);

    Task<ClientProfileSnapshotDataTransferObject> GetClientProfileSnapshotAsync(int clientId);

    Task<bool> ConfirmDeloadAsync(ConfirmDeloadRequestDataTransferObject request);

    Task<bool> SyncNutritionAsync(NutritionSyncRequestDataTransferObject request);

    Task<bool> ModifyWorkoutAsync(WorkoutLogDataTransferObject updatedWorkoutLog);
}
