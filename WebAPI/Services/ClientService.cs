using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public sealed class ClientService : IClientService
{
    private const string UNLOCKED_ACHIEVEMENT_ICON = "&#xE73E;";
    private const string LOCKED_ACHIEVEMENT_ICON = "&#xE72E;";

    private readonly IAchievementsRepository achievementsRepository;
    private readonly INotificationRepository notificationRepository;
    private readonly INutritionRepository nutritionRepository;
    private readonly IWorkoutLogRepository workoutLogRepository;
    private readonly IWorkoutTemplateRepository workoutTemplateRepository;
    private readonly IClientRepository clientRepository;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    public ClientService(
        IAchievementsRepository achievementsRepository,
        INotificationRepository notificationRepository,
        INutritionRepository nutritionRepository,
        IWorkoutLogRepository workoutLogRepository,
        IWorkoutTemplateRepository workoutTemplateRepository,
        IClientRepository clientRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        this.achievementsRepository = achievementsRepository;
        this.notificationRepository = notificationRepository;
        this.nutritionRepository = nutritionRepository;
        this.workoutLogRepository = workoutLogRepository;
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.clientRepository = clientRepository;
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetAchievementsAsync(int clientId)
    {
        var showcaseItems = await this.achievementsRepository.GetAchievementShowcaseForClientAsync(clientId);
        return showcaseItems.Select(item => new AchievementDataTransferObject
        {
            AchievementId = item.AchievementId,
            Title = item.Title,
            Description = item.Description,
            Criteria = item.Criteria,
            IsUnlocked = item.IsUnlocked,
            Icon = item.IsUnlocked ? UNLOCKED_ACHIEVEMENT_ICON : LOCKED_ACHIEVEMENT_ICON,
        }).ToList();
    }

    public async Task<IReadOnlyList<NotificationDataTransferObject>> GetNotificationsAsync(int clientId)
    {
        var notifications = await this.notificationRepository.GetNotificationsAsync(clientId);
        return notifications.Select(MapNotification).ToList();
    }

    public async Task<NutritionPlanDataTransferObject?> GetActiveNutritionPlanAsync(int clientId)
    {
        if (clientId <= 0)
        {
            return null;
        }

        var plans = await this.nutritionRepository.GetNutritionPlansForClientAsync(clientId);
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

        return MapNutritionPlan(activePlan, activePlan.Meals);
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId);
        return logs.Select(MapWorkoutLog).ToList();
    }

    public async Task<bool> FinalizeWorkoutAsync(FinalizeWorkoutRequestDataTransferObject request)
    {
        if (request?.WorkoutLog == null)
        {
            return false;
        }

        if (request.WorkoutLog.Client == null || request.WorkoutLog.Client.ClientId <= 0)
        {
            return false;
        }

        var client = await this.clientRepository.GetByIdAsync(request.WorkoutLog.Client.ClientId);
        if (client == null)
        {
            return false;
        }

        var log = MapToWorkoutLog(request.WorkoutLog, client);
        log.Date = DateTime.Now;
        await this.workoutLogRepository.SaveWorkoutLogAsync(log);
        return true;
    }

    public async Task<IReadOnlyList<WorkoutTemplateDataTransferObject>> GetAvailableWorkoutsAsync(int clientId)
    {
        var templates = await this.workoutTemplateRepository.GetAvailableWorkoutsAsync(clientId);
        return templates.Select(MapWorkoutTemplate).ToList();
    }

    public async Task<PreviousBestWeightsDataTransferObject> GetPreviousBestWeightsAsync(int clientId)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId);
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

    public async Task<ClientProfileSnapshotDataTransferObject> GetClientProfileSnapshotAsync(int clientId)
    {
        var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(clientId);
        var totalCalories = logs.Sum(log => log.TotalCaloriesBurned);

        var latestLog = logs.OrderByDescending(log => log.Date).FirstOrDefault();
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

        var activePlan = await this.GetActiveNutritionPlanAsync(clientId);
        var meals = activePlan?.Meals ?? new List<MealDataTransferObject>();

        return new ClientProfileSnapshotDataTransferObject
        {
            CaloriesSummary = $"Calories burned (all logged workouts): {totalCalories}",
            LatestSessionHint = latestSessionHint,
            LoggedExercises = loggedExercises,
            Meals = meals,
        };
    }

    public Task<bool> ConfirmDeloadAsync(ConfirmDeloadRequestDataTransferObject request)
    {
        if (request == null || request.NotificationId <= 0)
        {
            return Task.FromResult(false);
        }

        throw new NotImplementedException("Deload confirmation requires progression service integration which is not yet available.");
    }

    public async Task<bool> SyncNutritionAsync(NutritionSyncRequestDataTransferObject request)
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
            var logs = await this.workoutLogRepository.GetWorkoutHistoryAsync(request.ClientId);
            var totalCalories = logs.Sum(log => log.TotalCaloriesBurned);
            var lastIntensityTag = logs.OrderByDescending(log => log.Date).FirstOrDefault()?.IntensityTag;
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
            var response = await httpClient.PostAsync(syncEndpoint, content);
            return response.IsSuccessStatusCode;
        }
        catch (OperationCanceledException)
        {
            throw;
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
            ClientId = template.Client?.ClientId ?? 0,
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

    private static WorkoutLog MapToWorkoutLog(WorkoutLogDataTransferObject workoutLogDataTransferObject, Client client)
    {
        var workoutType = Enum.TryParse<WorkoutType>(workoutLogDataTransferObject.Type, ignoreCase: true, out var parsedType) ? parsedType : WorkoutType.CUSTOM;

        var workoutLog = new WorkoutLog
        {
            WorkoutLogId = 0,
            Client = client,
            WorkoutName = workoutLogDataTransferObject.WorkoutName,
            Date = workoutLogDataTransferObject.Date,
            Duration = workoutLogDataTransferObject.Duration,
            SourceTemplateId = workoutLogDataTransferObject.SourceTemplateId,
            Type = workoutType,
            TotalCaloriesBurned = workoutLogDataTransferObject.TotalCaloriesBurned,
            AverageMetabolicEquivalent = workoutLogDataTransferObject.AverageMetabolicEquivalent,
            IntensityTag = workoutLogDataTransferObject.IntensityTag,
            Rating = workoutLogDataTransferObject.Rating,
            TrainerNotes = workoutLogDataTransferObject.TrainerNotes,
        };

        foreach (var loggedExerciseDataTransferObject in workoutLogDataTransferObject.Exercises ?? Enumerable.Empty<LoggedExerciseDataTransferObject>())
        {
            var loggedExercise = MapToLoggedExercise(loggedExerciseDataTransferObject, workoutLog);
            workoutLog.Exercises.Add(loggedExercise);
        }

        return workoutLog;
    }

    private static LoggedExercise MapToLoggedExercise(LoggedExerciseDataTransferObject loggedExerciseDataTransferObject, WorkoutLog parentWorkoutLog)
    {
        var targetMuscles = Enum.TryParse<MuscleGroup>(loggedExerciseDataTransferObject.TargetMuscles, ignoreCase: true, out var parsedMuscles) ? parsedMuscles : MuscleGroup.OTHER;

        var loggedExercise = new LoggedExercise
        {
            LoggedExerciseId = 0,
            WorkoutLog = parentWorkoutLog,
            ExerciseName = loggedExerciseDataTransferObject.ExerciseName,
            TargetMuscles = targetMuscles,
            MetabolicEquivalent = loggedExerciseDataTransferObject.MetabolicEquivalent,
            ExerciseCaloriesBurned = loggedExerciseDataTransferObject.ExerciseCaloriesBurned,
            PerformanceRatio = loggedExerciseDataTransferObject.PerformanceRatio,
            IsSystemAdjusted = loggedExerciseDataTransferObject.IsSystemAdjusted,
            AdjustmentNote = loggedExerciseDataTransferObject.AdjustmentNote,
        };

        foreach (var loggedSetDataTransferObject in loggedExerciseDataTransferObject.Sets ?? Enumerable.Empty<LoggedSetDataTransferObject>())
        {
            var loggedSet = MapToLoggedSet(loggedSetDataTransferObject, parentWorkoutLog, loggedExercise);
            loggedExercise.Sets.Add(loggedSet);
        }

        return loggedExercise;
    }

    private static LoggedSet MapToLoggedSet(LoggedSetDataTransferObject loggedSetDataTransferObject, WorkoutLog parentWorkoutLog, LoggedExercise parentLoggedExercise)
    {
        return new LoggedSet
        {
            LoggedSetId = 0,
            WorkoutLog = parentWorkoutLog,
            Exercise = parentLoggedExercise,
            ExerciseName = loggedSetDataTransferObject.ExerciseName,
            SetIndex = loggedSetDataTransferObject.SetIndex,
            TargetReps = loggedSetDataTransferObject.TargetReps,
            ActualReps = loggedSetDataTransferObject.ActualReps,
            TargetWeight = loggedSetDataTransferObject.TargetWeight,
            ActualWeight = loggedSetDataTransferObject.ActualWeight,
            SetNumber = loggedSetDataTransferObject.SetNumber,
        };
    }
}
