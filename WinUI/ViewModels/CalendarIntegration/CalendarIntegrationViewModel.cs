using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClassLibrary.Models;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.Services.Interfaces;

namespace WinUI.ViewModels.CalendarIntegration;

public sealed partial class CalendarIntegrationViewModel : ObservableObject
{
    private const int DEFAULT_DURATION_WEEKS = 4;
    private const int DAY_COUNT = 7;
    private const int WORKOUT_LOAD_TIMEOUT_MILLISECONDS = 1500;
    private const int MINIMUM_DURATION_WEEKS = 1;
    private const int MAXIMUM_DURATION_WEEKS = 52;
    private static readonly string[] DEFAULT_DAY_NAMES = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    private static readonly bool[] DEFAULT_DAY_SELECTIONS = { false, true, true, true, true, true, false };

    private readonly ICalendarWorkoutCatalogService calendarWorkoutCatalogService;
    private readonly ICalendarExportService calendarExportService;
    private readonly IUserSession userSession;

    private ObservableCollection<WorkoutTemplate> availableWorkouts = new();
    private WorkoutTemplate? selectedWorkout;
    private int durationWeeks = DEFAULT_DURATION_WEEKS;
    private ObservableCollection<DaySelectionItem> selectedDays = new();
    private bool isLoading;
    private string generatedIcsContent = string.Empty;
    private InfoBarSeverity statusSeverity;
    private string statusTitle = string.Empty;
    private string statusMessage = string.Empty;
    private bool isStatusOpen;

    public ObservableCollection<WorkoutTemplate> AvailableWorkouts
    {
        get => this.availableWorkouts;
        set => SetProperty(ref this.availableWorkouts, value);
    }

    public WorkoutTemplate? SelectedWorkout
    {
        get => this.selectedWorkout;
        set => SetProperty(ref this.selectedWorkout, value);
    }

    public int DurationWeeks
    {
        get => this.durationWeeks;
        set => SetProperty(ref this.durationWeeks, value);
    }

    public ObservableCollection<DaySelectionItem> SelectedDays
    {
        get => this.selectedDays;
        set => SetProperty(ref this.selectedDays, value);
    }

    public bool IsLoading
    {
        get => this.isLoading;
        set => SetProperty(ref this.isLoading, value);
    }

    public string GeneratedIcsContent
    {
        get => this.generatedIcsContent;
        set => SetProperty(ref this.generatedIcsContent, value);
    }

    public InfoBarSeverity StatusSeverity
    {
        get => this.statusSeverity;
        set => SetProperty(ref this.statusSeverity, value);
    }

    public string StatusTitle
    {
        get => this.statusTitle;
        set => SetProperty(ref this.statusTitle, value);
    }

    public string StatusMessage
    {
        get => this.statusMessage;
        set => SetProperty(ref this.statusMessage, value);
    }

    public bool IsStatusOpen
    {
        get => this.isStatusOpen;
        set => SetProperty(ref this.isStatusOpen, value);
    }

    public CalendarIntegrationViewModel(ICalendarWorkoutCatalogService calendarWorkoutCatalogService, ICalendarExportService calendarExportService, IUserSession userSession)
    {
        this.calendarWorkoutCatalogService = calendarWorkoutCatalogService ?? throw new ArgumentNullException(nameof(calendarWorkoutCatalogService));
        this.calendarExportService = calendarExportService ?? throw new ArgumentNullException(nameof(calendarExportService));
        this.userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        InitializeDaySelection();
        int clientId = this.userSession.CurrentClientId;
        SetAvailableWorkouts(this.calendarWorkoutCatalogService.GetFallbackWorkouts(clientId));
    }

    public sealed class CalendarGenerationResult
    {
        public bool IsSuccessful { get; init; }

        public string Message { get; init; } = string.Empty;

        public string GeneratedCalendarContent { get; init; } = string.Empty;

        public static CalendarGenerationResult CreateSuccess(string generatedCalendarContent)
        {
            return new CalendarGenerationResult
            {
                IsSuccessful = true,
                GeneratedCalendarContent = generatedCalendarContent,
            };
        }

        public static CalendarGenerationResult CreateFailure(string failureMessage)
        {
            return new CalendarGenerationResult
            {
                IsSuccessful = false,
                Message = failureMessage,
            };
        }
    }

    private void InitializeDaySelection()
    {
        SelectedDays.Clear();
        for (int dayIndex = 0; dayIndex < DAY_COUNT; dayIndex++)
        {
            SelectedDays.Add(new DaySelectionItem(dayIndex, DEFAULT_DAY_NAMES[dayIndex], DEFAULT_DAY_SELECTIONS[dayIndex]));
        }
    }

