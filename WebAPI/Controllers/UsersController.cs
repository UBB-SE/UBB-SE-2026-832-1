using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

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
    public async Task<IActionResult> GetAll()
    {
        var users = await this.userService.GetUsersAsync();
        return this.Ok(users);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await this.userService.LoginAsync(request.Username, request.Password);
        if (user == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await this.userService.RegisterAsync(request.Username, request.Password, request.Role);
        if (user == null)
        {
            return this.Conflict("Username already taken.");
        }

        return this.CreatedAtAction(nameof(GetAll), user);
    }

    [HttpGet("exists/{username}")]
    public async Task<IActionResult> CheckUsernameExists(string username)
    {
        bool exists = await this.userService.CheckIfUsernameExistsAsync(username);
        return this.Ok(exists);
    }

    [HttpGet("{userId}/data")]
    public async Task<IActionResult> GetUserData(int userId)
    {
        var userData = await this.userService.GetUserDataAsync(userId);
        if (userData == null)
        {
            return this.NotFound();
        }

        return this.Ok(userData);
    }

    [HttpPost("data")]
    public async Task<IActionResult> AddUserData([FromBody] UserDataDto userDataDto)
    {
        await this.userService.AddUserDataAsync(userDataDto);
        return this.Ok();
    }

    [HttpPut("data")]
    public async Task<IActionResult> UpdateUserData([FromBody] UserDataDto userDataDto)
    {
        await this.userService.UpdateUserDataAsync(userDataDto);
        return this.Ok();
    }
}

public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
