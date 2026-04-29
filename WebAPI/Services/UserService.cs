using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using ClassLibrary.Repositories.Interfaces;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken)
        {
            return (await _userRepository.GetAllAsync()).ToList();
        }

        public async Task<bool> CheckIfUsernameExistsAsync(string username)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Any(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _userRepository.GetByUsernameAndPasswordAsync(username, password);
        }

        public async Task<User?> RegisterUserAsync(User user)
        {
            if (await CheckIfUsernameExistsAsync(user.Username))
                return null;

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<UserData?> GetUserDataAsync(int userId)
        {
            return await _userRepository.GetUserDataByUserIdAsync(userId);
        }

        public async Task UpdateUserDataAsync(UserData data)
        {
            await _userRepository.UpdateUserDataAsync(data);
        }
    }
}