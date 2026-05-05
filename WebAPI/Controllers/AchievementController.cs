using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;
using ClassLibrary.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/achievement")]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        this.achievementService = achievementService;
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClient(int clientId)
    {
        var result = await achievementService.GetAchievementsByClientAsync(clientId);
        return Ok(result);
    }
}