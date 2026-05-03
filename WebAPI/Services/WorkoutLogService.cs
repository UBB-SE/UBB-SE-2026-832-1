using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class WorkoutLogService : IWorkoutLogService
{
    private readonly IWorkoutLogRepository workoutLogRepository;

    public WorkoutLogService(IWorkoutLogRepository workoutLogRepository)
    {
        this.workoutLogRepository = workoutLogRepository;
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId);
        return logs.Select(MapToDto).ToList();
    }

    public async Task SaveWorkoutLogAsync(WorkoutLogDataTransferObject workoutLog)
    {
        var log = MapToModel(workoutLog);
        await this.workoutLogRepository.SaveWorkoutLogAsync(log);
    }

    public async Task UpdateWorkoutLogAsync(WorkoutLogDataTransferObject workoutLog)
    {
        var log = MapToModel(workoutLog);
        await this.workoutLogRepository.UpdateWorkoutLogAsync(log);
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        return await this.workoutLogRepository.GetClientWeightAsync(clientId);
    }

    private static WorkoutLogDataTransferObject MapToDto(WorkoutLog log)
    {
        return new WorkoutLogDataTransferObject
        {
            WorkoutLogId = log.WorkoutLogId,
            Client = new ClientDataTransferObject
            {
                ClientId = log.Client?.ClientId ?? 0,
                Email = log.Client?.Email ?? string.Empty,
                FullName = log.Client?.FullName ?? string.Empty,
                Weight = log.Client?.Weight ?? 0,
                Height = log.Client?.Height ?? 0,
                PrimaryGoal = log.Client?.PrimaryGoal ?? string.Empty,
            },
            WorkoutName = log.WorkoutName,
            Date = log.Date,
            Duration = log.Duration,
            SourceTemplateId = log.SourceTemplateId,
            Type = log.Type.ToString(),
            Exercises = log.Exercises.Select(MapExerciseToDto).ToList(),
            TotalCaloriesBurned = log.TotalCaloriesBurned,
            AverageMetabolicEquivalent = log.AverageMetabolicEquivalent,
            IntensityTag = log.IntensityTag,
            Rating = log.Rating <= 0 ? null : log.Rating,
            TrainerNotes = log.TrainerNotes,
        };
    }

    private static LoggedExerciseDataTransferObject MapExerciseToDto(LoggedExercise exercise)
    {
        return new LoggedExerciseDataTransferObject
        {
            LoggedExerciseId = exercise.LoggedExerciseId,
            ExerciseName = exercise.ExerciseName,
            TargetMuscles = exercise.TargetMuscles.ToString(),
            Sets = exercise.Sets.Select(MapSetToDto).ToList(),
            MetabolicEquivalent = exercise.MetabolicEquivalent,
            ExerciseCaloriesBurned = exercise.ExerciseCaloriesBurned,
            PerformanceRatio = exercise.PerformanceRatio,
            IsSystemAdjusted = exercise.IsSystemAdjusted,
            AdjustmentNote = exercise.AdjustmentNote,
        };
    }

    private static LoggedSetDataTransferObject MapSetToDto(LoggedSet set)
    {
        return new LoggedSetDataTransferObject
        {
            LoggedSetId = set.LoggedSetId,
            ExerciseName = set.ExerciseName,
            SetIndex = set.SetIndex,
            TargetReps = set.TargetReps,
            ActualReps = set.ActualReps,
            TargetWeight = set.TargetWeight,
            ActualWeight = set.ActualWeight,
            SetNumber = set.SetNumber,
        };
    }

    private static WorkoutLog MapToModel(WorkoutLogDataTransferObject dto)
    {
        var workoutType = Enum.TryParse<WorkoutType>(dto.Type, ignoreCase: true, out var parsedType)
            ? parsedType
            : WorkoutType.CUSTOM;

        return new WorkoutLog
        {
            WorkoutLogId = dto.WorkoutLogId,
            Client = new Client { ClientId = dto.Client?.ClientId ?? 0 },
            WorkoutName = dto.WorkoutName,
            Date = dto.Date,
            Duration = dto.Duration,
            SourceTemplateId = dto.SourceTemplateId,
            Type = workoutType,
            TotalCaloriesBurned = dto.TotalCaloriesBurned,
            AverageMetabolicEquivalent = dto.AverageMetabolicEquivalent,
            IntensityTag = dto.IntensityTag,
            Rating = dto.Rating ?? 0,
            TrainerNotes = dto.TrainerNotes,
        };
    }
}
