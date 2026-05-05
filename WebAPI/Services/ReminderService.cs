using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository reminderRepository;

    public ReminderService(IReminderRepository reminderRepository)
    {
        this.reminderRepository = reminderRepository;
    }

    public async Task<List<Reminder>> GetUserRemindersAsync(int userId)
    {
        var reminders = await this.reminderRepository.GetAllByUserIdAsync(userId);
        return reminders.ToList();
    }

    public async Task<Reminder?> GetReminderByIdAsync(int id)
    {
        return await this.reminderRepository.GetByIdAsync(id);
    }

    public async Task<Reminder?> GetNextReminderAsync(int userId)
    {
        return await this.reminderRepository.GetNextReminderAsync(userId);
    }

    public async Task<bool> SaveReminderAsync(Reminder reminder)
    {
        if (string.IsNullOrWhiteSpace(reminder.Name) || reminder.Name.Length > 50)
            return false;

        if (reminder.Id == 0)
            await this.reminderRepository.AddAsync(reminder);
        else
            await this.reminderRepository.UpdateAsync(reminder);

        return true;
    }

    public async Task DeleteReminderAsync(int id)
    {
        await this.reminderRepository.DeleteAsync(id);
    }
}