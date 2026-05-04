using ClassLibrary.Models;

namespace WinUI.Services;

/// <summary>
/// UI service surface for ActiveWorkoutViewModel. Methods are intentionally
/// defined but not implemented — viewmodels will call into this service and
/// the implementations will be filled out as the UI and viewmodels are
/// refined.
///
/// Note: This is a UI layer service. Any repository logic must remain in
/// the ClassLibrary repositories (EF Core); this interface does not contain
/// direct data access responsibilities.
/// </summary>
public interface IActiveWorkoutService
{
    void StartWorkout(WorkoutLog workoutLog);
    void SaveSet(WorkoutLog workoutLog, LoggedSet set);
    bool FinalizeWorkout(WorkoutLog workoutLog);
    IReadOnlyList<WorkoutTemplate> GetAvailableWorkoutsForClient(int clientId);
    IReadOnlyList<WorkoutTemplate> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId);
    WorkoutTemplate? FindWorkoutTemplateById(int clientId, int? id);
    IReadOnlyList<LoggedExercise> GetLastTwoLogsForExercise(int templateExerciseId);
}
