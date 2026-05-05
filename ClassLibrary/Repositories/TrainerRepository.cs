using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    public sealed class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext databaseContext;

        public TrainerRepository(AppDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<Client>> GetTrainerClientsAsync(int trainerId)
        {
            return await this.databaseContext.Clients
                .Include(client => client.WorkoutLogs)
                .ToListAsync();
        }

        public async Task SaveTrainerWorkoutAsync(WorkoutTemplate template)
        {
            if (template.WorkoutTemplateId == 0)
            {
                await this.databaseContext.Set<WorkoutTemplate>().AddAsync(template);
            }
            else
            {
                this.databaseContext.Set<WorkoutTemplate>().Update(template);
            }

            await this.databaseContext.SaveChangesAsync();
        }

        public async Task DeleteWorkoutTemplateAsync(int templateId)
        {
            var template = await this.databaseContext.Set<WorkoutTemplate>()
                .FirstOrDefaultAsync(template => template.WorkoutTemplateId == templateId);

            if (template == null)
            {
                throw new KeyNotFoundException($"Workout template with ID {templateId} not found.");
            }

            this.databaseContext.Remove(template);
            await this.databaseContext.SaveChangesAsync();
        }
    }
}
