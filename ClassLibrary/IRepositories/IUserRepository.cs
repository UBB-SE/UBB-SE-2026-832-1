using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default);

    Task AddAsync(User entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(User entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task AddUserDataAsync(UserData data, CancellationToken cancellationToken = default);

    Task<UserData?> GetUserDataByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default);
}
