using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface ICreateWorkoutProxy
{
    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync();

    Task SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate);
}



