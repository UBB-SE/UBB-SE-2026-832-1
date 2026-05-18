namespace ClassLibrary.Proxies.Interfaces;

using ClassLibrary.DTOs;
public interface IClientProfileProxy
{
    Task<ClientProfileSnapshotDataTransferObject> GetClientProfileAsync(int clientId);

    Task<ClientProfileSnapshotDataTransferObject> SyncNutritionAsync(int clientId, NutritionSyncRequestDataTransferObject request);
}



