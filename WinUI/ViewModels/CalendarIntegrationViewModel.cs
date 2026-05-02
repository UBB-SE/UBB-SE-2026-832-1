using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ClassLibrary.Models;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed class DaySelectionItem : INotifyPropertyChanged
{
    private bool isSelected;

    public DaySelectionItem(int dayOfWeekIndex, string dayName, bool initialSelection = false)
    {
        DayOfWeekIndex = dayOfWeekIndex;
        DayName = dayName;
        this.isSelected = initialSelection;
    }

    public int DayOfWeekIndex { get; }

    public string DayName { get; }

    public bool IsSelected
    {
        get => this.isSelected;
        set
        {
            if (this.isSelected == value)
            {
                return;
            }

            this.isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class CalendarIntegrationViewModel : INotifyPropertyChanged
{
    private readonly ICalendarIntegrationService calendarIntegrationService;
    private readonly IUserSession userSession;
    private ObservableCollection<WorkoutTemplate> availableWorkouts = new();
    private WorkoutTemplate? selectedWorkout;
    private int durationWeeks = 4;
    private ObservableCollection<DaySelectionItem> selectedDays = new();
    private bool isLoading;
    private string generatedIcsContent = string.Empty;

    public CalendarIntegrationViewModel(ICalendarIntegrationService calendarIntegrationService, IUserSession userSession)
    {
        this.calendarIntegrationService = calendarIntegrationService ?? throw new ArgumentNullException(nameof(calendarIntegrationService));
        this.userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        InitializeDaySelection();
    }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadAvailableWorkoutsAsync()
    {
        try
        {
            IsLoading = true;

            var clientId = this.userSession.CurrentClientId;
            var workouts = await this.calendarIntegrationService.GetAvailableWorkoutsAsync(clientId).ConfigureAwait(true);

            this.availableWorkouts.Clear();
            foreach (var workout in workouts)
            {
                this.availableWorkouts.Add(workout);
            }

            if (this.selectedWorkout == null && this.availableWorkouts.Count > 0)
            {
                this.SelectedWorkout = this.availableWorkouts[0];
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task EnsureWorkoutsLoadedAsync()
    {
        if (this.isLoading || this.availableWorkouts.Count > 0)
        {
            return;
        }

        await LoadAvailableWorkoutsAsync().ConfigureAwait(true);
    }

    public int[] GetSelectedDaysOfWeek()
    {
        return this.selectedDays
            .Where(d => d.IsSelected)
            .Select(d => d.DayOfWeekIndex)
            .ToArray();
    }

    public string? ValidateInput()
    {
        if (this.selectedWorkout == null)
        {
            return "Please select a workout from the dropdown.";
        }

        if (this.durationWeeks < 1 || this.durationWeeks > 52)
        {
            return "Duration must be between 1 and 52 weeks.";
        }

        var selectedDaysArray = GetSelectedDaysOfWeek();
        if (selectedDaysArray.Length == 0)
        {
            return "Please select at least one training day.";
        }

        return null;
    }

    public async Task<string> GenerateCalendarAsync()
    {
        var validationError = ValidateInput();
        if (validationError != null)
        {
            throw new InvalidOperationException(validationError);
        }

        if (this.selectedWorkout == null)
        {
            throw new InvalidOperationException("No workout selected.");
        }

        var selectedDaysArray = GetSelectedDaysOfWeek();
        var icsContent = await this.calendarIntegrationService
            .GenerateCalendarIcsAsync(this.selectedWorkout, this.durationWeeks, selectedDaysArray)
            .ConfigureAwait(true);

        GeneratedIcsContent = icsContent;
        return icsContent;
    }

    public void ToggleDaySelection(int dayOfWeek)
    {
        var dayItem = this.selectedDays.FirstOrDefault(d => d.DayOfWeekIndex == dayOfWeek);
        if (dayItem != null)
        {
            dayItem.IsSelected = !dayItem.IsSelected;
        }
    }

    private void InitializeDaySelection()
    {
        var dayNames = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        var defaultSelections = new[] { false, true, true, true, true, true, false };

        this.selectedDays.Clear();
        for (var i = 0; i < 7; i++)
        {
            this.selectedDays.Add(new DaySelectionItem(i, dayNames[i], defaultSelections[i]));
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        this.OnPropertyChanged(propertyName);
    }
}
