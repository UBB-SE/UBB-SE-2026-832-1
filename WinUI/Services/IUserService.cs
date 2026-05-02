using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();

    Task<UserDto?> LoginAsync(string username, string password);

    Task<UserDto?> RegisterAsync(string username, string password, string role);

    Task<bool> CheckIfUsernameExistsAsync(string username);

    Task<UserDataDto?> GetUserDataAsync(int userId);

    Task AddUserDataAsync(UserDataDto userDataDto);

    Task UpdateUserDataAsync(UserDataDto userDataDto);
}

