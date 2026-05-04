using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using ClassLibrary.Services;
using WebAPI.IServices;

namespace WebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await this.userRepository.GetAllAsync();

        return users.Select(user => new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Role = user.Role
        }).ToList();
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var user = await this.userRepository.GetByUsernameAndPasswordAsync(username, password);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<UserDto?> RegisterAsync(string username, string password, string role)
    {
        var existing = await this.userRepository.GetByUsernameAndPasswordAsync(username, password);
        if (existing != null) return null;

        var user = new User
        {
            Username = username,
            Password = password,
            Role = role
        };

        await this.userRepository.AddAsync(user);

        return new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        var users = await this.userRepository.GetAllAsync();
        return users.Any(user =>
            string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<UserDataDto?> GetUserDataAsync(int userId)
    {
        var userData = await this.userRepository.GetUserDataByUserIdAsync(userId);
        if (userData == null) return null;

        return MapToUserDataDto(userData, userId);
    }

    public async Task AddUserDataAsync(UserDataDto userDataDto)
    {
        var userData = MapToUserData(userDataDto);
        await this.userRepository.AddUserDataAsync(userData);
    }

    public async Task UpdateUserDataAsync(UserDataDto userDataDto)
    {
        var userData = MapToUserData(userDataDto);
        var computed = NutritionCalculator.ComputeAllNutritionValues(userData);

        await this.userRepository.UpdateUserDataAsync(computed);
    }

    private static UserDataDto MapToUserDataDto(UserData data, int userId)
    {
        return new UserDataDto
        {
            UserDataId = data.UserDataId,
            UserId = userId,
            Weight = data.Weight,
            Height = data.Height,
            Age = data.Age,
            Gender = data.Gender,
            Goal = data.Goal,
            BodyMassIndex = data.BodyMassIndex,
            CalorieNeeds = data.CalorieNeeds,
            ProteinNeeds = data.ProteinNeeds,
            CarbohydrateNeeds = data.CarbohydrateNeeds,
            FatNeeds = data.FatNeeds
        };
    }

    private static UserData MapToUserData(UserDataDto dto)
    {
        return new UserData
        {
            UserDataId = dto.UserDataId,
            User = new User { UserId = dto.UserId },
            Weight = dto.Weight,
            Height = dto.Height,
            Age = dto.Age,
            Gender = dto.Gender,
            Goal = dto.Goal,
            BodyMassIndex = dto.BodyMassIndex,
            CalorieNeeds = dto.CalorieNeeds,
            ProteinNeeds = dto.ProteinNeeds,
            CarbohydrateNeeds = dto.CarbohydrateNeeds,
            FatNeeds = dto.FatNeeds
        };
    }
}