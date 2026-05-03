namespace WinUI.Services;
using ClassLibrary.DTOs;
public interface IClientProfileService
{
    Task<ClientProfileSnapshotDataTransferObject> GetClientProfileAsync(int clientId);

    Task<ClientProfileSnapshotDataTransferObject> SyncNutritionAsync(int clientId, NutritionSyncRequestDataTransferObject request);
}
