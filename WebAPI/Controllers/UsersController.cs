using ClassLibrary.Models;
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
        var users = await userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userService.LoginAsync(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await userService.RegisterUserAsync(new User
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role
        });

        if (user == null)
        {
            return Conflict("Username already taken.");
        }

        return CreatedAtAction(nameof(GetAll), user);
    }

    [HttpGet("exists/{username}")]
    public async Task<IActionResult> CheckUsernameExists(string username)
    {
        bool exists = await userService.CheckIfUsernameExistsAsync(username);
        return Ok(exists);
    }

    [HttpGet("{userId}/data")]
    public async Task<IActionResult> GetUserData(int userId)
    {
        var userData = await userService.GetUserDataAsync(userId);
        if (userData == null)
        {
            return NotFound();
        }

        return Ok(userData);
    }

    [HttpPost("data")]
    public async Task<IActionResult> AddUserData([FromBody] UserData userData)
    {
        await userService.UpdateUserDataAsync(userData);
        return Ok();
    }

    [HttpPut("data")]
    public async Task<IActionResult> UpdateUserData([FromBody] UserData userData)
    {
        await userService.UpdateUserDataAsync(userData);
        return Ok();
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