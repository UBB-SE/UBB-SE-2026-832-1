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

    public async Task<TemplateExercise?> GetTemplateExerciseByIdAsync(int templateExerciseId)
    {
        return await this.databaseContext.Set<TemplateExercise>()
            .AsNoTracking()
            .FirstOrDefaultAsync(templateExercise => templateExercise.TemplateExerciseId == templateExerciseId);
    }

    public async Task<bool> UpdateTemplateExerciseWeightAsync(int templateExerciseId, double newWeight)
    {
        var templateExercise = await this.databaseContext.Set<TemplateExercise>()
            .FirstOrDefaultAsync(exercise => exercise.TemplateExerciseId == templateExerciseId);

        if (templateExercise == null)
        {
            return false;
        }

        templateExercise.TargetWeight = newWeight;
        await this.databaseContext.SaveChangesAsync();
        return true;
    }
}
