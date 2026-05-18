using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class RankShowcaseProxy : IRankShowcaseProxy
{
    private const string ApiBaseAddress = "http://localhost:5000/api"; private const string EvaluationRoute = "evaluation";
    private readonly HttpClient httpClient;

    public RankShowcaseProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<RankShowcaseSnapshot> GetRankShowcaseAsync(int clientId)
    {
        var snapshotDataTransferObject = await this.httpClient.GetFromJsonAsync<RankShowcaseSnapshotDataTransferObject>(
            $"{ApiBaseAddress}/{EvaluationRoute}/{clientId}/rank-showcase");

        if (snapshotDataTransferObject is null)
        {
            return new RankShowcaseSnapshot();
        }

        return MapRankShowcaseSnapshot(snapshotDataTransferObject);
    }

    private static RankShowcaseSnapshot MapRankShowcaseSnapshot(RankShowcaseSnapshotDataTransferObject snapshotDataTransferObject)
    {
        return new RankShowcaseSnapshot
        {
            DisplayLevel = snapshotDataTransferObject.DisplayLevel,
            RankTitle = snapshotDataTransferObject.RankTitle,
            UnlockedAchievementsDisplay = snapshotDataTransferObject.UnlockedAchievementsDisplay,
            LevelDisplayLine = snapshotDataTransferObject.LevelDisplayLine,
            ProgressPercent = snapshotDataTransferObject.ProgressPercent,
            NextRankInfo = snapshotDataTransferObject.NextRankInfo,
            HasNextRank = snapshotDataTransferObject.HasNextRank,
            ShowcaseAchievements = snapshotDataTransferObject.ShowcaseAchievements
                .Select(MapAchievementShowcaseItem)
                .ToList(),
        };
    }

    private static AchievementShowcaseItem MapAchievementShowcaseItem(AchievementDataTransferObject achievementDataTransferObject)
    {
        return new AchievementShowcaseItem
        {
            AchievementId = achievementDataTransferObject.AchievementId,
            Title = achievementDataTransferObject.Title,
            Description = achievementDataTransferObject.Description,
            Criteria = achievementDataTransferObject.Criteria,
            IsUnlocked = achievementDataTransferObject.IsUnlocked,
        };
    }
}


