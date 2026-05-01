using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext databaseContext;

        public TrainerRepository(AppDbContext context)
        {
            databaseContext = context;
        }

        
        public async Task<List<Client>> GetTrainerClientsAsync(int trainerId)
        {
            return await databaseContext.Clients
                .Include(client => client.WorkoutLogs)
                .ToListAsync();
        }

        public async Task SaveTrainerWorkoutAsync(WorkoutTemplate template)
        {
            if (template.WorkoutTemplateId == 0)
            {
                await databaseContext.Set<WorkoutTemplate>().AddAsync(template);
            }
            else
            {
                databaseContext.Set<WorkoutTemplate>().Update(template);
            }

            await databaseContext.SaveChangesAsync();
        }

        public async Task DeleteWorkoutTemplateAsync(int templateId)
        {
            var template = await databaseContext.Set<WorkoutTemplate>()
                .FirstOrDefaultAsync(template => template.WorkoutTemplateId == templateId);

            if (template == null)
            {
                throw new KeyNotFoundException($"Workout template with ID {templateId} not found.");
            }

            databaseContext.Remove(template);
            await databaseContext.SaveChangesAsync();
        }
    }
}
