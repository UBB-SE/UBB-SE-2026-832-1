namespace ClassLibrary.Models;

public sealed class RankShowcaseSnapshot
{
    public int DisplayLevel { get; set; }

    public string RankTitle { get; set; } = string.Empty;

    public string UnlockedAchievementsDisplay { get; set; } = string.Empty;

    public string LevelDisplayLine { get; set; } = string.Empty;

    public double ProgressPercent { get; set; }

    public string NextRankInfo { get; set; } = string.Empty;

    public bool HasNextRank { get; set; }

    public IReadOnlyList<AchievementShowcaseItem> ShowcaseAchievements { get; set; } = Array.Empty<AchievementShowcaseItem>();
}
