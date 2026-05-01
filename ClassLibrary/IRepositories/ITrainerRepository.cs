using ClassLibrary.Models;

namespace ClassLibrary.IRepositories
{
    public interface ITrainerRepository
    {
        Task<List<Client>> GetTrainerClientsAsync(int trainerId);
        Task SaveTrainerWorkoutAsync(WorkoutTemplate template);
        Task DeleteWorkoutTemplateAsync(int templateId);
    }
}
