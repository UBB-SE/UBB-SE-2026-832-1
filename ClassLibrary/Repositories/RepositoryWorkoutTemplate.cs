using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class RepositoryWorkoutTemplate : IRepositoryWorkoutTemplate
{
    private readonly AppDbContext databaseContext;

    public RepositoryWorkoutTemplate(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.WorkoutTemplates
            .AsNoTracking()
            .Where(workoutTemplate => workoutTemplate.Client.ClientId == clientId)
            .Include(workoutTemplate => workoutTemplate.Client)
            .OrderBy(workoutTemplate => workoutTemplate.WorkoutTemplateId)
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkoutTemplate?> GetByIdAsync(int workoutTemplateId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.WorkoutTemplates
            .AsNoTracking()
            .Include(workoutTemplate => workoutTemplate.Client)
            .Include(workoutTemplate => workoutTemplate.Exercises)
            .FirstOrDefaultAsync(workoutTemplate => workoutTemplate.WorkoutTemplateId == workoutTemplateId, cancellationToken);
    }
}
