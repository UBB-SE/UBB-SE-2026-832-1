using ClassLibrary.Models;
using ClassLibrary.IRepositories;

namespace WinUI.Services;

/// <summary>
/// UI service implementation placeholder for ActiveWorkoutViewModel. The
/// implementation should call into repository interfaces (EF Core) to
/// perform operations. Currently methods are unimplemented stubs; they will
/// be implemented later as viewmodels are refined.
///
/// Any non-repository logic from the original source must not be placed
/// here — only UI-related orchestration belongs in this project-specific
/// service. Data access must go through ClassLibrary repositories.
/// </summary>
public sealed class ActiveWorkoutService : IActiveWorkoutService
{
    private readonly IWorkoutLogRepository workoutLogRepository;
    private readonly IWorkoutTemplateRepository workoutTemplateRepository;

    public ActiveWorkoutService(
        IWorkoutLogRepository workoutLogRepository,
        IWorkoutTemplateRepository workoutTemplateRepository)
    {
        this.workoutLogRepository = workoutLogRepository;
        this.workoutTemplateRepository = workoutTemplateRepository;
    }

    public void StartWorkout(WorkoutLog workoutLog)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public void SaveSet(WorkoutLog workoutLog, LoggedSet set)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public bool FinalizeWorkout(WorkoutLog workoutLog)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public IReadOnlyList<WorkoutTemplate> GetAvailableWorkoutsForClient(int clientId)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public IReadOnlyList<WorkoutTemplate> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public WorkoutTemplate? FindWorkoutTemplateById(int clientId, int? id)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }

    public IReadOnlyList<LoggedExercise> GetLastTwoLogsForExercise(int templateExerciseId)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new System.NotImplementedException();
    }
}
