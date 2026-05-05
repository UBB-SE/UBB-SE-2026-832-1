using Microsoft.AspNetCore.Mvc;
using ClassLibrary.DTOs;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/achievement")]
public class AchievementController : ControllerBase
{
    private readonly IClientService clientService;

    public AchievementController(IClientService clientService)
    {
        this.clientService = clientService;
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClient(int clientId)
    {
        var result = await this.clientService.GetAchievementsAsync(clientId);
        return Ok(result);
    }
}