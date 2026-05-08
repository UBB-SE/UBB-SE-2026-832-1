using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class RankShowcaseViewModel : ObservableObject
{
    private const string DEFAULT_LEVEL_DISPLAY_LINE = "Level 1: Beginner";
    private const string DEFAULT_UNLOCKED_ACHIEVEMENTS_DISPLAY = "0 achievements unlocked";
    private const string DEFAULT_NEXT_RANK_INFO = "Complete more achievements to unlock your next rank.";
    private const string RANK_SHOWCASE_LOAD_ERROR_FORMAT = "Failed to load rank showcase: {0}";


    private readonly IRankShowcaseService rankShowcaseService;
    private readonly WinUI.Services.UserSession userSession;

    [ObservableProperty]
    private string levelDisplayLine = DEFAULT_LEVEL_DISPLAY_LINE;

    [ObservableProperty]
    private string unlockedAchievementsDisplay = DEFAULT_UNLOCKED_ACHIEVEMENTS_DISPLAY;

    [ObservableProperty]
    private string nextRankInfo = DEFAULT_NEXT_RANK_INFO;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private Visibility loadingVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private ObservableCollection<AchievementShowcaseItem> showcaseAchievements = [];

    public RankShowcaseViewModel(IRankShowcaseService rankShowcaseService, WinUI.Services.UserSession userSession)
    {
        this.rankShowcaseService = rankShowcaseService;
        this.userSession = userSession;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await this.LoadAsync().ConfigureAwait(true);
    }

    public async Task LoadAsync()
    {
        try
        {
            this.IsLoading = true;
            this.LoadingVisibility = Visibility.Visible;
            this.ErrorMessage = string.Empty;

            RankShowcaseSnapshot snapshot = await this.rankShowcaseService.GetRankShowcaseAsync(this.userSession.CurrentClientId);

            this.LevelDisplayLine = snapshot.LevelDisplayLine;
            this.UnlockedAchievementsDisplay = snapshot.UnlockedAchievementsDisplay;
            this.NextRankInfo = snapshot.NextRankInfo;

            this.ShowcaseAchievements.Clear();
            foreach (AchievementShowcaseItem showcaseAchievement in snapshot.ShowcaseAchievements)
            {
                this.ShowcaseAchievements.Add(showcaseAchievement);
            }
        }
        catch (Exception exception)
        {
            this.ErrorMessage = string.Format(RANK_SHOWCASE_LOAD_ERROR_FORMAT, exception.Message);
        }
        finally
        {
            this.IsLoading = false;
            this.LoadingVisibility = Visibility.Collapsed;
        }
    }
}
