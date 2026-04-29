using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public sealed class ClientService : IClientService
{
    private const string UnlockedAchievementIcon = "&#xE73E;";
    private const string LockedAchievementIcon = "&#xE72E;";

    private readonly IRepositoryAchievements achievementsRepository;
    private readonly IRepositoryNotification notificationRepository;
    private readonly IRepositoryNutrition nutritionRepository;
    private readonly IRepositoryWorkoutLog workoutLogRepository;
    private readonly IRepositoryWorkoutTemplate workoutTemplateRepository;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    public ClientService(
        IRepositoryAchievements achievementsRepository,
        IRepositoryNotification notificationRepository,
        IRepositoryNutrition nutritionRepository,
        IRepositoryWorkoutLog workoutLogRepository,
        IRepositoryWorkoutTemplate workoutTemplateRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        this.achievementsRepository = achievementsRepository;
        this.notificationRepository = notificationRepository;
        this.nutritionRepository = nutritionRepository;
        this.workoutLogRepository = workoutLogRepository;
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetAchievementsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var showcaseItems = await this.achievementsRepository.GetAchievementShowcaseForClientAsync(clientId, cancellationToken);
        return showcaseItems.Select(item => new AchievementDataTransferObject
        {
            AchievementId = item.AchievementId,
            Title = item.Title,
            Description = item.Description,
            Criteria = item.Criteria,
            IsUnlocked = item.IsUnlocked,
            Icon = item.IsUnlocked ? UnlockedAchievementIcon : LockedAchievementIcon,
        }).ToList();
    }

    public async Task<IReadOnlyList<NotificationDataTransferObject>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var notifications = await this.notificationRepository.GetNotificationsAsync(clientId, cancellationToken);
        return notifications.Select(MapNotification).ToList();
    }

    public async Task<NutritionPlanDataTransferObject?> GetActiveNutritionPlanAsync(int clientId, CancellationToken cancellationToken = default)
    {
        if (clientId <= 0)
        {
            return null;
        }

        var plans = await this.nutritionRepository.GetNutritionPlansForClientAsync(clientId, cancellationToken);
        var today = DateTime.Today;
        NutritionPlan? activePlan = null;

        foreach (var plan in plans)
        {
            if (plan.StartDate.Date > today || plan.EndDate.Date < today)
            {
                continue;
            }

            if (activePlan == null || plan.StartDate > activePlan.StartDate)
            {
                activePlan = plan;
            }
        }

        if (activePlan == null)
        {
            return null;
        }

        var meals = await this.nutritionRepository.GetMealsForPlanAsync(activePlan.NutritionPlanId, cancellationToken);
        return MapNutritionPlan(activePlan, meals);
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId, cancellationToken);
        return logs.Select(MapWorkoutLog).ToList();
    }

    public async Task<bool> FinalizeWorkoutAsync(FinalizeWorkoutRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        if (request?.WorkoutLog == null)
        {
            return false;
        }

        var log = MapToWorkoutLog(request.WorkoutLog);
        log.Date = DateTime.Now;
        return await this.workoutLogRepository.SaveWorkoutLogAsync(log, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var templates = await this.workoutTemplateRepository.GetAvailableWorkoutsAsync(clientId, cancellationToken);
        return templates.Select(MapWorkoutTemplate).ToList();
    }

    public async Task<PreviousBestWeightsDataTransferObject> GetPreviousBestWeightsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId, cancellationToken);
        var bestWeightsByExerciseName = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var log in logs)
        {
            foreach (var exercise in log.Exercises)
            {
                foreach (var set in exercise.Sets)
                {
                    double actualWeight = set.ActualWeight ?? 0;
                    if (!bestWeightsByExerciseName.TryGetValue(set.ExerciseName, out double previousBest) || actualWeight > previousBest)
                    {
                        bestWeightsByExerciseName[set.ExerciseName] = actualWeight;
                    }
                }
            }
        }

        return new PreviousBestWeightsDataTransferObject
        {
            BestWeightsByExercise = bestWeightsByExerciseName,
        };
    }

    public async Task<ClientProfileSnapshotDataTransferObject> GetClientProfileSnapshotAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId, cancellationToken);
        var totalCalories = logs.Sum(log => log.TotalCaloriesBurned);

        var latestLog = logs.FirstOrDefault();
        string latestSessionHint;
        List<LoggedExerciseDataTransferObject> loggedExercises;

        if (latestLog != null && latestLog.Exercises is { Count: > 0 })
        {
            latestSessionHint = $"Latest session: {latestLog.WorkoutName} - {latestLog.Date:g}";
            loggedExercises = latestLog.Exercises.Select(MapLoggedExercise).ToList();
        }
        else
        {
            latestSessionHint = "No completed workouts with exercises yet.";
            loggedExercises = new List<LoggedExerciseDataTransferObject>();
        }

        var activePlan = await this.GetActiveNutritionPlanAsync(clientId, cancellationToken);
        var meals = activePlan?.Meals ?? new List<MealDataTransferObject>();

        return new ClientProfileSnapshotDataTransferObject
        {
            CaloriesSummary = $"Calories burned (all logged workouts): {totalCalories}",
            LatestSessionHint = latestSessionHint,
            LoggedExercises = loggedExercises,
            Meals = meals,
        };
    }

    public Task<bool> ConfirmDeloadAsync(ConfirmDeloadRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        if (request == null || request.NotificationId <= 0)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(false);
    }

    public async Task<bool> SyncNutritionAsync(NutritionSyncRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        if (request == null || request.ClientId <= 0)
        {
            return false;
        }

        var syncEndpoint = this.configuration["NutritionSync:Endpoint"];
        if (string.IsNullOrWhiteSpace(syncEndpoint))
        {
            return false;
        }

        try
        {
            var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(request.ClientId, cancellationToken);
            var totalCalories = logs.Sum(log => log.TotalCaloriesBurned);
            var lastIntensityTag = logs.FirstOrDefault()?.IntensityTag;
            var workoutDifficulty = string.IsNullOrWhiteSpace(lastIntensityTag) ? "unknown" : lastIntensityTag;

            var payload = new
            {
                TotalCalories = totalCalories,
                WorkoutDifficulty = workoutDifficulty,
                UserBmi = 0.0f,
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var httpClient = this.httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(syncEndpoint, content, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
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
            Rating = log.Rating,
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
            ClientId = template.ClientId,
            Client = MapClient(template.Client),
            Name = template.Name,
            Type = template.Type.ToString(),
        };
    }

    private static NotificationDataTransferObject MapNotification(Notification notification)
    {
        return new NotificationDataTransferObject
        {
            NotificationId = notification.NotificationId,
            Client = MapClient(notification.Client),
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            RelatedId = notification.RelatedId,
            DateCreated = notification.DateCreated,
            IsRead = notification.IsRead,
        };
    }

    private static NutritionPlanDataTransferObject MapNutritionPlan(NutritionPlan plan, IReadOnlyList<Meal> meals)
    {
        return new NutritionPlanDataTransferObject
        {
            NutritionPlanId = plan.NutritionPlanId,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            Meals = meals.Select(MapMeal).ToList(),
        };
    }

    private static MealDataTransferObject MapMeal(Meal meal)
    {
        return new MealDataTransferObject
        {
            MealId = meal.MealId,
            Name = meal.Name,
            Ingredients = new List<string>(meal.Ingredients),
            Instructions = meal.Instructions,
        };
    }

    private static ClientDataTransferObject MapClient(Client? client)
    {
        if (client == null)
        {
            return new ClientDataTransferObject();
        }

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

    private static WorkoutLog MapToWorkoutLog(WorkoutLogDataTransferObject dto)
    {
        var workoutType = Enum.TryParse<WorkoutType>(dto.Type, ignoreCase: true, out var parsedType) ? parsedType : WorkoutType.CUSTOM;

        return new WorkoutLog
        {
            WorkoutLogId = dto.WorkoutLogId,
            WorkoutName = dto.WorkoutName,
            Date = dto.Date,
            Duration = dto.Duration,
            SourceTemplateId = dto.SourceTemplateId,
            Type = workoutType,
            TotalCaloriesBurned = dto.TotalCaloriesBurned,
            AverageMetabolicEquivalent = dto.AverageMetabolicEquivalent,
            IntensityTag = dto.IntensityTag,
            Rating = dto.Rating,
            TrainerNotes = dto.TrainerNotes,
            Exercises = dto.Exercises.Select(MapToLoggedExercise).ToList(),
        };
    }

    private static LoggedExercise MapToLoggedExercise(LoggedExerciseDataTransferObject dto)
    {
        var targetMuscles = Enum.TryParse<MuscleGroup>(dto.TargetMuscles, ignoreCase: true, out var parsedMuscles) ? parsedMuscles : MuscleGroup.OTHER;

        return new LoggedExercise
        {
            LoggedExerciseId = dto.LoggedExerciseId,
            ExerciseName = dto.ExerciseName,
            TargetMuscles = targetMuscles,
            Sets = dto.Sets.Select(MapToLoggedSet).ToList(),
            MetabolicEquivalent = dto.MetabolicEquivalent,
            ExerciseCaloriesBurned = dto.ExerciseCaloriesBurned,
            PerformanceRatio = dto.PerformanceRatio,
            IsSystemAdjusted = dto.IsSystemAdjusted,
            AdjustmentNote = dto.AdjustmentNote,
        };
    }

    private static LoggedSet MapToLoggedSet(LoggedSetDataTransferObject dto)
    {
        return new LoggedSet
        {
            LoggedSetId = dto.LoggedSetId,
            ExerciseName = dto.ExerciseName,
            SetIndex = dto.SetIndex,
            TargetReps = dto.TargetReps,
            ActualReps = dto.ActualReps,
            TargetWeight = dto.TargetWeight,
            ActualWeight = dto.ActualWeight,
            SetNumber = dto.SetNumber,
        };
    }
}
