using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClassLibrary.Models;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public sealed partial class AchievementsViewModel : ObservableObject
{
    private readonly IAchievementsProxy achievementsService;

    [ObservableProperty]
    private ObservableCollection<Achievement> achievements;

    [ObservableProperty]
    private bool isLoading;

    public AchievementsViewModel(IAchievementsProxy achievementsService)
    {
        this.achievementsService = achievementsService;
        this.achievements = new ObservableCollection<Achievement>();
    }

    [RelayCommand]
    private async Task LoadAchievementsAsync(int clientId)
    {
        IsLoading = true;

        try
        {
            Achievements.Clear();
            var result = await achievementsService.GetAchievementsAsync(clientId);

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

