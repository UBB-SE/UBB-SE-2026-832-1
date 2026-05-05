using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class WorkoutLogViewModel : ObservableObject
{
    private const string ERROR_LOADING_HISTORY_FORMAT = "Failed to load workout history: {0}";
    private const string ERROR_LOADING_WEIGHT_FORMAT = "Failed to load client weight: {0}";

    private readonly IWorkoutLogService workoutLogService;

    [ObservableProperty]
    private ObservableCollection<WorkoutLogDataTransferObject> workoutHistory = new();

    [ObservableProperty]
    private WorkoutLogDataTransferObject? selectedWorkout;

    [ObservableProperty]
    private double clientWeight;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public WorkoutLogViewModel(IWorkoutLogService workoutLogService)
    {
        this.workoutLogService = workoutLogService ?? throw new ArgumentNullException(nameof(workoutLogService));
    }

    public async Task LoadWorkoutHistoryAsync(int clientId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var history = await this.workoutLogService.GetWorkoutHistoryAsync(clientId);

            this.WorkoutHistory.Clear();
            foreach (var workoutLog in history)
            {
                this.WorkoutHistory.Add(workoutLog);
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = string.Format(ERROR_LOADING_HISTORY_FORMAT, exception.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadClientWeightAsync(int clientId)
    {
        try
        {
            ClientWeight = await this.workoutLogService.GetClientWeightAsync(clientId);
        }
        catch (Exception exception)
        {
            ErrorMessage = string.Format(ERROR_LOADING_WEIGHT_FORMAT, exception.Message);
        }
    }
}
