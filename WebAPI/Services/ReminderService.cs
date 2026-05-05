using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class ReminderService : IReminderService
{
    private readonly IReminderRepository reminderRepository;
    private readonly IUserRepository userRepository;

    public ReminderService(IReminderRepository reminderRepository, IUserRepository userRepository)
    {
        this.reminderRepository = reminderRepository;
        this.userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ReminderDataTransferObject>> GetUserRemindersAsync(int userId)
    {
        var reminders = await this.reminderRepository.GetAllByUserIdAsync(userId);
        return reminders.Select(MapToDto).ToList();
    }

    public async Task<ReminderDataTransferObject?> GetReminderByIdAsync(int id)
    {
        var reminder = await this.reminderRepository.GetByIdAsync(id);
        return reminder == null ? null : MapToDto(reminder);
    }

    public async Task<ReminderDataTransferObject?> GetNextReminderAsync(int userId)
    {
        var reminder = await this.reminderRepository.GetNextReminderAsync(userId);
        return reminder == null ? null : MapToDto(reminder);
    }

    public async Task<bool> SaveReminderAsync(SaveReminderRequestDataTransferObject request)
    {
        if (request == null || request.UserId <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 50)
        {
            return false;
        }

        var user = await this.userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return false;
        }

        var reminder = new Reminder
        {
            Id = request.Id,
            User = user,
            Name = request.Name,
            HasSound = request.HasSound,
            Time = request.Time,
            ReminderDate = request.ReminderDate,
            Frequency = request.Frequency,
        };

        if (reminder.Id == 0)
        {
            await this.reminderRepository.AddAsync(reminder);
        }
        else
        {
            await this.reminderRepository.UpdateAsync(reminder);
        }

        return true;
    }

    public async Task DeleteReminderAsync(int id)
    {
        await this.reminderRepository.DeleteAsync(id);
    }

    private static ReminderDataTransferObject MapToDto(Reminder reminder)
    {
        return new ReminderDataTransferObject
        {
            Id = reminder.Id,
            UserId = reminder.User?.UserId ?? 0,
            Name = reminder.Name,
            HasSound = reminder.HasSound,
            Time = reminder.Time,
            ReminderDate = reminder.ReminderDate,
            Frequency = reminder.Frequency,
        };
    }
}