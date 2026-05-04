using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext databaseContext;

    public ReminderRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await databaseContext.Reminders.ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId)
    {
        return await databaseContext.Reminders
            .Where(reminder => reminder.UserId == userId)
            .ToListAsync();
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await databaseContext.Reminders
            .FirstOrDefaultAsync(reminder => reminder.ReminderId == id);
    }

    public async Task<Reminder?> GetNextReminderAsync(int userId)
    {
        return await databaseContext.Reminders
            .Where(reminder => reminder.UserId == userId)
            .OrderBy(reminder => reminder.Time)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Reminder reminder)
    {
        await databaseContext.Reminders.AddAsync(reminder);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        databaseContext.Reminders.Update(reminder);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await databaseContext.Reminders.FindAsync(id);
        if (reminder == null) return;

        databaseContext.Reminders.Remove(reminder);
        await databaseContext.SaveChangesAsync();
    }
}