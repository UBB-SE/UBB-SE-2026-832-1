using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class ActiveWorkoutProxy : IActiveWorkoutProxy
{
	private readonly HttpClient httpClient;
	private const string BaseAddress = ApiBaseUrl.BASE_URL + "/api";
	private const string ClientRoute = BaseAddress + "/client";

	public ActiveWorkoutProxy(HttpClient httpClient)
	{
		this.httpClient = httpClient;
	}

	public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsForClient(int clientId)
	{
		var workoutTemplateDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<WorkoutTemplateDataTransferObject>>($"{ClientRoute}/{clientId}/available-workouts");
		return DataTransferObjectToDomainModelMappers.MapWorkoutTemplates(workoutTemplateDataTransferObjects);
	}

	public Task<IReadOnlyList<WorkoutTemplate>> GetCustomAndTrainerAssignedWorkoutsForClient(int clientId)
	{
		return this.GetAvailableWorkoutsForClient(clientId);
	}

	public async Task<WorkoutTemplate?> FindWorkoutTemplateById(int clientId, int? id)
	{
		if (!id.HasValue)
		{
			return null;
		}

		var availableWorkoutTemplates = await this.GetAvailableWorkoutsForClient(clientId);
		return availableWorkoutTemplates.FirstOrDefault(workoutTemplate => workoutTemplate.WorkoutTemplateId == id.Value);
	}

	public async Task<IDictionary<string, double>> GetPreviousBestWeightsAsync(int clientId)
	{
		var previousBestWeightsDataTransferObject = await this.httpClient.GetFromJsonAsync<PreviousBestWeightsDataTransferObject>($"{ClientRoute}/{clientId}/previous-best-weights");
		return previousBestWeightsDataTransferObject?.BestWeightsByExercise ?? new Dictionary<string, double>();
	}

	public async Task<bool> SaveSetAsync(WorkoutLog workoutLog, LoggedSet set)
	{
		ArgumentNullException.ThrowIfNull(workoutLog);
		ArgumentNullException.ThrowIfNull(set);

		set.WorkoutLog = workoutLog;
		var exercise = workoutLog.Exercises.FirstOrDefault(loggedExercise => string.Equals(loggedExercise.ExerciseName, set.ExerciseName, StringComparison.OrdinalIgnoreCase));
		if (exercise is null)
		{
			exercise = new LoggedExercise
			{
				ExerciseName = set.ExerciseName,
				WorkoutLog = workoutLog,
				TargetMuscles = set.Exercise?.TargetMuscles ?? default,
			};
			workoutLog.Exercises.Add(exercise);
		}

		if (!exercise.Sets.Contains(set))
		{
			set.Exercise = exercise;
			exercise.Sets.Add(set);
		}

		if (workoutLog.WorkoutLogId == 0)
		{
			return true;
		}

		var modifyWorkoutResponse = await this.httpClient.PutAsJsonAsync($"{ClientRoute}/modify-workout", MapToWorkoutLogDataTransferObject(workoutLog));
		return modifyWorkoutResponse.IsSuccessStatusCode;
	}

	public async Task<bool> FinalizeWorkoutAsync(WorkoutLog workoutLog)
	{
		ArgumentNullException.ThrowIfNull(workoutLog);

		var finalizeWorkoutRequestDataTransferObject = new FinalizeWorkoutRequestDataTransferObject
		{
			WorkoutLog = MapToWorkoutLogDataTransferObject(workoutLog),
		};
		var finalizeWorkoutResponse = await this.httpClient.PostAsJsonAsync($"{ClientRoute}/finalize-workout", finalizeWorkoutRequestDataTransferObject);
		return finalizeWorkoutResponse.IsSuccessStatusCode;
	}

	public async Task<IReadOnlyList<Notification>> GetNotifications(int clientId)
	{
		var notificationDataTransferObjects = await this.httpClient.GetFromJsonAsync<List<NotificationDataTransferObject>>($"{ClientRoute}/{clientId}/notifications");
		return DataTransferObjectToDomainModelMappers.MapNotifications(notificationDataTransferObjects);
	}

	public async Task ConfirmDeload(Notification notification)
	{
		ArgumentNullException.ThrowIfNull(notification);

		var request = new ConfirmDeloadRequestDataTransferObject
		{
			NotificationId = notification.NotificationId,
		};

		var confirmDeloadResponse = await this.httpClient.PostAsJsonAsync($"{ClientRoute}/confirm-deload", request);
		confirmDeloadResponse.EnsureSuccessStatusCode();
	}

	private static WorkoutLogDataTransferObject MapToWorkoutLogDataTransferObject(WorkoutLog workoutLog)
	{
		return new WorkoutLogDataTransferObject
		{
			WorkoutLogId = workoutLog.WorkoutLogId,
			Client = MapToClientDataTransferObject(workoutLog.Client),
			WorkoutName = workoutLog.WorkoutName,
			Date = workoutLog.Date,
			Duration = workoutLog.Duration,
			SourceTemplateId = workoutLog.SourceTemplateId,
			Type = workoutLog.Type.ToString(),
			Exercises = workoutLog.Exercises.Select(MapToLoggedExerciseDataTransferObject).ToList(),
			TotalCaloriesBurned = workoutLog.TotalCaloriesBurned,
			AverageMetabolicEquivalent = workoutLog.AverageMetabolicEquivalent,
			IntensityTag = workoutLog.IntensityTag,
			Rating = workoutLog.Rating,
			TrainerNotes = workoutLog.TrainerNotes,
		};
	}

	private static ClientDataTransferObject MapToClientDataTransferObject(Client client)
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

	private static LoggedExerciseDataTransferObject MapToLoggedExerciseDataTransferObject(LoggedExercise exercise)
	{
		return new LoggedExerciseDataTransferObject
		{
			LoggedExerciseId = exercise.LoggedExerciseId,
			ExerciseName = exercise.ExerciseName,
			TargetMuscles = exercise.TargetMuscles.ToString(),
			Sets = exercise.Sets.Select(MapToLoggedSetDataTransferObject).ToList(),
			MetabolicEquivalent = exercise.MetabolicEquivalent,
			ExerciseCaloriesBurned = exercise.ExerciseCaloriesBurned,
			PerformanceRatio = exercise.PerformanceRatio,
			IsSystemAdjusted = exercise.IsSystemAdjusted,
			AdjustmentNote = exercise.AdjustmentNote,
		};
	}

	private static LoggedSetDataTransferObject MapToLoggedSetDataTransferObject(LoggedSet set)
	{
		return new LoggedSetDataTransferObject
		{
			LoggedSetId = set.LoggedSetId,
			ExerciseName = set.ExerciseName,
			SetIndex = set.SetIndex,
			TargetReps = set.TargetReps ?? 0,
			ActualReps = set.ActualReps ?? 0,
			TargetWeight = (float)(set.TargetWeight ?? 0),
			ActualWeight = (float)(set.ActualWeight ?? 0),
			SetNumber = set.SetNumber,
		};
	}
}
