using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class ClientRepository : IClientRepository
{
    private readonly AppDbContext databaseContext;

    public ClientRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<Client?> GetByIdAsync(int clientId)
    {
        return await this.databaseContext.Clients
            .FirstOrDefaultAsync(client => client.ClientId == clientId);
    }
    public async Task AddAsync(Client client)
    {
        this.databaseContext.Clients.Add(client);

        await this.databaseContext.SaveChangesAsync();
    }
}
