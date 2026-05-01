using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(int id);

    Task<IEnumerable<Reminder>> GetAllAsync(); 

    Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId);
    Task<Reminder?> GetNextReminderAsync(int userId);

    Task AddAsync(Reminder entity);
    Task UpdateAsync(Reminder entity);
    Task DeleteAsync(int id);
}