using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutTemplateRepository
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId);

    Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId);

    Task<TemplateExercise?> GetTemplateExerciseByIdAsync(int templateExerciseId);

    Task<bool> UpdateTemplateExerciseWeightAsync(int templateExerciseId, double newWeight);
}
