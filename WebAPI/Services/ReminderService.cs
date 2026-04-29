using ClassLibrary.Models;
using ClassLibrary.IRepositories;   
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;

        public ReminderService(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        public async Task<List<Reminder>> GetUserRemindersAsync(int userId)
        {
            var reminders = await _reminderRepository.GetAllByUserIdAsync(userId);
            return reminders.ToList();
        }

        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            return await _reminderRepository.GetByIdAsync(id);
        }

        public async Task<Reminder?> GetNextReminderAsync(int userId)
        {
            return await _reminderRepository.GetNextReminderAsync(userId);
        }

        public async Task<bool> SaveReminderAsync(Reminder reminder)
        {
            if (string.IsNullOrWhiteSpace(reminder.Name) || reminder.Name.Length > 50)
                return false;

            if (reminder.Id == 0)
                await _reminderRepository.AddAsync(reminder);
            else
                await _reminderRepository.UpdateAsync(reminder);

            return true;
        }

        public async Task DeleteReminderAsync(int id)
        {
            await _reminderRepository.DeleteAsync(id);
        }
    }
}