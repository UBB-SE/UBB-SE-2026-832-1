using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class ProgressionService : IProgressionService
{
    private const double PLATEAU_THRESHOLD = 0.9;
    private const int CONSECUTIVE_FAILED_SETS_FOR_PLATEAU = 2;
    private const double LEGS_WEIGHT_INCREMENT = 5.0;
    private const double DEFAULT_WEIGHT_INCREMENT = 2.5;
    private const double DELOAD_FACTOR = 0.90;
    private const double DELOAD_ROUNDING_STEP = 0.5;
    private const double OVERLOAD_PERFORMANCE_THRESHOLD = 1.0;

    private readonly IWorkoutTemplateRepository workoutTemplateRepository;
    private readonly INotificationRepository notificationRepository;

    public ProgressionService(
        IWorkoutTemplateRepository workoutTemplateRepository,
        INotificationRepository notificationRepository)
    {
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.notificationRepository = notificationRepository;
    }

    public async Task EvaluateWorkoutAsync(EvaluateWorkoutRequestDataTransferObject request)
    {
        var exercises = request.Exercises.Select(exerciseDto => new LoggedExercise
        {
            LoggedExerciseId = exerciseDto.LoggedExerciseId,
            ExerciseName = exerciseDto.ExerciseName,
            ParentTemplateExerciseId = exerciseDto.ParentTemplateExerciseId,
            TargetMuscles = Enum.TryParse<MuscleGroup>(exerciseDto.TargetMuscles, ignoreCase: true, out var muscleGroup)
                ? muscleGroup
                : MuscleGroup.OTHER,
            Sets = exerciseDto.Sets.Select(setDto => new LoggedSet
            {
                LoggedSetId = setDto.LoggedSetId,
                ExerciseName = setDto.ExerciseName,
                SetIndex = setDto.SetIndex,
                TargetReps = setDto.TargetReps,
                ActualReps = setDto.ActualReps,
                TargetWeight = setDto.TargetWeight,
                ActualWeight = setDto.ActualWeight,
                SetNumber = setDto.SetNumber,
            }).ToList(),
            MetabolicEquivalent = exerciseDto.MetabolicEquivalent,
            ExerciseCaloriesBurned = exerciseDto.ExerciseCaloriesBurned,
            PerformanceRatio = exerciseDto.PerformanceRatio,
            IsSystemAdjusted = exerciseDto.IsSystemAdjusted,
            AdjustmentNote = exerciseDto.AdjustmentNote,
        }).ToList();

        foreach (var exercise in exercises)
        {
            if (exercise.Sets.Count == 0)
            {
                continue;
            }

            var templateExercise = await this.workoutTemplateRepository.GetTemplateExerciseByIdAsync(exercise.ParentTemplateExerciseId);
            if (templateExercise == null)
            {
                continue;
            }

            // Compute performance ratio for each set (Requirements 2.1, 2.2)
            var ratios = exercise.Sets.Select(set =>
                templateExercise.TargetReps > 0
                    ? (double)(set.ActualReps ?? 0) / templateExercise.TargetReps
                    : 0.0).ToList();

            // Plateau detection: count consecutive sets below threshold (Requirements 2.3, 2.4)
            bool plateauDetected = false;
            int consecutiveFailedSets = 0;
            foreach (var ratio in ratios)
            {
                if (ratio < PLATEAU_THRESHOLD)
                {
                    consecutiveFailedSets++;
                    if (consecutiveFailedSets >= CONSECUTIVE_FAILED_SETS_FOR_PLATEAU)
                    {
                        plateauDetected = true;
                        break;
                    }
                }
                else
                {
                    consecutiveFailedSets = 0;
                }
            }

            if (plateauDetected)
            {
                // Requirement 2.5: save plateau notification
                double deloadTarget = ComputeDeloadWeight(templateExercise.TargetWeight);
                var notification = new Notification
                {
                    Client = new Client { ClientId = request.ClientId },
                    Title = "Deload Recommended",
                    Message = $"Plateau detected for exercise {exercise.ExerciseName}. Recommended deload target: {deloadTarget} kg.",
                    Type = NotificationType.Plateau,
                    RelatedId = exercise.ParentTemplateExerciseId,
                    DateCreated = DateTime.UtcNow,
                    IsRead = false,
                };
                await this.notificationRepository.SaveNotificationAsync(notification);

                // Requirement 2.6: mark exercise as system adjusted
                exercise.IsSystemAdjusted = true;
                exercise.AdjustmentNote = $"Plateau detected. Recommended deload target weight: {deloadTarget} kg.";
            }
            else
            {
                double averageRatio = ratios.Average();

                if (averageRatio >= OVERLOAD_PERFORMANCE_THRESHOLD)
                {
                    // Requirements 3.1, 3.2, 3.3: apply progressive overload
                    double increment = templateExercise.MuscleGroup == MuscleGroup.LEGS
                        ? LEGS_WEIGHT_INCREMENT
                        : DEFAULT_WEIGHT_INCREMENT;
                    double newWeight = templateExercise.TargetWeight + increment;

                    await this.workoutTemplateRepository.UpdateTemplateExerciseWeightAsync(exercise.ParentTemplateExerciseId, newWeight);

                    // Requirement 3.4: mark exercise as system adjusted
                    exercise.IsSystemAdjusted = true;
                    exercise.AdjustmentNote = $"Progressive overload applied for {templateExercise.MuscleGroup} muscle group. Weight increased by {increment} kg. Next session target weight: {newWeight} kg.";
                }
            }
        }
    }

    public async Task<bool> ProcessDeloadAsync(ProcessDeloadRequestDataTransferObject request)
    {
        var templateExercise = await this.workoutTemplateRepository.GetTemplateExerciseByIdAsync(request.RelatedId);
        if (templateExercise == null)
        {
            return false;
        }

        double roundedWeight = ComputeDeloadWeight(templateExercise.TargetWeight);
        return await this.workoutTemplateRepository.UpdateTemplateExerciseWeightAsync(request.RelatedId, roundedWeight);
    }

    private static double ComputeDeloadWeight(double currentWeight)
    {
        double rawWeight = currentWeight * DELOAD_FACTOR;
        return Math.Max(0, Math.Round(rawWeight / DELOAD_ROUNDING_STEP) * DELOAD_ROUNDING_STEP);
    }
}
