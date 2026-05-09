using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class CalendarWorkoutCatalogService : ICalendarWorkoutCatalogService
{
    private readonly HttpClient httpClient;
    private const int FALLBACK_WORKOUT_COUNT = 4;

    public CalendarWorkoutCatalogService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout)
    {
        try
        {
            string requestUri = $"{ApiBaseUrl.BASE_URL}/api/client/{clientId}/available-workouts";
            Task<List<WorkoutTemplateDataTransferObject>?> workoutsLoadTask = this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>(requestUri);
            Task completedTask = await Task.WhenAny(workoutsLoadTask, Task.Delay(timeout));
            if (completedTask != workoutsLoadTask)
            {
                return GetFallbackWorkouts(clientId);
            }

            List<WorkoutTemplateDataTransferObject>? workoutTemplateDataTransferObjects = await workoutsLoadTask;
            IReadOnlyList<WorkoutTemplate> workoutTemplates = DataTransferObjectToDomainModelMappers.MapWorkoutTemplates(workoutTemplateDataTransferObjects);
            if (workoutTemplates.Count == 0)
            {
                return GetFallbackWorkouts(clientId);
            }

            return workoutTemplates;
        }
        catch
        {
            return GetFallbackWorkouts(clientId);
        }
    }

    public IReadOnlyList<WorkoutTemplate> GetFallbackWorkouts(int clientId)
    {
        WorkoutTemplate fullBodyStrength = new WorkoutTemplate
        {
            WorkoutTemplateId = -1,
            Client = CreateFallbackClient(clientId),
            Name = "Fallback - Full Body Strength",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
            {
                new TemplateExercise { TemplateExerciseId = -101, Name = "Back Squat", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 6, TargetWeight = 60 },
                new TemplateExercise { TemplateExerciseId = -102, Name = "Bench Press", MuscleGroup = MuscleGroup.CHEST, TargetSets = 4, TargetReps = 6, TargetWeight = 40 },
                new TemplateExercise { TemplateExerciseId = -103, Name = "Barbell Row", MuscleGroup = MuscleGroup.BACK, TargetSets = 4, TargetReps = 8, TargetWeight = 35 },
            }
        };

        WorkoutTemplate hiitConditioning = new WorkoutTemplate
        {
            WorkoutTemplateId = -2,
            Client = CreateFallbackClient(clientId),
            Name = "Fallback - HIIT Conditioning",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
            {
                new TemplateExercise { TemplateExerciseId = -201, Name = "Burpees", MuscleGroup = MuscleGroup.CORE, TargetSets = 4, TargetReps = 12, TargetWeight = 0 },
                new TemplateExercise { TemplateExerciseId = -202, Name = "Jump Squats", MuscleGroup = MuscleGroup.LEGS, TargetSets = 4, TargetReps = 15, TargetWeight = 0 },
                new TemplateExercise { TemplateExerciseId = -203, Name = "Mountain Climbers", MuscleGroup = MuscleGroup.CORE, TargetSets = 4, TargetReps = 20, TargetWeight = 0 },
            }
        };

        WorkoutTemplate pushPull = new WorkoutTemplate
        {
            WorkoutTemplateId = -3,
            Client = CreateFallbackClient(clientId),
            Name = "Fallback - Push Pull Split",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
            {
                new TemplateExercise { TemplateExerciseId = -301, Name = "Overhead Press", MuscleGroup = MuscleGroup.SHOULDERS, TargetSets = 4, TargetReps = 8, TargetWeight = 25 },
                new TemplateExercise { TemplateExerciseId = -302, Name = "Pull-Ups", MuscleGroup = MuscleGroup.BACK, TargetSets = 4, TargetReps = 8, TargetWeight = 0 },
                new TemplateExercise { TemplateExerciseId = -303, Name = "Dumbbell Curl", MuscleGroup = MuscleGroup.ARMS, TargetSets = 3, TargetReps = 12, TargetWeight = 12 },
            }
        };

        WorkoutTemplate coreMobility = new WorkoutTemplate
        {
            WorkoutTemplateId = -4,
            Client = CreateFallbackClient(clientId),
            Name = "Fallback - Core and Mobility",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
            {
                new TemplateExercise { TemplateExerciseId = -401, Name = "Plank", MuscleGroup = MuscleGroup.CORE, TargetSets = 3, TargetReps = 60, TargetWeight = 0 },
                new TemplateExercise { TemplateExerciseId = -402, Name = "Dead Bug", MuscleGroup = MuscleGroup.CORE, TargetSets = 3, TargetReps = 12, TargetWeight = 0 },
                new TemplateExercise { TemplateExerciseId = -403, Name = "Hip Bridge", MuscleGroup = MuscleGroup.LEGS, TargetSets = 3, TargetReps = 15, TargetWeight = 0 },
            }
        };

        List<WorkoutTemplate> fallbackWorkouts = new List<WorkoutTemplate>(FALLBACK_WORKOUT_COUNT)
        {
            fullBodyStrength,
            hiitConditioning,
            pushPull,
            coreMobility
        };
        return fallbackWorkouts;
    }

    private static Client CreateFallbackClient(int clientId)
    {
        return new Client
        {
            ClientId = clientId,
            Email = "fallback@local",
            FullName = "Fallback Client",
            Weight = 0,
            Height = 0,
            PrimaryGoal = "Fallback",
        };
    }
}
