using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;
using static ClassLibrary.Proxies.DataTransferObjectToDomainModelMappers;

namespace ClassLibrary.Proxies;

public sealed class AchievementsProxy : IAchievementsProxy
{
    private readonly HttpClient httpClient;
    private const string BaseAddress = ApiBaseUrl.BASE_URL + "/api";
    private const string ClientRoute = "client";

    public AchievementsProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Achievement>> GetAchievementsAsync(int clientId)
    {
        var achievementDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>($"{BaseAddress}/{ClientRoute}/{clientId}/achievements");
        return DataTransferObjectToDomainModelMappers.MapAchievements(achievementDataTransferObjects);
    }
}

