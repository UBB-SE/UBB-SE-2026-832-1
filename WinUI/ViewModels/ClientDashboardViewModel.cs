using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using ClassLibrary.DTOs;
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

    public ObservableCollection<AchievementDataTransferObject> RecentAchievements { get; } = new();

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

    public ObservableCollection<ConsistencyWeekBucket> ConsistencyBuckets => this.consistencyBuckets;

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

    public async Task LoadWorkoutDetailAsync(WorkoutHistoryItemViewModel item)
    {
        if (item.IsLoadingDetail)
        {
            return;
        }

        if (item.ExerciseCalories.Count > 0 && item.ExerciseSetGroups.Count > 0)
        {
            return;
        }

        item.IsLoadingDetail = true;
        try
        {
            var detail = await this.dashboardService.GetWorkoutSessionDetailAsync(
                (int)this.userSession.CurrentClientId,
                item.WorkoutLogId).ConfigureAwait(true);

            if (detail == null)
            {
                return;
            }

            item.ApplyDetail(detail);
        }
        catch
        {
        }
        finally
        {
            item.IsLoadingDetail = false;
        }
    }

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
            RecentAchievements.Clear();

            var summaryTask = this.dashboardService.GetDashboardSummaryAsync(clientId);
            var bucketsTask = this.dashboardService.GetConsistencyLastFourWeeksAsync(clientId);
            CurrentPage = 0;
            var historyTask = this.dashboardService.GetWorkoutHistoryPageAsync(clientId, CurrentPage, PageSize);
            var achievementsTask = this.dashboardService.GetRecentAchievementsAsync(clientId);

            await Task.WhenAll(summaryTask, bucketsTask, historyTask, achievementsTask).ConfigureAwait(true);
            token.ThrowIfCancellationRequested();

            ApplySummary(summaryTask.Result);
            ApplyBuckets(bucketsTask.Result);
            ApplyHistory(historyTask.Result, clientId);
            ApplyAchievements(achievementsTask.Result);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            ShowEmptyState = true;
            TotalWorkouts = 0;
            ActiveTimeSevenDaysDisplay = "—";
            PreferredWorkoutDisplay = "—";
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
        catch (Exception ex)
        {
            ShowEmptyState = true;
            HistoryItems.Clear();
            UpdatePaginationButtons();
        }
        finally
        {
            IsLoadingHistory = false;
        }
    }

    public void ReloadAchievementsPreview()
    {
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
    }

    private void ApplyAchievements(IReadOnlyList<AchievementDataTransferObject> achievements)
    {
        RecentAchievements.Clear();
        foreach (var achievement in achievements)
        {
            RecentAchievements.Add(achievement);
        }
    }

    private void ApplyHistory(WorkoutHistoryPageResult result, int clientId)
    {
        TotalCount = result.TotalCount;
        ShowEmptyState = result.TotalCount == 0;
        UpdatePaginationButtons();

        foreach (var item in HistoryItems)
        {
            item.PropertyChanged -= OnHistoryItemPropertyChanged;
        }

        HistoryItems.Clear();
        foreach (var row in result.Items)
        {
            var item = WorkoutHistoryItemViewModel.FromWorkoutHistoryRow(row);
            item.PropertyChanged += OnHistoryItemPropertyChanged;
            HistoryItems.Add(item);
        }
    }

    private void OnHistoryItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(WorkoutHistoryItemViewModel.IsExpanded))
        {
            return;
        }

        if (sender is WorkoutHistoryItemViewModel item && item.IsExpanded)
        {
            _ = LoadWorkoutDetailAsync(item);
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
