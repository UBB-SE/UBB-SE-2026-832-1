using ClassLibrary.Models;
using Moq;
using WinUI.Services;
using WinUI.ViewModels;

namespace Tests;

public class AchievementsViewModelTests
{
    private readonly Mock<IAchievementsService> mockAchievementsService;
    private readonly AchievementsViewModel viewModel;

    public AchievementsViewModelTests()
    {
        this.mockAchievementsService = new Mock<IAchievementsService>();
        this.viewModel = new AchievementsViewModel(this.mockAchievementsService.Object);
    }

    [Fact]
    public async Task LoadAchievements_ShouldPopulateCollection_WhenServiceReturnsItems()
    {
        var expectedAchievements = new List<Achievement>
        {
            new Achievement { AchievementId = 1, Name = "First Workout" },
            new Achievement { AchievementId = 2, Name = "Week Streak" },
        };
        this.mockAchievementsService
            .Setup(service => service.GetAchievementsAsync(1))
            .ReturnsAsync(expectedAchievements);

        await this.viewModel.LoadAchievementsCommand.ExecuteAsync(1);

        Assert.Equal(2, this.viewModel.Achievements.Count);
        Assert.Equal("First Workout", this.viewModel.Achievements[0].Name);
        Assert.Equal("Week Streak", this.viewModel.Achievements[1].Name);
    }

    [Fact]
    public async Task LoadAchievements_ShouldResultInEmptyCollection_WhenServiceReturnsEmpty()
    {
        this.mockAchievementsService
            .Setup(service => service.GetAchievementsAsync(1))
            .ReturnsAsync(new List<Achievement>());

        await this.viewModel.LoadAchievementsCommand.ExecuteAsync(1);

        Assert.Empty(this.viewModel.Achievements);
    }

    [Fact]
    public async Task LoadAchievements_ShouldSetIsLoadingFalse_AfterCompletion()
    {
        this.mockAchievementsService
            .Setup(service => service.GetAchievementsAsync(1))
            .ReturnsAsync(new List<Achievement>());

        await this.viewModel.LoadAchievementsCommand.ExecuteAsync(1);

        Assert.False(this.viewModel.IsLoading);
    }

    [Fact]
    public async Task LoadAchievements_ShouldClearPreviousItems_BeforeLoading()
    {
        var firstBatch = new List<Achievement>
        {
            new Achievement { AchievementId = 1, Name = "Old" },
        };
        var secondBatch = new List<Achievement>
        {
            new Achievement { AchievementId = 2, Name = "New" },
        };

        this.mockAchievementsService
            .SetupSequence(service => service.GetAchievementsAsync(1))
            .ReturnsAsync(firstBatch)
            .ReturnsAsync(secondBatch);

        await this.viewModel.LoadAchievementsCommand.ExecuteAsync(1);
        await this.viewModel.LoadAchievementsCommand.ExecuteAsync(1);

        Assert.Single(this.viewModel.Achievements);
        Assert.Equal("New", this.viewModel.Achievements[0].Name);
    }

    [Fact]
    public async Task LoadAchievements_ShouldSetIsLoadingFalse_WhenServiceThrows()
    {
        this.mockAchievementsService
            .Setup(service => service.GetAchievementsAsync(1))
            .ThrowsAsync(new HttpRequestException("Network error"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => this.viewModel.LoadAchievementsCommand.ExecuteAsync(1));

        Assert.False(this.viewModel.IsLoading);
        Assert.Empty(this.viewModel.Achievements);
    }
}
