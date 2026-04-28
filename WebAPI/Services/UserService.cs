using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;

namespace WebAPI.Services;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);

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
