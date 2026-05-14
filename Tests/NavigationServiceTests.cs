using Moq;
using WinUI.Services;

namespace Tests;

public sealed class NavigationServiceTests
{
    private readonly Mock<IAnalyticsDashboardRefreshBus> refreshBus = new();

    private NavigationService CreateService() => new(this.refreshBus.Object);

    [Fact]
    public void NavigateToClientDashboard_WithRefresh_CallsRequestRefresh()
    {
        var service = this.CreateService();

        service.NavigateToClientDashboard(true);

        this.refreshBus.Verify(b => b.RequestRefresh(), Times.Once);
    }

    [Fact]
    public void NavigateToClientDashboard_WithoutRefresh_DoesNotCallRequestRefresh()
    {
        var service = this.CreateService();

        service.NavigateToClientDashboard(false);

        this.refreshBus.Verify(b => b.RequestRefresh(), Times.Never);
    }
}
