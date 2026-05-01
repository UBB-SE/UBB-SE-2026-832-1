using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories; 
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class RepositoryTrainer : IRepositoryTrainer
{
    private readonly AppDbContext dbContext;

    public RepositoryTrainer(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<Client>> GetTrainerClientsAsync(int trainerId)
    {
        return await dbContext.Clients
            .Include(client => client.WorkoutLogs)
            .ToListAsync();
    }

    public async Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate)
    {
        if (workoutTemplate.WorkoutTemplateId == 0)
        {
            await dbContext.Set<WorkoutTemplate>().AddAsync(workoutTemplate);
        }
        else
        {
            dbContext.Set<WorkoutTemplate>().Update(workoutTemplate);
        }

        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int workoutTemplateId)
    {
        var workoutTemplate = await dbContext.Set<WorkoutTemplate>()
            .FirstOrDefaultAsync(template => template.WorkoutTemplateId == workoutTemplateId);

        if (workoutTemplate == null)
            return false;

        dbContext.Remove(workoutTemplate);
        return await dbContext.SaveChangesAsync() > 0;
    }
}