namespace WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.AchievementBus.Interfaces;
using ClassLibrary.Models;

[ApiController]
[Route("api/achievementUnlockedBus")]
public class AchievementUnlockedBusController : ControllerBase
{
    private readonly IAchievementUnlockedBus achievementUnlockedBus;

    public AchievementUnlockedBusController(IAchievementUnlockedBus achievementUnlockedBus)
    {
        this.achievementUnlockedBus = achievementUnlockedBus;
    }

    [HttpPost("notify-unlocked")]
    public IActionResult Notify([FromBody] AchievementShowcaseItem request)
    {
        this.achievementUnlockedBus.NotifyUnlocked(request);
        return Ok();
    }
}