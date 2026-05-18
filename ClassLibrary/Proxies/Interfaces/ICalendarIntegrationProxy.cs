using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface ICalendarIntegrationProxy
{
    Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<string> GenerateCalendarIcsAsync(WorkoutTemplate workoutTemplate, int durationWeeks, IReadOnlyList<int> selectedDaysOfWeek, CancellationToken cancellationToken = default);
}



