using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface ITrainerRepository
{
    Task<IReadOnlyList<User>> GetTrainerClientsAsync(int trainerId, CancellationToken cancellationToken = default);
    Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate template, CancellationToken cancellationToken = default);
    Task<bool> DeleteWorkoutTemplateAsync(int templateId, CancellationToken cancellationToken = default);
}
