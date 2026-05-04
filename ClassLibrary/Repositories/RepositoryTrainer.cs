using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public class RepositoryTrainer : IRepositoryTrainer
{
    private readonly AppDbContext databaseContext;

    public RepositoryTrainer(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<List<Client>> GetTrainerClientsAsync(int trainerId)
    {
        return await databaseContext.Clients
            .Include(client => client.WorkoutLogs)
            .ToListAsync();
    }

    public async Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplate workoutTemplate)
    {
        if (workoutTemplate.WorkoutTemplateId == 0)
            await databaseContext.Set<WorkoutTemplate>().AddAsync(workoutTemplate);
        else
            databaseContext.Set<WorkoutTemplate>().Update(workoutTemplate);

        return await databaseContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int workoutTemplateId)
    {
        var workoutTemplate = await databaseContext.Set<WorkoutTemplate>()
            .FirstOrDefaultAsync(template => template.WorkoutTemplateId == workoutTemplateId);

        if (workoutTemplate == null) return false;

        databaseContext.Remove(workoutTemplate);
        return await databaseContext.SaveChangesAsync() > 0;
    }
}