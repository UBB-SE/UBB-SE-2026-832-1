using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class ReminderRepository(AppDbContext dbContext) : IReminderRepository
{
    public async Task<Reminder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reminders
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Reminder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Reminders
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Reminder>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reminders
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Reminder entity, CancellationToken cancellationToken = default)
    {
        dbContext.Reminders.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Reminder entity, CancellationToken cancellationToken = default)
    {
        dbContext.Reminders.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Reminders.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.Reminders.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Reminder?> GetNextReminderAsync(int userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var today = now.Date.ToString("yyyy-MM-dd");
        var currentTime = now.TimeOfDay;

        var reminders = await dbContext.Reminders
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);

        return reminders
            .Where(r => string.Compare(r.ReminderDate, today, StringComparison.Ordinal) > 0
                     || (r.ReminderDate == today && r.Time >= currentTime))
            .OrderBy(r => r.ReminderDate)
            .ThenBy(r => r.Time)
            .FirstOrDefault();
    }
}
