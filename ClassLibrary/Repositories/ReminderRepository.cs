using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext dbContext;

    public ReminderRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await dbContext.Reminders.ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId)
    {
        return await dbContext.Reminders
            .Where(reminder => reminder.User.UserId == userId)
            .ToListAsync();
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await dbContext.Reminders
            .FirstOrDefaultAsync(reminder => reminder.ReminderId == id);
    }

    public async Task<Reminder?> GetNextReminderAsync(int userId)
    {
        return await dbContext.Reminders
            .Where(reminder => reminder.User.UserId == userId)
            .OrderBy(reminder => reminder.Time)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Reminder reminder)
    {
        await dbContext.Reminders.AddAsync(reminder);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        dbContext.Reminders.Update(reminder);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await dbContext.Reminders
            .FirstOrDefaultAsync(reminder => reminder.ReminderId == id);

        if (reminder == null)
            return;

        dbContext.Reminders.Remove(reminder);
        await dbContext.SaveChangesAsync();
    }
}