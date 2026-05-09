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
        return await databaseContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task AddAsync(User entity)
    {
        databaseContext.Users.Add(entity);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        databaseContext.Users.Update(entity);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await databaseContext.Users.FindAsync(id);

        if (entity is not null)
        {
            databaseContext.Users.Remove(entity);
            await databaseContext.SaveChangesAsync();
        }
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == username && user.Password == password);
    }

    public async Task AddUserDataAsync(UserData data)
    {
        databaseContext.UserData.Add(data);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<UserData?> GetUserDataByUserIdAsync(int userId)
    {
        return await databaseContext.UserData
            .AsNoTracking()
            .FirstOrDefaultAsync(userData => userData.User.UserId == userId);
    }

    public async Task UpdateUserDataAsync(UserData data)
    {
        var existing = await databaseContext.UserData.FindAsync(data.UserDataId);
        if (existing == null)
        {
            return;
        }

        existing.Weight = data.Weight;
        existing.Height = data.Height;
        existing.Age = data.Age;
        existing.Gender = data.Gender;
        existing.Goal = data.Goal;
        existing.BodyMassIndex = data.BodyMassIndex;
        existing.CalorieNeeds = data.CalorieNeeds;
        existing.ProteinNeeds = data.ProteinNeeds;
        existing.CarbohydrateNeeds = data.CarbohydrateNeeds;
        existing.FatNeeds = data.FatNeeds;

        await databaseContext.SaveChangesAsync();
    }
}