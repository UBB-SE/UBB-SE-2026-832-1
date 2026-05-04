using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int clientId);
}
