using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WebUI.Models.Workout;

public class WorkoutHistoryViewModel
{
    public List<WorkoutLogDataTransferObject> Logs { get; set; } = new();
}


public class FocusModeViewModel
{
    public List<WorkoutTemplate> AvailableWorkouts { get; set; } = new();

    public WorkoutTemplate? SelectedTemplate { get; set; }

    public DateTime SessionStartUtc { get; set; } = DateTime.UtcNow;
}

public class WorkoutDashboardViewModel
{
    public List<WorkoutTemplate> AvailableWorkouts { get; set; } = new();

    public List<WorkoutTemplate> SavedWorkouts { get; set; } = new();

    public string? SelectedGoal { get; set; }

    public List<string> AvailableExercises { get; set; } = new();
}

public class CompleteSetFormModel
{
    public int WorkoutTemplateId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public int SetIndex { get; set; }

    public int TargetReps { get; set; }

    public int Reps { get; set; }

    public double Weight { get; set; }
}
