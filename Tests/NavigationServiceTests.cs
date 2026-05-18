using Microsoft.UI.Xaml.Controls;
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

    [Fact]
    public void GoBack_without_attached_frame_does_not_throw()
    {
        // no AttachFrame call → internal frame is null → should be a no-op
        var service = this.CreateService();

        var ex = Record.Exception(() => service.GoBack());

        Assert.Null(ex);
    }

    [Fact]
    public void NavigateToCalendarIntegration_without_frame_is_safe()
    {
        var service = this.CreateService();

        var ex = Record.Exception(() => service.NavigateToCalendarIntegration());

        Assert.Null(ex);
    }

    [Fact]
    public void NavigateToRankShowcase_without_frame_is_safe()
    {
        var service = this.CreateService();

        var ex = Record.Exception(() => service.NavigateToRankShowcase());

        Assert.Null(ex);
    }

    [Fact]
    public void NavigateToActiveWorkout_without_frame_does_not_throw()
    {
        var service = this.CreateService();

        var ex = Record.Exception(() => service.NavigateToActiveWorkout(42));

        Assert.Null(ex);
    }
}
