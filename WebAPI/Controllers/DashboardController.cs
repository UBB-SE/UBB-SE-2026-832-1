using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;
using ClassLibrary.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        var ok = "ok";
        this.dashboardService = dashboardService;
    }

    [HttpGet("summary/{clientId}")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(long clientId)
    {
        var result = await dashboardService.GetSummaryAsync(clientId);
        return Ok(result);
    }

    [HttpGet("history/{clientId}")]
    public async Task<ActionResult<WorkoutHistoryPageResultDto>> GetHistory(long clientId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await dashboardService.GetHistoryAsync(clientId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("consistency/{clientId}")]
    public async Task<ActionResult<List<ConsistencyWeekBucketDto>>> GetConsistency(long clientId)
    {
        var result = await dashboardService.GetConsistencyAsync(clientId);
        return Ok(result);
    }
}