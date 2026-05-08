using ClassLibrary.Models;

namespace WebAPI.Services.Interfaces
{
    public interface IReminderService
    {
        Task<List<Reminder>> GetUserRemindersAsync(int userId);
        Task<Reminder?> GetReminderByIdAsync(int id);
        Task<Reminder?> GetNextReminderAsync(int userId);
        Task<bool> SaveReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(int id);
    }
}