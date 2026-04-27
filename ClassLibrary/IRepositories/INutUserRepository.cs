using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface INutUserRepository
{
    Task AddAsync(NutUser entity, CancellationToken cancellationToken = default);

    Task AddUserDataAsync(UserData data, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NutUser>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<NutUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<NutUser?> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<UserData?> GetUserDataByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task UpdateAsync(NutUser entity, CancellationToken cancellationToken = default);

    Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default);
}
