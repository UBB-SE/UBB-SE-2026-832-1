using ClassLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.UserSession.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("api/user-session")]
public class UserSessionController : ControllerBase
{
    private readonly IUserSessionService userSessionService;

    public UserSessionController(IUserSessionService userSessionService)
    {
        this.userSessionService = userSessionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentSession()
    {
        var result = await this.userSessionService.GetCurrentSessionAsync();
        return Ok(result);
    }

    [HttpPost("update-session")]
    public async Task<IActionResult> UpdateSession([FromBody] UserSessionRequestDto request)
    {
        var result = await this.userSessionService.UpdateSessionAsync(request);
        return Ok(result);
    }
}