using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class TrainerService : ITrainerService
{
    private readonly ITrainerRepository trainerRepository;
    private readonly IWorkoutTemplateRepository workoutTemplateRepository;
    private readonly IWorkoutLogRepository workoutLogRepository;
    private readonly INutritionRepository nutritionRepository;

    public TrainerService(
        ITrainerRepository trainerRepository,
        IWorkoutTemplateRepository workoutTemplateRepository,
        IWorkoutLogRepository workoutLogRepository,
        INutritionRepository nutritionRepository)
    {
        this.trainerRepository = trainerRepository;
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.workoutLogRepository = workoutLogRepository;
        this.nutritionRepository = nutritionRepository;
    }

    public async Task<IReadOnlyList<ClientDataTransferObject>> GetAssignedClientsAsync(int trainerId)
    {
        var clients = await this.trainerRepository.GetTrainerClientsAsync(trainerId);
        return clients.Select(MapClient).ToList();
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetClientWorkoutHistoryAsync(int clientId)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId);
        return logs.Select(MapWorkoutLog).ToList();
    }

    public async Task<bool> SaveWorkoutFeedbackAsync(SaveWorkoutFeedbackRequestDataTransferObject request)
    {
        var log = new WorkoutLog
        {
            WorkoutLogId = request.WorkoutLogId,
            Rating = request.Rating,
            TrainerNotes = request.TrainerNotes,
        };

        try
        {
            await this.workoutLogRepository.UpdateWorkoutLogAsync(log);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId)
    {
        var templates = await this.workoutTemplateRepository.GetAvailableWorkoutsAsync(clientId);
        return templates.Select(MapWorkoutTemplate).ToList();
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int templateId)
    {
        try
        {
            await this.trainerRepository.DeleteWorkoutTemplateAsync(templateId);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public async Task<bool> SaveTrainerWorkoutAsync(WorkoutTemplateDataTransferObject templateDto)
    {
        var template = MapToWorkoutTemplate(templateDto);
        await this.trainerRepository.SaveTrainerWorkoutAsync(template);
        return true;
    }

    public async Task<(bool Success, string ErrorMessage)> AssignNewRoutineAsync(AssignNewRoutineRequestDataTransferObject request)
    {
        if (request.EditingTemplateId == null)
        {
            var newTemplate = new WorkoutTemplate
            {
                Client = new Client { ClientId = request.ClientId },
                Name = request.RoutineName,
                Exercises = request.Exercises.Select(MapToTemplateExercise).ToList(),
            };

            await this.trainerRepository.SaveTrainerWorkoutAsync(newTemplate);
        }
        else
        {
            var existingTemplate = await this.workoutTemplateRepository.GetByIdAsync(request.EditingTemplateId.Value);
            if (existingTemplate == null)
            {
                return (false, "Template not found.");
            }

            existingTemplate.Name = request.RoutineName;
            existingTemplate.Exercises = request.Exercises.Select(MapToTemplateExercise).ToList();
            await this.trainerRepository.SaveTrainerWorkoutAsync(existingTemplate);
        }

        return (true, string.Empty);
    }

    public async Task<IReadOnlyList<string>> GetAllExerciseNamesAsync()
    {
        var templates = await this.workoutTemplateRepository.GetAllTemplatesAsync();
        return templates
            .SelectMany(template => template.Exercises)
            .Select(exercise => exercise.Name)
            .Distinct()
            .ToList();
    }

    public async Task<bool> AssignNutritionPlanAsync(AssignNutritionPlanRequestDataTransferObject request)
    {
        try
        {
            await this.nutritionRepository.AssignNutritionPlanToClientAsync(request.ClientId, request.NutritionPlanId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateAndAssignNutritionPlanAsync(CreateNutritionPlanRequestDataTransferObject request)
    {
        try
        {
            var plan = new NutritionPlan
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Meals = new List<Meal>(),
            };

            await this.nutritionRepository.InsertNutritionPlanAsync(plan);
            await this.nutritionRepository.AssignNutritionPlanToClientAsync(request.ClientId, plan.NutritionPlanId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static ClientDataTransferObject MapClient(Client client)
    {
        return new ClientDataTransferObject
        {
            ClientId = client.ClientId,
            Email = client.Email,
            FullName = client.FullName,
            Weight = client.Weight,
            Height = client.Height,
            PrimaryGoal = client.PrimaryGoal,
        };
    }

    private static WorkoutLogDataTransferObject MapWorkoutLog(WorkoutLog log)
    {
        return new WorkoutLogDataTransferObject
        {
            WorkoutLogId = log.WorkoutLogId,
            Client = MapClient(log.Client),
            WorkoutName = log.WorkoutName,
            Date = log.Date,
            Duration = log.Duration,
            SourceTemplateId = log.SourceTemplateId,
            Type = log.Type.ToString(),
            Exercises = log.Exercises.Select(MapLoggedExercise).ToList(),
            TotalCaloriesBurned = log.TotalCaloriesBurned,
            AverageMetabolicEquivalent = log.AverageMetabolicEquivalent,
            IntensityTag = log.IntensityTag,
            Rating = log.Rating <= 0 ? null : log.Rating,
            TrainerNotes = log.TrainerNotes,
        };
    }

    private static LoggedExerciseDataTransferObject MapLoggedExercise(LoggedExercise exercise)
    {
        return new LoggedExerciseDataTransferObject
        {
            LoggedExerciseId = exercise.LoggedExerciseId,
            ExerciseName = exercise.ExerciseName,
            TargetMuscles = exercise.TargetMuscles.ToString(),
            Sets = exercise.Sets.Select(MapLoggedSet).ToList(),
            MetabolicEquivalent = exercise.MetabolicEquivalent,
            ExerciseCaloriesBurned = exercise.ExerciseCaloriesBurned,
            PerformanceRatio = exercise.PerformanceRatio,
            IsSystemAdjusted = exercise.IsSystemAdjusted,
            AdjustmentNote = exercise.AdjustmentNote,
        };
    }

    private static LoggedSetDataTransferObject MapLoggedSet(LoggedSet set)
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

    private static WorkoutTemplateDataTransferObject MapWorkoutTemplate(WorkoutTemplate template)
    {
        return new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = template.WorkoutTemplateId,
            ClientId = template.Client?.ClientId ?? 0,
            Client = template.Client != null ? MapClient(template.Client) : new ClientDataTransferObject(),
            Name = template.Name,
            Type = template.Type.ToString(),
        };
    }

    private static WorkoutTemplate MapToWorkoutTemplate(WorkoutTemplateDataTransferObject templateDto)
    {
        var workoutType = Enum.TryParse<WorkoutType>(templateDto.Type, ignoreCase: true, out var parsedType)
            ? parsedType
            : WorkoutType.TRAINER_ASSIGNED;

        return new WorkoutTemplate
        {
            WorkoutTemplateId = templateDto.WorkoutTemplateId,
            Client = new Client { ClientId = templateDto.ClientId },
            Name = templateDto.Name,
            Type = workoutType,
        };
    }

    private static TemplateExercise MapToTemplateExercise(TemplateExerciseDataTransferObject exerciseDto)
    {
        var muscleGroup = Enum.TryParse<MuscleGroup>(exerciseDto.MuscleGroup, ignoreCase: true, out var parsedMuscle)
            ? parsedMuscle
            : MuscleGroup.OTHER;

        return new TemplateExercise
        {
            Name = exerciseDto.Name,
            MuscleGroup = muscleGroup,
            TargetSets = exerciseDto.TargetSets,
            TargetReps = exerciseDto.TargetReps,
            TargetWeight = exerciseDto.TargetWeight,
        };
    }
}
