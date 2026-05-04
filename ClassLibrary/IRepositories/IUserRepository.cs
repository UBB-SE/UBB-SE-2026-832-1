using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByUsernameAndPasswordAsync(string username, string password);

    Task AddAsync(User entity);

    Task UpdateAsync(User entity);

    Task DeleteAsync(int id);

    Task AddUserDataAsync(UserData data);

    Task<UserData?> GetUserDataByUserIdAsync(int userId);

    Task UpdateUserDataAsync(UserData data);
}