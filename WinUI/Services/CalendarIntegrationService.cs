using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services;

public interface IUserSession
{
    int CurrentClientId { get; }
}

public interface ICalendarIntegrationService
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<string> GenerateCalendarIcsAsync(WorkoutTemplate workoutTemplate, int durationWeeks, IReadOnlyList<int> selectedDaysOfWeek, CancellationToken cancellationToken = default);
}

public sealed class CalendarIntegrationService : ICalendarIntegrationService
{
    private readonly HttpClient httpClient;
    private const string apiBaseAddress = "https://localhost:7197";
    private const string clientRoutePrefix = "api/client";

    public CalendarIntegrationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{apiBaseAddress}/{clientRoutePrefix}/{clientId}/available-workouts";
        var response = await this.httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var dataTransferObjects = await response.Content.ReadFromJsonAsync<List<WorkoutTemplateDataTransferObject>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return MapToModels(dataTransferObjects);
    }

    public Task<string> GenerateCalendarIcsAsync(WorkoutTemplate workoutTemplate, int durationWeeks, IReadOnlyList<int> selectedDaysOfWeek, CancellationToken cancellationToken = default)
    {
        _ = workoutTemplate;
        _ = durationWeeks;
        _ = selectedDaysOfWeek;
        _ = cancellationToken;
        return Task.FromResult("BEGIN:VCALENDAR\r\nVERSION:2.0\r\nEND:VCALENDAR\r\n");
    }

    private static IReadOnlyList<WorkoutTemplate> MapToModels(List<WorkoutTemplateDataTransferObject>? dataTransferObjects)
    {
        if (dataTransferObjects is null || dataTransferObjects.Count == 0)
        {
            return Array.Empty<WorkoutTemplate>();
        }

        return dataTransferObjects.Select(MapToModel).ToList();
    }

    private static WorkoutTemplate MapToModel(WorkoutTemplateDataTransferObject dataTransferObject)
    {
        var client = MapToClient(dataTransferObject.Client, dataTransferObject.ClientId);
        return new WorkoutTemplate
        {
            WorkoutTemplateId = dataTransferObject.WorkoutTemplateId,
            Client = client,
            Name = dataTransferObject.Name,
            Type = ParseWorkoutType(dataTransferObject.Type),
            Exercises = [],
        };
    }

    private static Client MapToClient(ClientDataTransferObject dataTransferObject, int clientId)
    {
        return new Client
        {
            ClientId = clientId,
            Email = dataTransferObject.Email,
            FullName = dataTransferObject.FullName,
            Weight = dataTransferObject.Weight,
            Height = dataTransferObject.Height,
            PrimaryGoal = dataTransferObject.PrimaryGoal,
        };
    }

    private static WorkoutType ParseWorkoutType(string type)
    {
        if (Enum.TryParse<WorkoutType>(type, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return WorkoutType.CUSTOM;
    }
}
