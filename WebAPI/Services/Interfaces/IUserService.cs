using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WebAPI.Services.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();

    Task<bool> CheckIfUsernameExistsAsync(string username);

    Task<User?> LoginAsync(string username, string password);

    Task<User?> RegisterUserAsync(User user);

    Task<UserData?> GetUserDataAsync(int userId);

    Task UpdateUserDataAsync(UserData data);
}