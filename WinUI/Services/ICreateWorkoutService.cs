using ClassLibrary.Models;

namespace WinUI.Services;

public interface ICreateWorkoutService
{
    Task<IReadOnlyList<string>> GetAllExerciseNamesAsync();

    Task SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate);
}
