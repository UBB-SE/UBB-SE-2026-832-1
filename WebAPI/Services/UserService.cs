using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await this.userRepository.GetAllAsync();

        return users
            .Select(user => new UserDto
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role
            })
            .ToList();
    }
}
