using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;

namespace WebApi.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await this.userRepository.GetAllAsync(cancellationToken);

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
