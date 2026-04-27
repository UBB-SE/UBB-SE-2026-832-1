using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class NutUserRepository(AppDbContext dbContext) : INutUserRepository
{
    public async Task<NutUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.NutUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NutUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.NutUsers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NutUser entity, CancellationToken cancellationToken = default)
    {
        dbContext.NutUsers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(NutUser entity, CancellationToken cancellationToken = default)
    {
        dbContext.NutUsers.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.NutUsers.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.NutUsers.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<NutUser?> GetByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        return await dbContext.NutUsers
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
