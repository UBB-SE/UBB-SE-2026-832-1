namespace WinUI.Services;

public interface IClientProfileService
{
    Task GetClientProfileAsync(int clientId);

    Task SyncNutritionAsync(int clientId);
}
