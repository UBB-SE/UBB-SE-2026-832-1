using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed partial class WorkoutLogViewModel : ObservableObject
{
    private readonly IWorkoutLogService workoutLogService;

    public ObservableCollection<WorkoutLogItemViewModel> Logs { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool showEmptyState;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public event Action<int>? StartWorkoutRequested;

    public WorkoutLogViewModel(IWorkoutLogService workoutLogService)
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

    [RelayCommand]
    private void ToggleEditMode(WorkoutLogItemViewModel item)
    {
        if (item is null)
        {
            return;
        }

        if (item.IsEditMode)
        {
            item.CancelEditMode();
        }
        else
        {
            item.EnterEditMode();
        }
    }

    [RelayCommand]
    private async Task SaveEditedLog(WorkoutLogItemViewModel item)
    {
        if (item is null || !item.IsEditMode)
        {
            return;
        }

        try
        {
            this.ErrorMessage = string.Empty;
            WorkoutLog updated = item.BuildUpdatedWorkoutLog();
            bool ok = await this.workoutLogService.UpdateWorkoutLogAsync(updated);

            if (!ok)
            {
                this.ErrorMessage = "Failed to save workout changes.";
                return;
            }

            item.CommitEditMode();
        }
        catch (Exception ex)
        {
            this.ErrorMessage = $"Failed to save workout changes: {ex.Message}";
        }
    }
}
