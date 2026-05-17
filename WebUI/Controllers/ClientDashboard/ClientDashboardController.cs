using ClassLibrary.Proxies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebUI.Filters;
using WebUI.Models.ClientDashboard;

namespace WebUI.Controllers.ClientDashboard;

[VerifiedClient]
public sealed class ClientDashboardController : Controller
{
    private readonly IClientDashboardProxy dashboardProxy;
    private readonly IUserSession userSession;

    public ClientDashboardController(IClientDashboardProxy dashboardProxy, IUserSession userSession)
    {
        this.dashboardProxy = dashboardProxy;
        this.userSession = userSession;
    }

    public async Task<IActionResult> Index(int page = 0)
    {
        var model = await ClientDashboardIndexViewModel.LoadAsync(
            this.dashboardProxy,
            this.userSession.CurrentClientId,
            page).ConfigureAwait(false);

        return this.View(model);
    }

    public async Task<IActionResult> Detail(int workoutLogId)
    {
        var detail = await this.dashboardProxy
            .GetWorkoutSessionDetailAsync(this.userSession.CurrentClientId, workoutLogId)
            .ConfigureAwait(false);

        if (detail == null)
        {
            return this.NotFound();
        }

        return this.PartialView("_WorkoutHistoryDetail", WorkoutSessionDetailViewModel.FromDetail(detail));
    }
}
