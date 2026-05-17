using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Proxies.Interfaces;

namespace WebUI.Models.ClientDashboard;

public sealed class ClientDashboardIndexViewModel
{
    public const int DefaultPageSize = 5;

    public int TotalWorkouts { get; init; }

    public string ActiveTimeSevenDaysDisplay { get; init; } = "0m";

    public string PreferredWorkoutDisplay { get; init; } = "-";

    public IReadOnlyList<ConsistencyWeekBucket> ConsistencyBuckets { get; init; } = [];

    public IReadOnlyList<WorkoutHistoryItemViewModel> HistoryItems { get; init; } = [];

    public IReadOnlyList<AchievementDataTransferObject> RecentAchievements { get; init; } = [];

    public bool ShowEmptyState { get; init; }

    public int CurrentPage { get; init; }

    public int TotalCount { get; init; }

    public bool CanGoPrevious { get; init; }

    public bool CanGoNext { get; init; }

    public string PageDisplayText { get; init; } = string.Empty;

    public static async Task<ClientDashboardIndexViewModel> LoadAsync(
        IClientDashboardProxy dashboardProxy,
        int clientId,
        int page,
        int pageSize = DefaultPageSize)
    {
        var summaryTask = dashboardProxy.GetDashboardSummaryAsync(clientId);
        var bucketsTask = dashboardProxy.GetConsistencyLastFourWeeksAsync(clientId);
        var historyTask = dashboardProxy.GetWorkoutHistoryPageAsync(clientId, page, pageSize);
        var achievementsTask = dashboardProxy.GetRecentAchievementsAsync(clientId);

        await Task.WhenAll(summaryTask, bucketsTask, historyTask, achievementsTask).ConfigureAwait(false);

        var summary = await summaryTask.ConfigureAwait(false);
        var history = await historyTask.ConfigureAwait(false);
        var totalPages = history.TotalCount == 0
            ? 0
            : (history.TotalCount + pageSize - 1) / pageSize;

        return new ClientDashboardIndexViewModel
        {
            TotalWorkouts = summary.TotalWorkouts,
            ActiveTimeSevenDaysDisplay = FormatTimeSpan(summary.TotalActiveTimeLastSevenDays),
            PreferredWorkoutDisplay = string.IsNullOrWhiteSpace(summary.PreferredWorkoutName)
                ? "-"
                : summary.PreferredWorkoutName,
            ConsistencyBuckets = await bucketsTask.ConfigureAwait(false),
            HistoryItems = history.Items.Select(WorkoutHistoryItemViewModel.FromRow).ToList(),
            RecentAchievements = (await achievementsTask.ConfigureAwait(false)).ToList(),
            ShowEmptyState = history.TotalCount == 0,
            CurrentPage = page,
            TotalCount = history.TotalCount,
            CanGoPrevious = page > 0 && totalPages > 0,
            CanGoNext = totalPages > 0 && page < totalPages - 1,
            PageDisplayText = totalPages == 0
                ? string.Empty
                : $"Page {page + 1} of {totalPages}",
        };
    }

    private static string FormatTimeSpan(TimeSpan span) =>
        span.TotalHours >= 1 ? $"{(int)span.TotalHours}h {span.Minutes}m" : $"{span.TotalMinutes:F0}m";
}
