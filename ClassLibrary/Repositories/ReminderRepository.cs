using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;

        public ReminderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reminder>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Set<Reminder>()
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<Reminder?> GetByIdAsync(int id)
        {
            return await _context.Set<Reminder>()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reminder?> GetNextReminderAsync(int userId)
        {
            return await _context.Set<Reminder>()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Time)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Reminder reminder)
        {
            await _context.Set<Reminder>().AddAsync(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reminder reminder)
        {
            _context.Set<Reminder>().Update(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reminder = await _context.Set<Reminder>().FindAsync(id);
            if (reminder != null)
            {
                _context.Remove(reminder);
                await _context.SaveChangesAsync();
            }
        }
    }
}