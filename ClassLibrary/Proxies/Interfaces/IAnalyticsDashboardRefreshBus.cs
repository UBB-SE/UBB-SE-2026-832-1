namespace ClassLibrary.Proxies.Interfaces;

public interface IAnalyticsDashboardRefreshBus
{
    event EventHandler? RefreshRequested;

    void RequestRefresh();
}



