using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    public class RepositoryTrainer : IRepositoryTrainer
    {
        private readonly AppDbContext _context;

        public RepositoryTrainer(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<Client>> GetTrainerClientsAsync(int trainerId)
        {
            return await _context.Clients
                .Include(c => c.WorkoutLogs)
                .ToListAsync();
        }

        public async Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate template)
        {
            try
            {
                if (template.WorkoutTemplateId == 0)
                {
                    await _context.Set<WorkoutTemplate>().AddAsync(template);
                }
                else
                {
                    _context.Set<WorkoutTemplate>().Update(template);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteWorkoutTemplateAsync(int templateId)
        {
            var template = await _context.Set<WorkoutTemplate>()
                .FirstOrDefaultAsync(t => t.WorkoutTemplateId == templateId);

            if (template == null)
                return false;

            _context.Remove(template);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}