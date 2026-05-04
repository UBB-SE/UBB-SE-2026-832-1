using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IReminderService
{
    Task<IReadOnlyList<ReminderDataTransferObject>> GetUserRemindersAsync(int userId);
    Task<ReminderDataTransferObject?> GetReminderByIdAsync(int id);
    Task<ReminderDataTransferObject?> GetNextReminderAsync(int userId);
    Task<bool> SaveReminderAsync(SaveReminderRequestDataTransferObject request);
    Task DeleteReminderAsync(int id);
}