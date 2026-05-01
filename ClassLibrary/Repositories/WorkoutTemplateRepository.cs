using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class WorkoutTemplateRepository : IWorkoutTemplateRepository
{
    private readonly AppDbContext databaseContext;

    public WorkoutTemplateRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId)
    {
        return await this.databaseContext.WorkoutTemplates
            .AsNoTracking()
            .Where(workoutTemplate => workoutTemplate.Client.ClientId == clientId)
            .Include(workoutTemplate => workoutTemplate.Client)
            .OrderBy(workoutTemplate => workoutTemplate.WorkoutTemplateId)
            .ToListAsync();
    }

    public async Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId)
    {
        return await this.databaseContext.WorkoutTemplates
            .AsNoTracking()
            .Include(workoutTemplate => workoutTemplate.Client)
            .Include(workoutTemplate => workoutTemplate.Exercises)
            .FirstOrDefaultAsync(workoutTemplate => workoutTemplate.WorkoutTemplateId == workoutTemplateId);
    }
}
