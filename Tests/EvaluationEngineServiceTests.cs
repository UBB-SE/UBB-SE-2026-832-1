using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class EvaluationEngineServiceTests
{
    private readonly Mock<IAchievementsRepository> achievementsRepo = new();

    private EvaluationEngineService CreateService() => new(this.achievementsRepo.Object);

    [Fact]
    public async Task EvaluateAsync_AllAchievementsAlreadyUnlocked_ReturnsEmptyList()
    {
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { AchievementId = 1, Title = "3-Day Streak", IsUnlocked = true },
            new() { AchievementId = 2, Title = "Iron Week", IsUnlocked = true },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.EvaluateAsync(1);

        Assert.Empty(result);

        this.achievementsRepo.Verify(
            r => r.AwardAchievementAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateAsync_EmptyShowcase_ReturnsEmptyList()
    {
        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(new List<AchievementShowcaseItem>());

        var service = this.CreateService();
        var result = await service.EvaluateAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task BuildRankShowcaseAsync_NoAchievementsUnlocked_ReturnsLevel1()
    {
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { Title = "Test", IsUnlocked = false },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(1);

        Assert.Equal(1, result.DisplayLevel);
        Assert.Equal("0 achievements unlocked", result.UnlockedAchievementsDisplay);
        Assert.True(result.HasNextRank);
    }

    [Fact]
    public async Task BuildRankShowcaseAsync_OneAchievementUnlocked_UsesSingularForm()
    {
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { Title = "First Step", IsUnlocked = true },
            new() { Title = "Locked One", IsUnlocked = false },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(1);

        Assert.Equal("1 achievement unlocked", result.UnlockedAchievementsDisplay);
    }

    [Fact]
    public async Task BuildRankShowcaseAsync_NineUnlocked_ProgressIsAboveZero()
    {
        var showcase = new List<AchievementShowcaseItem>();
        for (int i = 0; i < 9; i++)
        {
            showcase.Add(new AchievementShowcaseItem { IsUnlocked = true });
        }

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(1);

        Assert.NotNull(result);
        Assert.True(result.ProgressPercent > 0);
        Assert.Contains("more achievement", result.NextRankInfo);
    }

    [Fact]
    public async Task BuildRankShowcaseAsync_MaxAchievements_ProgressIs100()
    {
        var showcase = new List<AchievementShowcaseItem>();
        for (int i = 0; i < 10; i++)
        {
            showcase.Add(new AchievementShowcaseItem { IsUnlocked = true });
        }

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(1);

        Assert.Equal(100, result.ProgressPercent);
        Assert.False(result.HasNextRank);
    }

    [Fact]
    public async Task EvaluateAsync_AchievementNotInMilestoneList_IsIgnored()
    {
        // titles that don't match any IMilestoneCheck get skipped via TryGetValue miss
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { AchievementId = 99, Title = "Nonexistent Badge", IsUnlocked = false },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(42))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.EvaluateAsync(42);

        Assert.Empty(result);
        this.achievementsRepo.Verify(
            r => r.AwardAchievementAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Showcase_should_map_unlock_icon_correctly()
    {
        var showcase = new List<AchievementShowcaseItem>
        {
            new() { AchievementId = 1, Title = "Locked", Description = "d", Criteria = "c", IsUnlocked = false },
            new() { AchievementId = 2, Title = "Unlocked", Description = "d", Criteria = "c", IsUnlocked = true },
        };

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(3))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(3);

        // locked items get the lock icon, unlocked get the check icon
        var locked = result.ShowcaseAchievements.Single(a => a.AchievementId == 1);
        var unlocked = result.ShowcaseAchievements.Single(a => a.AchievementId == 2);
        Assert.NotEqual(locked.Icon, unlocked.Icon);
        Assert.True(unlocked.IsUnlocked);
        Assert.False(locked.IsUnlocked);
    }

    [Fact]
    public async Task BuildRankShowcase_FiveUnlocked_ReachesGymEnthusiastTier()
    {
        // leveling tiers: 0→Beginner, 1→Trainee, 2→Apprentice, 3→Gym Novice, 5→Gym Enthusiast
        var showcase = Enumerable.Range(0, 5)
            .Select(_ => new AchievementShowcaseItem { IsUnlocked = true })
            .ToList();

        this.achievementsRepo
            .Setup(r => r.GetAchievementShowcaseForClientAsync(1))
            .ReturnsAsync(showcase);

        var service = this.CreateService();
        var result = await service.BuildRankShowcaseAsync(1);

        Assert.Equal(5, result.DisplayLevel);
        Assert.Contains("Gym Enthusiast", result.LevelDisplayLine);
    }
}
