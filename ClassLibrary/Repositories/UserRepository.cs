using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Users.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.Users.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password, cancellationToken);
    }

    public async Task AddUserDataAsync(UserData data, CancellationToken cancellationToken = default)
    {
        dbContext.UserData.Add(data);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserData?> GetUserDataByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserData
            .AsNoTracking()
            .FirstOrDefaultAsync(ud => ud.UserId == userId, cancellationToken);
    }

    public async Task UpdateUserDataAsync(UserData data, CancellationToken cancellationToken = default)
    {
        dbContext.UserData.Update(data);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
