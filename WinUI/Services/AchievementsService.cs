using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public sealed class AchievementsService : IAchievementsService
{
    private readonly HttpClient httpClient;
    private const string ApiUrl = "https://localhost:7197/api";
    private const string Route = "client";

    public AchievementsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Achievement>> GetAchievementsAsync(int clientId)
    {
        var response = await this.httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>($"{ApiUrl}/{Route}/{clientId}/achievements");
        var dtos = response ?? new List<AchievementDataTransferObject>();
        return dtos.Select(dto => new Achievement
        {
            AchievementId = dto.AchievementId,
            Name = dto.Name,
            Description = dto.Description,
            Criteria = dto.Criteria,
        }).ToList();
    }
}
