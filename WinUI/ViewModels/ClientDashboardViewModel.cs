using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using WinUI.Services;
using WinUI.Services.Interfaces;

namespace WinUI.ViewModels;

public sealed partial class ClientDashboardViewModel : ObservableObject
{
    private const int DefaultPageSize = 5;

    private readonly IDashboardService dashboardService;
    private readonly IAchievementService achievementService;
    private readonly IUserSession session;
    private readonly IAnalyticsDashboardRefreshBus refreshBus;
    private CancellationTokenSource? loadCts;

    public ClientDashboardViewModel(
        IDashboardService dashboardService,
        IAchievementService achievementService,
        IUserSession session,
        IAnalyticsDashboardRefreshBus refreshBus)
    {
        this.dashboardService = dashboardService;
        this.achievementService = achievementService;
        this.session = session;
        this.refreshBus = refreshBus;
        this.refreshBus.RefreshRequested += OnRefreshRequested;
    }

    [ObservableProperty] private int totalWorkouts;
    [ObservableProperty] private string activeTimeSevenDaysDisplay = "0:00:00";
    [ObservableProperty] private string preferredWorkoutDisplay = "-";
    [ObservableProperty] private ISeries[] chartSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] chartXAxes = new[] { new Axis() };
    [ObservableProperty] private int currentPage;
    [ObservableProperty] private int totalCount;
    [ObservableProperty] private bool canGoPrevious;
    [ObservableProperty] private bool canGoNext;
    [ObservableProperty] private bool isLoadingSummary;
    [ObservableProperty] private bool isLoadingHistory;
    [ObservableProperty] private bool isLoadingChart;
    [ObservableProperty] private bool showEmptyState = true;

    public ObservableCollection<WorkoutHistoryItem> HistoryItems { get; } = new();
    public ObservableCollection<Achievement> RecentAchievements { get; } = new();
    public int PageSize { get; set; } = DefaultPageSize;

    private int TotalPages => totalCount == 0 ? 0 : (totalCount + PageSize - 1) / PageSize;
    public string PageDisplayText => TotalPages == 0 ? string.Empty : $"Page {CurrentPage + 1} of {TotalPages}";

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageDisplayText));
    partial void OnTotalCountChanged(int value) => OnPropertyChanged(nameof(PageDisplayText));

    [RelayCommand]
    private async Task LoadAllAsync()
    {
        CancelPendingLoad();
        var token = loadCts!.Token;
        var clientId = session.CurrentClientId;

        try
        {
            SetLoadingStates(true);

            var summaryTask = dashboardService.GetSummaryAsync(clientId, token);
            var bucketsTask = dashboardService.GetConsistencyAsync(clientId, token);
            CurrentPage = 0;
            var historyTask = dashboardService.GetHistoryAsync(clientId, CurrentPage, PageSize, token);

            await Task.WhenAll(summaryTask, bucketsTask, historyTask);
            token.ThrowIfCancellationRequested();

            ApplySummary(summaryTask.Result);
            ApplyBuckets(bucketsTask.Result);
            ApplyHistory(historyTask.Result);
            await LoadRecentAchievementsAsync((int)clientId);
        }
        catch (OperationCanceledException) { }
        finally { SetLoadingStates(false); }
    }

    private void SetLoadingStates(bool state)
    {
        IsLoadingSummary = IsLoadingChart = IsLoadingHistory = state;
    }

    private void ApplySummary(DashboardSummary summary)
    {
        TotalWorkouts = summary.TotalWorkouts;
        ActiveTimeSevenDaysDisplay = summary.TotalActiveTimeLastSevenDays.ToString(); 
        PreferredWorkoutDisplay = string.IsNullOrWhiteSpace(summary.PreferredWorkoutName) ? "-" : summary.PreferredWorkoutName;
    }

    private void ApplyBuckets(IReadOnlyList<ConsistencyWeekBucket> buckets)
    {
        ChartSeries = new ISeries[] { new LineSeries<int> { Values = buckets.Select(b => b.WorkoutCount).ToArray() } };
        ChartXAxes = new Axis[] { new Axis { Labels = buckets.Select(b => b.WeekStart.ToString("MMM dd")).ToArray() } };
    }

    private void ApplyHistory(WorkoutHistoryPageResult result)
    {
        TotalCount = result.TotalCount;
        ShowEmptyState = result.TotalCount == 0;
        UpdatePaginationButtons();
        HistoryItems.Clear();
        foreach (var item in result.Items) HistoryItems.Add(item);
    }

    private async Task LoadRecentAchievementsAsync(int clientId)
    {
        RecentAchievements.Clear();
        var achievements = await achievementService.GetAchievementsAsync(clientId);
        foreach (var item in achievements.Where(a => a.IsUnlocked).Take(3)) RecentAchievements.Add(item);
    }

    private void UpdatePaginationButtons()
    {
        CanGoPrevious = CurrentPage > 0;
        CanGoNext = CurrentPage < TotalPages - 1;
    }

    private void CancelPendingLoad()
    {
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();
    }

    private void OnRefreshRequested(object? sender, EventArgs e) => _ = LoadAllAsync();
}