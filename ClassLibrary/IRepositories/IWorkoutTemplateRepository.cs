using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IWorkoutTemplateRepository
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAllTemplatesAsync();

    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId);

    Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId);
}
