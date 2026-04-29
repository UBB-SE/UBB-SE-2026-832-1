using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await this.userService.GetUsersAsync(cancellationToken);
        return this.Ok(users);
    }
}

