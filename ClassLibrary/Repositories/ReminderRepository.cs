using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

namespace ClassLibrary.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _dbContext;

    public ReminderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await _dbContext.Reminders
            .Include(r => r.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId)
    {
        return await _dbContext.Reminders
            .Include(r => r.User)
            .Where(reminder => reminder.User.UserId == userId)
            .ToListAsync();
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await _dbContext.Reminders
            .Include(r => r.User)
            .FirstOrDefaultAsync(reminder => reminder.ReminderId == id);
    }

    public async Task<Reminder?> GetNextReminderAsync(int userId)
    {
        var reminders = await _dbContext.Reminders
            .Include(r => r.User)
            .Where(r => r.User.UserId == userId)
            .ToListAsync();

        return reminders
            .OrderBy(r => r.Time)
            .FirstOrDefault();
    }

    public async Task AddAsync(Reminder reminder)
    {
        await _dbContext.Reminders.AddAsync(reminder);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        _dbContext.Reminders.Update(reminder);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await _dbContext.Reminders
            .FirstOrDefaultAsync(reminder => reminder.ReminderId == id);

        if (reminder != null)
        {
            _dbContext.Reminders.Remove(reminder);
            await _dbContext.SaveChangesAsync();
        }
    }
}