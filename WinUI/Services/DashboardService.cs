using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Models;
using System.Net.Http.Json;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly HttpClient httpClient;

    public DashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DashboardSummary> GetSummaryAsync(long clientId, CancellationToken token)
    {
        var dto = await httpClient.GetFromJsonAsync<DashboardSummaryDto>($"dashboard/summary/{clientId}", token);
        return dto == null ? new DashboardSummary() : new DashboardSummary
        {
            TotalWorkouts = dto.TotalWorkouts,
            TotalActiveTimeLastSevenDays = dto.TotalActiveTimeLastSevenDays,
            PreferredWorkoutName = dto.PreferredWorkoutName
        };
    }

    public async Task<List<ConsistencyWeekBucket>> GetConsistencyAsync(long clientId, CancellationToken token)
    {
        var dtos = await httpClient.GetFromJsonAsync<List<ConsistencyWeekBucketDto>>($"dashboard/consistency/{clientId}", token);
        return dtos?.Select(d => new ConsistencyWeekBucket { WeekStart = d.WeekStart, WorkoutCount = d.WorkoutCount }).ToList() ?? new();
    }

    public async Task<WorkoutHistoryPageResult> GetHistoryAsync(long clientId, int page, int pageSize, CancellationToken token)
    {
        var dto = await httpClient.GetFromJsonAsync<WorkoutHistoryPageResultDto>($"dashboard/history/{clientId}?page={page}&pageSize={pageSize}", token);
        return dto == null ? new WorkoutHistoryPageResult() : new WorkoutHistoryPageResult
        {
            TotalCount = dto.TotalCount,
            Items = dto.Items.Select(i => new WorkoutHistoryItem {   }).ToList()
        };
    }
}