    private void SetAvailableWorkouts(IEnumerable<WorkoutTemplate> workouts)
    {
        AvailableWorkouts.Clear();
        foreach (WorkoutTemplate workout in workouts)
        {
            AvailableWorkouts.Add(workout);
        }

        if (AvailableWorkouts.Count > 0)
        {
            SelectedWorkout = AvailableWorkouts[0];
        }
    }

    public async Task LoadAvailableWorkoutsAsync()
    {
        try
        {
            IsLoading = true;
            int clientId = this.userSession.CurrentClientId;
            IReadOnlyList<WorkoutTemplate> workouts = await this.calendarWorkoutCatalogService.GetAvailableWorkoutsAsync(clientId, TimeSpan.FromMilliseconds(WORKOUT_LOAD_TIMEOUT_MILLISECONDS));
            SetAvailableWorkouts(workouts);
        }
        catch
        {
            int clientId = this.userSession.CurrentClientId;
            SetAvailableWorkouts(this.calendarWorkoutCatalogService.GetFallbackWorkouts(clientId));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task EnsureWorkoutsLoadedAsync()
    {
        if (IsLoading || AvailableWorkouts.Count > 0)
        {
            return;
        }

        await LoadAvailableWorkoutsAsync();
    }

    public int[] GetSelectedDaysOfWeek()
    {
        List<int> selectedDayIndexes = new List<int>();
        for (int dayIndex = 0; dayIndex < SelectedDays.Count; dayIndex++)
        {
            DaySelectionItem selectedDayItem = SelectedDays[dayIndex];
            if (selectedDayItem.IsSelected)
            {
                selectedDayIndexes.Add(selectedDayItem.DayOfWeekIndex);
            }
        }

        return selectedDayIndexes.ToArray();
    }

    public string? ValidateInput()
    {
        if (SelectedWorkout == null)
        {
            return "Please select a workout from the dropdown.";
        }

        if (DurationWeeks < MINIMUM_DURATION_WEEKS || DurationWeeks > MAXIMUM_DURATION_WEEKS)
        {
            return "Duration must be between 1 and 52 weeks.";
        }

        int[] selectedDaysArray = GetSelectedDaysOfWeek();
        if (selectedDaysArray.Length == 0)
        {
            return "Please select at least one training day.";
        }

        return null;
    }

    public Task<string> GenerateCalendarAsync()
    {
        string? validationErrorMessage = ValidateInput();
        if (validationErrorMessage != null)
        {
            throw new InvalidOperationException(validationErrorMessage);
        }

        if (SelectedWorkout == null)
        {
            throw new InvalidOperationException("No workout selected.");
        }

        int[] selectedDayIndexes = GetSelectedDaysOfWeek();
        string generatedCalendarContent = this.calendarExportService.GenerateCalendar(SelectedWorkout, DurationWeeks, selectedDayIndexes);
        GeneratedIcsContent = generatedCalendarContent;
        return Task.FromResult(generatedCalendarContent);
    }

    public async Task<CalendarGenerationResult> GenerateCalendarForExportAsync()
    {
        try
        {
            string generatedCalendarContent = await GenerateCalendarAsync();
            if (string.IsNullOrEmpty(generatedCalendarContent))
            {
                return CalendarGenerationResult.CreateFailure("Failed to generate calendar file. Please try again.");
            }

            return CalendarGenerationResult.CreateSuccess(generatedCalendarContent);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            return CalendarGenerationResult.CreateFailure(invalidOperationException.Message);
        }
    }

    public Task<string?> SaveGeneratedCalendarToDownloadsFallbackAsync()
    {
        string selectedWorkoutName = SelectedWorkout?.Name ?? "Workout";
        return this.calendarExportService.SaveCalendarToDownloadsAsync(GeneratedIcsContent, selectedWorkoutName);
    }

    public void SetErrorStatus(string errorMessage)
    {
        StatusSeverity = InfoBarSeverity.Error;
        StatusTitle = "Error";
        StatusMessage = errorMessage;
        IsStatusOpen = true;
    }

    public void SetSuccessStatus(string successMessage)
    {
        StatusSeverity = InfoBarSeverity.Success;
        StatusTitle = "Success";
        StatusMessage = successMessage;
        IsStatusOpen = true;
    }

    public void ClearStatus()
    {
        StatusTitle = string.Empty;
        StatusMessage = string.Empty;
        IsStatusOpen = false;
    }
}
