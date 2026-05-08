using System.Collections.ObjectModel;
using System.ComponentModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed class CreateWorkoutViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? WorkoutSaved;

    private readonly ICreateWorkoutService createWorkoutService;

    private string workoutName = string.Empty;
    private string? selectedNewExercise;
    private double newExerciseSets = 3;
    private double newExerciseReps = 10;

    public int ClientId { get; set; }

    public ObservableCollection<TemplateExercise> Exercises { get; } = new();
    public ObservableCollection<string> AvailableExercises { get; } = new();

    public string WorkoutName
    {
        get => this.workoutName;
        set
        {
            this.workoutName = value;
            OnPropertyChanged(nameof(WorkoutName));
        }
    }

    public string? SelectedNewExercise
    {
        get => this.selectedNewExercise;
        set
        {
            this.selectedNewExercise = value;
            OnPropertyChanged(nameof(SelectedNewExercise));
        }
    }

    public double NewExerciseSets
    {
        get => this.newExerciseSets;
        set
        {
            this.newExerciseSets = value;
            OnPropertyChanged(nameof(NewExerciseSets));
        }
    }

    public double NewExerciseReps
    {
        get => this.newExerciseReps;
        set
        {
            this.newExerciseReps = value;
            OnPropertyChanged(nameof(NewExerciseReps));
        }
    }

    public IRelayCommand AddExerciseCommand { get; }
    public IRelayCommand<TemplateExercise> RemoveExerciseCommand { get; }
    public IRelayCommand SaveWorkoutCommand { get; }

    public CreateWorkoutViewModel(ICreateWorkoutService createWorkoutService)
    {
        this.createWorkoutService = createWorkoutService;
        AddExerciseCommand = new RelayCommand(AddExercise);
        RemoveExerciseCommand = new RelayCommand<TemplateExercise>(RemoveExercise);
        SaveWorkoutCommand = new RelayCommand(SaveWorkout);
        _ = LoadAvailableExercisesAsync();
    }

    private void AddExercise()
    {
        if (string.IsNullOrWhiteSpace(this.SelectedNewExercise))
        {
            return;
        }

        this.Exercises.Add(new TemplateExercise
        {
            Name = this.SelectedNewExercise,
            TargetSets = (int)this.NewExerciseSets,
            TargetReps = (int)this.NewExerciseReps,
            MuscleGroup = MuscleGroup.OTHER,
            WorkoutTemplate = null!,
        });

        this.SelectedNewExercise = null;
    }

    private void RemoveExercise(TemplateExercise? exercise)
    {
        if (exercise is null)
        {
            return;
        }

        this.Exercises.Remove(exercise);
    }

    private void SaveWorkout()
    {
        if (string.IsNullOrWhiteSpace(this.WorkoutName) || this.Exercises.Count == 0)
        {
            return;
        }

        var newWorkout = new WorkoutTemplate
        {
            Name = this.WorkoutName,
            Type = WorkoutType.CUSTOM,
            Client = new Client { ClientId = this.ClientId },
            Exercises = this.Exercises.ToList(),
        };

        _ = SaveWorkoutAsync(newWorkout);
    }

    private async Task SaveWorkoutAsync(WorkoutTemplate template)
    {
        await this.createWorkoutService.SaveTrainerWorkoutAsync(template);
        WorkoutSaved?.Invoke();
    }

    private async Task LoadAvailableExercisesAsync()
    {
        var names = await this.createWorkoutService.GetAllExerciseNamesAsync();
        this.AvailableExercises.Clear();
        foreach (var name in names)
        {
            this.AvailableExercises.Add(name);
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
