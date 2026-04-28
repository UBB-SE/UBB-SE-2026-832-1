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
    private readonly AppDbContext context;

    public ReminderRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await context.Reminders.FindAsync(id);
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await context.Reminders.ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.Reminders
            .Where(r => r.User.Id == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Reminder entity)
    {
        await context.Reminders.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reminder entity)
    {
        context.Reminders.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await context.Reminders.FindAsync(id);
        if (reminder != null)
        {
            context.Reminders.Remove(reminder);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Reminder?> GetNextReminderAsync(Guid userId)
    {
        var todayString = DateTime.Now.ToString("yyyy-MM-dd");
        var currentTime = DateTime.Now.TimeOfDay;

        return await context.Reminders
            .Where(r => r.User.Id == userId)
            .Where(r => string.Compare(r.ReminderDate, todayString) > 0 ||
                        (r.ReminderDate == todayString && r.Time >= currentTime))
            .OrderBy(r => r.ReminderDate)
            .ThenBy(r => r.Time)
            .FirstOrDefaultAsync();
    }
}