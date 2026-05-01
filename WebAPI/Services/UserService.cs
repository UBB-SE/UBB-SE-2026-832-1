using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await userRepository.GetAllAsync();

        return users
            .Select(user => new UserDto
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role
            })
            .ToList();
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        var users = await userRepository.GetAllAsync();

        return users.Any(user =>
            string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        return await userRepository.GetByUsernameAndPasswordAsync(username, password);
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        if (await CheckIfUsernameExistsAsync(user.Username))
            return null;

        await userRepository.AddAsync(user);
        return user;
    }

    public async Task<UserData?> GetUserDataAsync(int userId)
    {
        return await userRepository.GetUserDataByUserIdAsync(userId);
    }

    public async Task UpdateUserDataAsync(UserData data)
    {
        await userRepository.UpdateUserDataAsync(data);
    }
}