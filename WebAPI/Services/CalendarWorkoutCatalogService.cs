using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class CalendarWorkoutCatalogService : ICalendarWorkoutCatalogService
{
    private static readonly List<WorkoutTemplateDataTransferObject> FALLBACK_WORKOUTS = new()
    {
        new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = -1,
            ClientId = 0,
            Name = "Full Body Strength",
            Type = "PREBUILT",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new TemplateExerciseDataTransferObject
                {
                    Name = "Squats",
                    MuscleGroup = "LEGS",
                    TargetSets = 3,
                    TargetReps = 12,
                    TargetWeight = 0
                },
                new TemplateExerciseDataTransferObject
                {
                    Name = "Push-ups",
                    MuscleGroup = "CHEST",
                    TargetSets = 3,
                    TargetReps = 15,
                    TargetWeight = 0
                },
                new TemplateExerciseDataTransferObject
                {
                    Name = "Rows",
                    MuscleGroup = "BACK",
                    TargetSets = 3,
                    TargetReps = 12,
                    TargetWeight = 0
                }
            }
        },
        new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = -2,
            ClientId = 0,
            Name = "Cardio Blast",
            Type = "PREBUILT",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new TemplateExerciseDataTransferObject
                {
                    Name = "Jumping Jacks",
                    MuscleGroup = "CARDIO",
                    TargetSets = 3,
                    TargetReps = 30,
                    TargetWeight = 0
                },
                new TemplateExerciseDataTransferObject
                {
                    Name = "Burpees",
                    MuscleGroup = "CARDIO",
                    TargetSets = 3,
                    TargetReps = 15,
                    TargetWeight = 0
                },
                new TemplateExerciseDataTransferObject
                {
                    Name = "Mountain Climbers",
                    MuscleGroup = "CORE",
                    TargetSets = 3,
                    TargetReps = 20,
                    TargetWeight = 0
                }
            }
        }
    };

    private readonly IWorkoutTemplateRepository workoutTemplateRepository;
    private readonly ILogger<CalendarWorkoutCatalogService> logger;

    public CalendarWorkoutCatalogService(
        IWorkoutTemplateRepository workoutTemplateRepository,
        ILogger<CalendarWorkoutCatalogService> logger)
    {
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.logger = logger;
    }

    public async Task<CalendarWorkoutCatalogResponseDataTransferObject> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout)
    {
        try
        {
            var dbTask = this.workoutTemplateRepository.GetAvailableWorkoutsAsync(clientId);
            var timeoutTask = Task.Delay(timeout);
            var completedTask = await Task.WhenAny(dbTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                this.logger.LogWarning(
                    "Database timeout after {Timeout}ms for client {ClientId}. Returning fallback workouts.",
                    timeout.TotalMilliseconds,
                    clientId);
                return GetFallbackWorkouts(clientId);
            }

            var workoutTemplates = await dbTask;
            var workoutDtos = workoutTemplates.Select(MapWorkoutTemplate).ToList();

            return new CalendarWorkoutCatalogResponseDataTransferObject
            {
                Workouts = workoutDtos
            };
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Error fetching workouts for client {ClientId}. Returning fallback workouts.",
                clientId);
            return GetFallbackWorkouts(clientId);
        }
    }

    public CalendarWorkoutCatalogResponseDataTransferObject GetFallbackWorkouts(int clientId)
    {
        return new CalendarWorkoutCatalogResponseDataTransferObject
        {
            Workouts = FALLBACK_WORKOUTS
        };
    }

    private static WorkoutTemplateDataTransferObject MapWorkoutTemplate(WorkoutTemplate template)
    {
        return new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = template.WorkoutTemplateId,
            ClientId = template.Client?.ClientId ?? 0,
            Name = template.Name,
            Type = template.Type.ToString(),
            Exercises = template.Exercises.Select(MapTemplateExercise).ToList()
        };
    }

    private static TemplateExerciseDataTransferObject MapTemplateExercise(TemplateExercise exercise)
    {
        return new TemplateExerciseDataTransferObject
        {
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup.ToString(),
            TargetSets = exercise.TargetSets,
            TargetReps = exercise.TargetReps,
            TargetWeight = exercise.TargetWeight
        };
    }
}
