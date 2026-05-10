namespace WinUI.Services;

public sealed class AnalyticsDashboardRefreshBus : IAnalyticsDashboardRefreshBus
{
    public static AnalyticsDashboardRefreshBus Shared { get; } = new AnalyticsDashboardRefreshBus();

    public event EventHandler? RefreshRequested;

    public void RequestRefresh()
    {
        RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
