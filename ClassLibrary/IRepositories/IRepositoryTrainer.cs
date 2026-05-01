using ClassLibrary.Models;

namespace ClassLibrary.IRepositories
{
    public interface IRepositoryTrainer
    {
        Task<List<Client>> GetTrainerClientsAsync(int trainerId);
        Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate);
        Task<bool> DeleteWorkoutTemplateAsync(int workoutTemplateId);
    }
}