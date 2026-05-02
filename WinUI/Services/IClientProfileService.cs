namespace WinUI.Services;

public interface IClientProfileService
{
    Task GetClientProfile(int clientId);

    Task SyncNutrition(int clientId);
}
