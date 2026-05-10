using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using static WinUI.Services.DataTransferObjectToDomainModelMappers;

namespace WinUI.Services;

public sealed class AchievementsService : IAchievementsService
{
    private readonly HttpClient httpClient;
    private const string BaseAddress = ApiBaseUrl.BASE_URL + "/api";
    private const string ClientRoute = "client";

    public AchievementsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Achievement>> GetAchievementsAsync(int clientId)
    {
        var achievementDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>($"{BaseAddress}/{ClientRoute}/{clientId}/achievements");
        return DataTransferObjectToDomainModelMappers.MapAchievements(achievementDataTransferObjects);
    }
}
