using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.DTOs;
using WinUI.Services;

namespace WinUI.ViewModels;

public sealed partial class ClientDashboardViewModel : ObservableObject
{
    public const int DefaultPageSize = 5;

    private readonly IClientDashboardService clientDashboardService;
    private readonly IUserSession userSession;
    private CancellationTokenSource? loadCts;

    public ClientDashboardViewModel(IClientDashboardService clientDashboardService, IUserSession userSession)
    {
        this.clientDashboardService = clientDashboardService;
        this.userSession = userSession;
    }

    [ObservableProperty]
    public partial int TotalWorkouts { get; set; }

    [ObservableProperty]
    public partial string ActiveTimeSevenDaysDisplay { get; set; } = "0:00:00";

    [ObservableProperty]
    public partial string PreferredWorkoutDisplay { get; set; } = "-";

    private ObservableCollection<ConsistencyWeekBucket> consistencyBuckets = new();

    [ObservableProperty]
    public partial ISeries[] ChartSeries { get; set; } = Array.Empty<ISeries>();

    [ObservableProperty]
    public partial Axis[] ChartXAxes { get; set; } = new[] { new Axis() };

    [ObservableProperty]
    public partial int CurrentPage { get; set; }

    [ObservableProperty]
    public partial int TotalCount { get; set; }

    [ObservableProperty]
    public partial bool CanGoPrevious { get; set; }

    [ObservableProperty]
    public partial bool CanGoNext { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingSummary { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingHistory { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingChart { get; set; }

    [ObservableProperty]
    public partial bool ShowEmptyState { get; set; } = true;

    public ObservableCollection<WorkoutHistoryRow> HistoryItems { get; } = new();

    public ObservableCollection<AchievementDataTransferObject> RecentAchievements { get; } = new();

    public int PageSize { get; set; } = DefaultPageSize;

    private int TotalPages => TotalCount == 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

    public string PageDisplayText => TotalPages == 0 ? string.Empty : string.Create(CultureInfo.InvariantCulture, $"Page {CurrentPage + 1} of {TotalPages}");

    partial void OnCurrentPageChanged(int value) => OnPropertyChanged(nameof(PageDisplayText));
    partial void OnTotalCountChanged(int value) => OnPropertyChanged(nameof(PageDisplayText));

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
        this.loadCts = new CancellationTokenSource();

        try
        {
            IsLoadingSummary = true;
            IsLoadingChart = true;
            IsLoadingHistory = true;

            var clientId = this.userSession.CurrentClientId;

            var summaryTask = this.clientDashboardService.GetDashboardSummaryAsync(clientId);
            var bucketsTask = this.clientDashboardService.GetConsistencyLastFourWeeksAsync(clientId);
            CurrentPage = 0;
            var historyTask = this.clientDashboardService.GetWorkoutHistoryPageAsync(clientId, CurrentPage, PageSize);
            var achievementsTask = this.clientDashboardService.GetRecentAchievementsAsync(clientId);

            await Task.WhenAll(summaryTask, bucketsTask, historyTask, achievementsTask).ConfigureAwait(true);

            ApplySummary(summaryTask.Result);
            ApplyBuckets(bucketsTask.Result);
            ApplyHistory(historyTask.Result);
            ApplyRecentAchievements(achievementsTask.Result);
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
        this.loadCts = new CancellationTokenSource();
        IsLoadingHistory = true;

        try
        {
            var clientId = this.userSession.CurrentClientId;
            var result = await this.clientDashboardService.GetWorkoutHistoryPageAsync(clientId, CurrentPage, PageSize).ConfigureAwait(true);
            ApplyHistory(result);
        }
        finally
        {
            IsLoadingHistory = false;
        }
    }

    private void ApplyRecentAchievements(IReadOnlyList<AchievementDataTransferObject> achievements)
    {
        RecentAchievements.Clear();
        foreach (var achievement in achievements)
        {
            RecentAchievements.Add(achievement);
        }
    }

    private void CancelPendingLoad()
    {
        this.loadCts?.Cancel();
        this.loadCts?.Dispose();
        this.loadCts = null;
    }

    private void ApplySummary(DashboardSummary summary)
    {
        TotalWorkouts = summary.TotalWorkouts;
        ActiveTimeSevenDaysDisplay = summary.TotalActiveTimeLastSevenDays.ToString();
        PreferredWorkoutDisplay = string.IsNullOrWhiteSpace(summary.PreferredWorkoutName) ? "-" : summary.PreferredWorkoutName;
    }

    private void ApplyBuckets(IReadOnlyList<ConsistencyWeekBucket> buckets)
    {
        this.consistencyBuckets.Clear();
        foreach (var bucket in buckets)
        {
            this.consistencyBuckets.Add(bucket);
        }

        this.ChartSeries = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = buckets.Select(bucket => bucket.WorkoutCount).ToArray(),
                Name = "Workouts",
                GeometrySize = 12,
                Stroke = new SolidColorPaint(new SKColor(0x00, 0x5F, 0xB8)) { StrokeThickness = 3 },
                GeometryStroke = new SolidColorPaint(new SKColor(0x00, 0x5F, 0xB8)) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(new SKColor(0xFF, 0xFF, 0xFF)),
                Fill = new LinearGradientPaint(
                    new[] { new SKColor(0x00, 0x5F, 0xB8, 90), new SKColor(0x00, 0x5F, 0xB8, 0) },
                    new SKPoint(0.5f, 0),
                    new SKPoint(0.5f, 1))
            }
        };

        this.ChartXAxes = new Axis[]
        {
            new Axis
            {
                Labels = buckets.Select(bucket => bucket.WeekStart.ToString("MMM dd", CultureInfo.InvariantCulture)).ToArray(),
                LabelsRotation = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(new SKColor(0x8A, 0x8A, 0x8A))
            }
        };
    }

    private void ApplyHistory(WorkoutHistoryPageResult result)
    {
        TotalCount = result.TotalCount;
        ShowEmptyState = result.TotalCount == 0;
        UpdatePaginationButtons();

        HistoryItems.Clear();
        foreach (var row in result.Items)
        {
            HistoryItems.Add(row);
        }
    }

    private void UpdatePaginationButtons()
    {
        var pages = TotalPages;
        CanGoPrevious = CurrentPage > 0 && pages > 0;
        CanGoNext = pages > 0 && CurrentPage < pages - 1;
    }
}
