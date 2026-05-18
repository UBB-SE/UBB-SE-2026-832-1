using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogViewModel : ObservableObject
{
    private readonly IWorkoutLogProxy workoutLogService;

    public ObservableCollection<WorkoutLogItemViewModel> Logs { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool showEmptyState;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public event Action<int>? StartWorkoutRequested;

    public WorkoutLogViewModel(IWorkoutLogProxy workoutLogService)
    {
        this.workoutLogService = workoutLogService;
    }

    [RelayCommand]
    private async Task LoadLogs(int clientId)
    {
        try
        {
            this.IsLoading = true;
            this.ErrorMessage = string.Empty;
            this.Logs.Clear();
            this.ShowEmptyState = false;

            var dtos = await this.workoutLogService.GetWorkoutHistoryAsync(clientId);
            var logs = DataTransferObjectToDomainModelMappers.MapWorkoutLogs(dtos);

            foreach (var log in logs)
            {
                this.Logs.Add(new WorkoutLogItemViewModel(log));
            }

            this.ShowEmptyState = this.Logs.Count == 0;
        }
        catch (Exception ex)
        {
            this.Logs.Clear();
            this.ShowEmptyState = true;
            this.ErrorMessage = $"Failed to load workout logs: {ex.Message}";
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    [RelayCommand]
    private void StartWorkout(int clientId)
    {
        StartWorkoutRequested?.Invoke(clientId);
    }
}

