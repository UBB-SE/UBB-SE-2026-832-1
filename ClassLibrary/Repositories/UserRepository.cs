using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == id, cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        this.databaseContext.Users.Add(entity);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        this.databaseContext.Users.Update(entity);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await this.databaseContext.Users.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            this.databaseContext.Users.Remove(entity);
            await this.databaseContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == username && user.Password == password, cancellationToken);
    }

    public async Task AddUserDataAsync(UserData data, CancellationToken cancellationToken = default)
    {
        this.databaseContext.UserData.Add(data);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserData?> GetUserDataByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.UserData
            .AsNoTracking()
            .FirstOrDefaultAsync(userData => userData.User.UserId == userId, cancellationToken);
    }

    public async Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default)
    {
        this.databaseContext.UserData.Update(data);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }
}
