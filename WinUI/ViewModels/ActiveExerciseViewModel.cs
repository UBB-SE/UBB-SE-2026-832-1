using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public sealed partial class ActiveExerciseViewModel : ObservableObject
{
    public string ExerciseName { get; }

    public MuscleGroup MuscleGroup { get; }

    public ObservableCollection<ActiveSetViewModel> Sets { get; } = new();

    [ObservableProperty]
    private double? previousBestWeight;

    [ObservableProperty]
    private bool isSystemAdjusted;

    [ObservableProperty]
    private string adjustmentNote = string.Empty;

    public ActiveExerciseViewModel(TemplateExercise template, Action<ActiveSetViewModel> autoSaveSet)
    {
        this.ExerciseName = template.Name;
        this.MuscleGroup = template.MuscleGroup;

        for (int i = 0; i < template.TargetSets; i++)
        {
            this.Sets.Add(new ActiveSetViewModel
            {
                ExerciseName = template.Name,
                SetIndex = i + 1,
                TargetReps = template.TargetReps,
                TargetWeight = template.TargetWeight > 0 ? template.TargetWeight : null,
                IsFocused = i == 0,
                AutoSaveHandler = autoSaveSet,
            });
        }
    }
}
