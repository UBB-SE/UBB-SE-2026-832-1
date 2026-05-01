using ClassLibrary.Models;

namespace ClassLibrary.Repositories.Interfaces
{
    public interface ITrainerRepository
    {
        Task<List<Client>> GetTrainerClientsAsync(int trainerId);
        Task SaveTrainerWorkoutAsync(WorkoutTemplate template);
        Task DeleteWorkoutTemplateAsync(int templateId);
    }
}
