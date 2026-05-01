using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken);

        Task<bool> CheckIfUsernameExistsAsync(string username, CancellationToken cancellationToken = default);

        Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

        Task<User?> RegisterUserAsync(User user, CancellationToken cancellationToken = default);

        Task<UserData?> GetUserDataAsync(int userId, CancellationToken cancellationToken = default);

        Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default);
    }
}