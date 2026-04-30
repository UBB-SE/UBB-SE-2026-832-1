using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IRepositoryClient
{
    Task<Client?> GetByIdAsync(int clientId, CancellationToken cancellationToken = default);
}
