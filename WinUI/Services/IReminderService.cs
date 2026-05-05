using ClassLibrary.DTOs;

namespace WinUI.Services
{
    public interface IReminderService
    {
        Task DeleteReminderAsync(int id);
        Task<ReminderDto?> GetNextReminderAsync(int userId);
        Task<ReminderDto?> GetReminderByIdAsync(int id);
        Task<IReadOnlyList<ReminderDto>> GetUserRemindersAsync(int userId);
        Task<bool> SaveReminderAsync(ReminderDto reminder);
    }
}