using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken)
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

        public async Task<bool> CheckIfUsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            var users = await userRepository.GetAllAsync(cancellationToken);

            return users.Any(user =>
                string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            return await userRepository.GetByUsernameAndPasswordAsync(username, password, cancellationToken);
        }

        public async Task<User?> RegisterUserAsync(User user, CancellationToken cancellationToken = default)
        {
            if (await CheckIfUsernameExistsAsync(user.Username, cancellationToken))
                return null;

            await userRepository.AddAsync(user, cancellationToken);
            return user;
        }

        public async Task<UserData?> GetUserDataAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await userRepository.GetUserDataByUserIdAsync(userId, cancellationToken);
        }

        public async Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default)
        {
            await userRepository.UpdateUserDataAsync(data, cancellationToken);
        }
    }
}