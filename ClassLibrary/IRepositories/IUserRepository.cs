using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
}

