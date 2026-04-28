using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(int id);
    Task<IEnumerable<Reminder>> GetAllAsync();

    Task<IEnumerable<Reminder>> GetAllByUserIdAsync(Guid userId);
    Task<Reminder?> GetNextReminderAsync(Guid userId);

    Task AddAsync(Reminder entity);
    Task UpdateAsync(Reminder entity);
    Task DeleteAsync(int id);
}