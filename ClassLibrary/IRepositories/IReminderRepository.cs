using ClassLibrary.Models;

namespace ClassLibrary.IRepositories
{
    public interface IReminderRepository
    {
        Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId);
        Task<Reminder?> GetByIdAsync(int id);
        Task<Reminder?> GetNextReminderAsync(int userId);
        Task AddAsync(Reminder reminder);
        Task UpdateAsync(Reminder reminder);
        Task DeleteAsync(int id);
    }
}