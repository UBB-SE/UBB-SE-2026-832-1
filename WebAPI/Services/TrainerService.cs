using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

namespace WebAPI.Services;

public sealed class TrainerService : ITrainerService
{
    private readonly IWorkoutTemplateRepository workoutTemplateRepository;
    private readonly IWorkoutLogRepository workoutLogRepository;
    private readonly ITrainerRepository trainerRepository;
    private readonly IRepositoryNutrition nutritionRepository;

    public TrainerService(
        IWorkoutTemplateRepository workoutTemplateRepository,
        IWorkoutLogRepository workoutLogRepository,
        ITrainerRepository trainerRepository,
        IRepositoryNutrition nutritionRepository)
    {
        this.workoutTemplateRepository = workoutTemplateRepository;
        this.workoutLogRepository = workoutLogRepository;
        this.trainerRepository = trainerRepository;
        this.nutritionRepository = nutritionRepository;
    }

    public async Task<IReadOnlyList<ClientDto>> GetAssignedClientsAsync(int trainerId, CancellationToken cancellationToken = default)
    {
        var clients = await trainerRepository.GetTrainerClientsAsync(trainerId);
        return clients.Select(c => new ClientDto { Id = c.UserId, Name = c.Username }).ToList();
    }

    public async Task<IReadOnlyList<WorkoutHistoryResponseDto>> GetClientWorkoutHistoryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var logs = await workoutLogRepository.GetWorkoutHistoryAsync(clientId);
        return logs.Select(l => new WorkoutHistoryResponseDto
        {
            Id = l.WorkoutLogId,
            Name = l.WorkoutName,
            DurationMinutes = (int)l.Duration.TotalMinutes,
            Rating = Convert.ToInt32(Math.Round(l.Rating, MidpointRounding.AwayFromZero)),
            TrainerNotes = l.TrainerNotes,
            Date = l.Date
        }).ToList();
    }

    public async Task<bool> SaveWorkoutFeedbackAsync(WorkoutFeedbackRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return false;
        }

        return await workoutLogRepository.UpdateWorkoutLogFeedbackAsync(request.LogId, request.Rating, request.TrainerNotes);
    }

    public async Task<IReadOnlyList<WorkoutTemplateDto>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var workouts = await workoutTemplateRepository.GetAvailableWorkoutsAsync(clientId);
        return workouts.Select(w => new WorkoutTemplateDto
        {
            Id = w.WorkoutTemplateId,
            Name = w.Name
        }).ToList();
    }

    public async Task<bool> DeleteWorkoutTemplateAsync(int templateId, CancellationToken cancellationToken = default)
    {
        return await trainerRepository.DeleteWorkoutTemplateAsync(templateId);
    }

    public async Task<(bool Success, string ErrorMessage)> AssignNewRoutineAsync(RoutineRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return (false, "Request cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.RoutineName))
        {
            return (false, "Routine Name cannot be empty.");
        }

        if (request.Exercises == null || !request.Exercises.Any())
        {
            return (false, "You must add at least one exercise to the routine.");
        }

        var newTemplate = new WorkoutTemplate
        {
            WorkoutTemplateId = request.EditingTemplateId ?? 0,
            ClientId = request.ClientId,
            Name = request.RoutineName,
            Type = WorkoutType.TRAINER_ASSIGNED,
            Exercises = request.Exercises.Select(e => new TemplateExercise
            {
                TemplateExerciseId = e.Id,
                Name = e.ExerciseName,
                TargetSets = e.TargetSets,
                TargetReps = e.TargetReps,
                MuscleGroup = e.MuscleGroup,
                TargetWeight = e.TargetWeight
            }).ToList()
        };

        bool isSaved = await trainerRepository.SaveTrainerWorkoutAsync(newTemplate);
        if (!isSaved)
        {
            return (false, "Could not save routine to database.");
        }

        return (true, string.Empty);
    }

    public async Task<IReadOnlyList<string>> GetAllExerciseNamesAsync(CancellationToken cancellationToken = default)
    {
        return (await workoutTemplateRepository.GetAllExerciseNamesAsync()).ToList();
    }

    public async Task<bool> CreateAndAssignNutritionPlanAsync(NutritionPlanRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null || request.ClientId <= 0)
        {
            return false;
        }

        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;

        if (endDate < startDate)
        {
            return false;
        }

        var plan = new NutritionPlan
        {
            StartDate = startDate,
            EndDate = endDate,
        };

        var planId = await nutritionRepository.InsertNutritionPlanAsync(plan, cancellationToken);
        if (planId <= 0)
        {
            return false;
        }

        await nutritionRepository.AssignNutritionPlanToClientAsync(request.ClientId, planId, cancellationToken);
        return true;
    }
}
