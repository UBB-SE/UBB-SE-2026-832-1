using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class UserService : IUserService
{
    private readonly IUserServiceProxy serviceProxy;

    public UserService(IUserServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        return this.serviceProxy.GetUsersAsync();
    }

    public Task<UserDto?> LoginAsync(string username, string password)
    {
        return this.serviceProxy.LoginAsync(username, password);
    }

    public Task<UserDto?> RegisterAsync(string username, string password, string role)
    {
        return this.serviceProxy.RegisterAsync(username, password, role);
    }

    public Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        return this.serviceProxy.CheckIfUsernameExistsAsync(username);
    }

    public Task<UserDataDto?> GetUserDataAsync(int userId)
    {
        return this.serviceProxy.GetUserDataAsync(userId);
    }

    public Task AddUserDataAsync(UserDataDto userDataDto)
    {
        return this.serviceProxy.AddUserDataAsync(userDataDto);
    }

    public Task UpdateUserDataAsync(UserDataDto userDataDto)
    {
        return this.serviceProxy.UpdateUserDataAsync(userDataDto);
    }
}
