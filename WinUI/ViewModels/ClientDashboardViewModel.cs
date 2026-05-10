using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.ViewModels;

public sealed partial class ClientDashboardViewModel : ObservableObject
{
    public const int DefaultPageSize = 5;

    private readonly IClientDashboardService dashboardService;
    private readonly IUserSession userSession;
    private CancellationTokenSource? loadCts;
    private ObservableCollection<ConsistencyWeekBucket> consistencyBuckets = new();

    [ObservableProperty]
    private int totalWorkouts;

    [ObservableProperty]
    private string activeTimeSevenDaysDisplay = "0:00";

    [ObservableProperty]
    private string preferredWorkoutDisplay = "-";

    [ObservableProperty]
    private int currentPage;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private bool canGoPrevious;

    [ObservableProperty]
    private bool canGoNext;

    [ObservableProperty]
    private bool isLoadingSummary;

    [ObservableProperty]
    private bool isLoadingHistory;

    [ObservableProperty]
    private bool isLoadingChart;

    [ObservableProperty]
    private bool showEmptyState = true;

    public ObservableCollection<WorkoutHistoryItemViewModel> HistoryItems { get; } = new();

    public int PageSize { get; set; } = DefaultPageSize;

    private int TotalPages =>
        TotalCount == 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

    public string PageDisplayText =>
        TotalPages == 0
            ? string.Empty
            : string.Create(CultureInfo.InvariantCulture, $"Page {CurrentPage + 1} of {TotalPages}");

    public ClientDashboardViewModel(IClientDashboardService dashboardService, IUserSession userSession)
    {
        this.dashboardService = dashboardService;
        this.userSession = userSession;
        CancelPendingLoad();
    }

    [RelayCommand]
    private Task RefreshAsync() => LoadAllAsync();

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (!this.CanGoNext)
        {
            return;
        }
        this.CurrentPage++;
        await LoadHistoryPageAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (!this.CanGoPrevious)
        {
            return;
        }
        this.CurrentPage--;
        await LoadHistoryPageAsync().ConfigureAwait(true);
    }

    public Task LoadInitialAsync() => LoadAllAsync();

    private async Task LoadAllAsync()
    {
        CancelPendingLoad();
        var token = this.loadCts!.Token;
        var clientId = (int)this.userSession.CurrentClientId;

        try
        {
            IsLoadingSummary = true;
            IsLoadingChart = true;
            IsLoadingHistory = true;

            var summaryTask = this.dashboardService.GetDashboardSummaryAsync(clientId);
            var bucketsTask = this.dashboardService.GetConsistencyLastFourWeeksAsync(clientId);
            CurrentPage = 0;
            var historyTask = this.dashboardService.GetWorkoutHistoryPageAsync(clientId, CurrentPage, PageSize);

            await Task.WhenAll(summaryTask, bucketsTask, historyTask).ConfigureAwait(true);
            token.ThrowIfCancellationRequested();

            ApplySummary(summaryTask.Result);
            ApplyBuckets(bucketsTask.Result);
            ApplyHistory(historyTask.Result, clientId);

            IsLoadingSummary = false;
            IsLoadingChart = false;
            IsLoadingHistory = false;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoadingSummary = false;
            IsLoadingChart = false;
            IsLoadingHistory = false;
        }
    }

    private async Task LoadHistoryPageAsync()
    {
        CancelPendingLoad();
        var token = this.loadCts!.Token;
        IsLoadingHistory = true;

        try
        {
            var result = await this.dashboardService.GetWorkoutHistoryPageAsync(
                (int)this.userSession.CurrentClientId, CurrentPage, PageSize).ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
            ApplyHistory(result, (int)this.userSession.CurrentClientId);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoadingHistory = false;
        }
    }

    public void ReloadAchievementsPreview()
    {
        // This method is called when achievements are unlocked
        // Can be used to refresh achievement-related UI if needed
    }

    private void CancelPendingLoad()
    {
        this.loadCts?.Cancel();
        this.loadCts?.Dispose();
        this.loadCts = new CancellationTokenSource();
    }

    private void ApplySummary(DashboardSummary summary)
    {
        TotalWorkouts = summary.TotalWorkouts;
        ActiveTimeSevenDaysDisplay = FormatTimeSpan(summary.TotalActiveTimeLastSevenDays);
        PreferredWorkoutDisplay = string.IsNullOrWhiteSpace(summary.PreferredWorkoutName)
            ? "-"
            : summary.PreferredWorkoutName;
    }

    private void ApplyBuckets(IReadOnlyList<ConsistencyWeekBucket> buckets)
    {
        this.consistencyBuckets.Clear();
        foreach (var b in buckets)
        {
            consistencyBuckets.Add(b);
        }

        // Chart data can be used by code-behind to populate LiveChartsCore if available
        // For now, we store the buckets for potential future use
    }

    private void ApplyHistory(WorkoutHistoryPageResult result, int clientId)
    {
        TotalCount = result.TotalCount;
        ShowEmptyState = result.TotalCount == 0;
        UpdatePaginationButtons();

        HistoryItems.Clear();
        foreach (var row in result.Items)
        {
            HistoryItems.Add(WorkoutHistoryItemViewModel.FromWorkoutHistoryRow(row));
        }
    }

    private void UpdatePaginationButtons()
    {
        var pages = TotalPages;
        CanGoPrevious = CurrentPage > 0 && pages > 0;
        CanGoNext = pages > 0 && CurrentPage < pages - 1;
    }

    private static string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalHours >= 1)
        {
            return $"{(int)span.TotalHours}h {span.Minutes}m";
        }
        return $"{span.TotalMinutes:F0}m";
    }
}
