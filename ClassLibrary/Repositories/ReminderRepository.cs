using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext databaseContext;

    public ReminderRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await databaseContext.Reminders.FindAsync(id);
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await databaseContext.Reminders.ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(Guid userId)
    {
        return await databaseContext.Reminders
            .Where(reminder => reminder.User.Id == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Reminder entity)
    {
        await databaseContext.Reminders.AddAsync(entity);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reminder entity)
    {
        databaseContext.Reminders.Update(entity);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await databaseContext.Reminders.FindAsync(id);
        if (reminder != null)
        {
            databaseContext.Reminders.Remove(reminder);
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task<Reminder?> GetNextReminderAsync(Guid userId)
    {
        var todayString = DateTime.Now.ToString("yyyy-MM-dd");
        var currentTime = DateTime.Now.TimeOfDay;

        return await databaseContext.Reminders
            .Where(reminder => reminder.User.Id == userId)
            .Where(reminder => string.Compare(reminder.ReminderDate, todayString) > 0 ||
                               (reminder.ReminderDate == todayString && reminder.Time >= currentTime))
            .OrderBy(reminder => reminder.ReminderDate)
            .ThenBy(reminder => reminder.Time)
            .FirstOrDefaultAsync();
    }
}