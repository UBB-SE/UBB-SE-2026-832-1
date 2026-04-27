using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IReminderRepository
{
    Task AddAsync(Reminder entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Reminder>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Reminder>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Reminder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Reminder?> GetNextReminderAsync(int userId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Reminder entity, CancellationToken cancellationToken = default);
}
