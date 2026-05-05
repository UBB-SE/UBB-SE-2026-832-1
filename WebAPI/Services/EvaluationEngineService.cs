using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;
using WebAPI.Services.MilestoneChecks;

namespace WebAPI.Services;

public sealed class EvaluationEngineService : IEvaluationEngineService
{
    private const string UnlockedAchievementIcon = "&#xE73E;";
    private const string LockedAchievementIcon = "&#xE72E;";

    private static readonly IReadOnlyList<IMilestoneCheck> milestoneChecks = new List<IMilestoneCheck>
    {
        new WorkoutCountCheck("First Rep", 1),
        new WorkoutCountCheck("Getting Serious", 10),
        new WorkoutCountCheck("Gym Regular", 25),
        new WorkoutCountCheck("Iron Warrior", 50),
        new WorkoutCountCheck("Gym Legend", 100),
        new StreakCheck("3-Day Streak", requiredConsecutiveDays: 3),
        new StreakCheck("Week Warrior", requiredConsecutiveDays: 7),
        new WeeklyVolumeCheck("Iron Week", requiredWorkoutsPerWeek: 5),
    };

    private static readonly IReadOnlyList<LevelingTier> levelingTiers = new List<LevelingTier>
    {
        new LevelingTier(1, "Beginner", 0),
        new LevelingTier(2, "Trainee", 1),
        new LevelingTier(3, "Apprentice", 2),
        new LevelingTier(4, "Gym Novice", 3),
        new LevelingTier(5, "Gym Enthusiast", 5),
        new LevelingTier(6, "Athlete", 7),
        new LevelingTier(7, "Elite", 10),
    };

    private readonly IAchievementsRepository achievementsRepository;

    public EvaluationEngineService(IAchievementsRepository achievementsRepository)
    {
        this.achievementsRepository = achievementsRepository;
    }

    public async Task<IReadOnlyList<string>> EvaluateAsync(int clientId)
    {
        IReadOnlyList<AchievementShowcaseItem> showcaseItems =
            await this.achievementsRepository.GetAchievementShowcaseForClientAsync(clientId);

        Dictionary<string, AchievementShowcaseItem> catalogByTitle =
            new Dictionary<string, AchievementShowcaseItem>(StringComparer.OrdinalIgnoreCase);

        foreach (AchievementShowcaseItem item in showcaseItems)
        {
            catalogByTitle[item.Title] = item;
        }

        List<string> newlyUnlocked = new List<string>();

        foreach (IMilestoneCheck check in milestoneChecks)
        {
            if (!catalogByTitle.TryGetValue(check.AchievementTitle, out AchievementShowcaseItem? showcaseItem))
            {
                continue;
            }

            if (showcaseItem.IsUnlocked)
            {
                continue;
            }

            if (!await check.IsMetAsync(clientId, this.achievementsRepository))
            {
                continue;
            }

            bool awarded = await this.achievementsRepository.AwardAchievementAsync(clientId, showcaseItem.AchievementId);

            if (awarded)
            {
                newlyUnlocked.Add(check.AchievementTitle);
            }
        }

        return newlyUnlocked.AsReadOnly();
    }

    public async Task<RankShowcaseSnapshotDataTransferObject> BuildRankShowcaseAsync(int clientId)
    {
        IReadOnlyList<AchievementShowcaseItem> showcaseItems =
            await this.achievementsRepository.GetAchievementShowcaseForClientAsync(clientId);

        int unlockedCount = showcaseItems.Count(item => item.IsUnlocked);

        int level = levelingTiers[0].level;
        string rankTitle = levelingTiers[0].rankTitle;

        foreach (LevelingTier tier in levelingTiers)
        {
            if (unlockedCount >= tier.minimumAchievements)
            {
                level = tier.level;
                rankTitle = tier.rankTitle;
            }
        }

        (bool hasNextRank, double progressPercent, string nextRankInfo) =
            ComputeNextRankProgress(unlockedCount, level);

        string unlockedAchievementsDisplay = unlockedCount == 1
            ? "1 achievement unlocked"
            : $"{unlockedCount} achievements unlocked";

        List<AchievementDataTransferObject> showcaseAchievements = showcaseItems
            .Select(item => new AchievementDataTransferObject
            {
                AchievementId = item.AchievementId,
                Title = item.Title,
                Description = item.Description,
                Criteria = item.Criteria,
                IsUnlocked = item.IsUnlocked,
                Icon = item.IsUnlocked ? UnlockedAchievementIcon : LockedAchievementIcon,
            })
            .ToList();

        return new RankShowcaseSnapshotDataTransferObject
        {
            DisplayLevel = level,
            RankTitle = rankTitle,
            UnlockedAchievementsDisplay = unlockedAchievementsDisplay,
            LevelDisplayLine = $"Level {level}: {rankTitle}",
            ProgressPercent = progressPercent,
            NextRankInfo = nextRankInfo,
            HasNextRank = hasNextRank,
            ShowcaseAchievements = showcaseAchievements,
        };
    }

    private static (bool hasNextRank, double progressPercent, string nextRankInfo) ComputeNextRankProgress(
        int unlockedCount,
        int level)
    {
        int currentIndex = -1;

        for (int i = 0; i < levelingTiers.Count; i++)
        {
            if (levelingTiers[i].level == level)
            {
                currentIndex = i;
                break;
            }
        }

        int nextIndex = currentIndex + 1;

        if (currentIndex < 0 || nextIndex >= levelingTiers.Count)
        {
            return (false, 100.0, "Max rank reached — keep going!");
        }

        int bandStart = levelingTiers[currentIndex].minimumAchievements;
        int bandEnd = levelingTiers[nextIndex].minimumAchievements;
        int earned = unlockedCount - bandStart;
        int needed = bandEnd - bandStart;

        double progressPercent = needed > 0
            ? Math.Min(100, Math.Round(earned * 100.0 / needed, 1))
            : 100;

        int remaining = Math.Max(0, bandEnd - unlockedCount);

        string nextRankInfo =
            $"Next: Level {levelingTiers[nextIndex].level}: {levelingTiers[nextIndex].rankTitle} – " +
            $"{remaining} more achievement{(remaining == 1 ? string.Empty : "s")} to go";

        return (true, progressPercent, nextRankInfo);
    }
}
