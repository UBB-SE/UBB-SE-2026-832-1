using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClassLibrary.Models;
using WinUI.Services.Interfaces;

namespace WinUI.ViewModels;

public sealed partial class AchievementsViewModel : ObservableObject
{
    private readonly IAchievementService achievementService;

    [ObservableProperty]
    private ObservableCollection<Achievement> achievements;

    [ObservableProperty]
    private bool isLoading;

    public AchievementsViewModel(IAchievementService achievementService)
    {
        this.achievementService = achievementService;
        this.achievements = new ObservableCollection<Achievement>();
    }

    [RelayCommand]
    private async Task LoadAchievementsAsync(int clientId)
    {
        IsLoading = true;

        try
        {
            Achievements.Clear();
            var result = await achievementService.GetAchievementsAsync(clientId);

            foreach (var achievement in result)
            {
                Achievements.Add(achievement);
            }
        }
        catch (Exception)
        {
            Achievements.Clear();
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }
}