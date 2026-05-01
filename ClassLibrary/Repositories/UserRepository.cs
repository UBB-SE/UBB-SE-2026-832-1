using System.Collections.Generic;
using System.Linq;
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

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task AddAsync(User entity)
    {
        this.databaseContext.Users.Add(entity);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        this.databaseContext.Users.Update(entity);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this.databaseContext.Users.FindAsync([id]);
        if (entity is not null)
        {
            this.databaseContext.Users.Remove(entity);
            await this.databaseContext.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await this.databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == username && user.Password == password);
    }

    public async Task AddUserDataAsync(UserData data)
    {
        this.databaseContext.UserData.Add(data);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task<UserData?> GetUserDataByUserIdAsync(int userId)
    {
        return await this.databaseContext.UserData
            .AsNoTracking()
            .FirstOrDefaultAsync(userData => userData.User.UserId == userId);
    }

    public async Task UpdateUserDataAsync(UserData data)
    {
        this.databaseContext.UserData.Update(data);
        await this.databaseContext.SaveChangesAsync();
    }
}
