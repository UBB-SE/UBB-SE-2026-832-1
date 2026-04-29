using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext databaseContext;

    public UserRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

