using ClassLibrary.Models;

namespace ClassLibrary.Repositories.Interfaces
{
    public interface IRepositoryTrainer
    {
        Task<List<Client>> GetTrainerClientsAsync(int trainerId);
        Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate template);
        Task<bool> DeleteWorkoutTemplateAsync(int templateId);
    }
}