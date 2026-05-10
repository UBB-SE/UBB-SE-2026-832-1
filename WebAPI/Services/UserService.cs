using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using ClassLibrary.Services;
using WebAPI.IServices;

namespace WebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IClientRepository clientRepository;
    public UserService(IUserRepository userRepository, IClientRepository clientRepository)
    {
        this.userRepository = userRepository;
        this.clientRepository = clientRepository;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await this.userRepository.GetAllAsync();

        return users
            .Select(user => new UserDto
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role,
            })
            .ToList();
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var user = await this.userRepository.GetByUsernameAndPasswordAsync(username, password);
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Role = user.Role,
        };
    }

    public async Task<UserDto?> RegisterAsync(string username, string password, string role)
    {
        var existing = await this.userRepository.GetByUsernameAndPasswordAsync(username, password);
        if (existing != null)
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            Password = password,
            Role = role,
        };

        await this.userRepository.AddAsync(user);
        if (role == "Client")
        {
            var client = new Client
            {
                ClientId = user.UserId
            };
            client.WorkoutTemplates = CreateDefaultWorkoutTemplates(client);

            await this.clientRepository.AddAsync(client);
        }

        return new UserDto
        {
            Id = user.UserId,
            Username = user.Username,
            Role = user.Role,
        };
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        var users = await this.userRepository.GetAllAsync();
        return users.Any(user => string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<UserDataDto?> GetUserDataAsync(int userId)
    {
        var userData = await this.userRepository.GetUserDataByUserIdAsync(userId);
        if (userData == null)
        {
            return null;
        }

        return MapToUserDataDto(userData, userId);
    }

    public async Task AddUserDataAsync(UserDataDto userDataDto)
    {
        var user =
            await this.userRepository.GetByIdAsync(
                userDataDto.UserId);

        if (user == null)
        {
            return;
        }

        var userData =
            MapToUserData(
                userDataDto,
                user);

        var computedData =
            NutritionCalculator.ComputeAllNutritionValues(
                userData);

        await this.userRepository.AddUserDataAsync(
            computedData);
    }

    public async Task UpdateUserDataAsync(UserDataDto userDataDto)
    {
        var user = await this.userRepository.GetByIdAsync(userDataDto.UserId);

        if (user == null)
        {
            return;
        }

        var userData = MapToUserData(userDataDto, user);

        var computedData = NutritionCalculator.ComputeAllNutritionValues(userData);

        await this.userRepository.UpdateUserDataAsync(computedData);
    }

    private static UserDataDto MapToUserDataDto(UserData userData, int userId)
    {
        return new UserDataDto
        {
            UserDataId = userData.UserDataId,
            UserId = userId,
            Weight = userData.Weight,
            Height = userData.Height,
            Age = userData.Age,
            Gender = userData.Gender,
            Goal = userData.Goal,
            BodyMassIndex = userData.BodyMassIndex,
            CalorieNeeds = userData.CalorieNeeds,
            ProteinNeeds = userData.ProteinNeeds,
            CarbohydrateNeeds = userData.CarbohydrateNeeds,
            FatNeeds = userData.FatNeeds,
        };
    }

    private static UserData MapToUserData(UserDataDto dto, User user)
    {
        return new UserData
        {
            UserDataId = dto.UserDataId,
            User = user,
            Weight = dto.Weight,
            Height = dto.Height,
            Age = dto.Age,
            Gender = dto.Gender,
            Goal = dto.Goal,
            BodyMassIndex = dto.BodyMassIndex,
            CalorieNeeds = dto.CalorieNeeds,
            ProteinNeeds = dto.ProteinNeeds,
            CarbohydrateNeeds = dto.CarbohydrateNeeds,
            FatNeeds = dto.FatNeeds,
        };
    }
    private static List<WorkoutTemplate> CreateDefaultWorkoutTemplates(
    Client client)
    {
        var pushTemplate = new WorkoutTemplate
        {
            Client = client,
            Name = "Push Day",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
        {
            new TemplateExercise
            {
                Name = "Bench Press",
                MuscleGroup = MuscleGroup.CHEST,
                TargetSets = 4,
                TargetReps = 10
            },

            new TemplateExercise
            {
                Name = "Incline Dumbbell Press",
                MuscleGroup = MuscleGroup.CHEST,
                TargetSets = 3,
                TargetReps = 12
            },

            new TemplateExercise
            {
                Name = "Shoulder Press",
                MuscleGroup = MuscleGroup.SHOULDERS,
                TargetSets = 3,
                TargetReps = 10
            }
        }
        };

        var pullTemplate = new WorkoutTemplate
        {
            Client = client,
            Name = "Pull Day",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
        {
            new TemplateExercise
            {
                Name = "Pull Ups",
                MuscleGroup = MuscleGroup.BACK,
                TargetSets = 4,
                TargetReps = 10
            },

            new TemplateExercise
            {
                Name = "Barbell Row",
                MuscleGroup = MuscleGroup.BACK,
                TargetSets = 4,
                TargetReps = 10
            },

            new TemplateExercise
            {
                Name = "Bicep Curl",
                MuscleGroup = MuscleGroup.ARMS,
                TargetSets = 3,
                TargetReps = 12
            }
        }
        };

        var hiitTemplate = new WorkoutTemplate
        {
            Client = client,
            Name = "HIIT Fat Burner",
            Type = WorkoutType.PREBUILT,
            Exercises = new List<TemplateExercise>
        {
            new TemplateExercise
            {
                Name = "Burpees",
                MuscleGroup = MuscleGroup.CARDIO,
                TargetSets = 4,
                TargetReps = 15
            },

            new TemplateExercise
            {
                Name = "Mountain Climbers",
                MuscleGroup = MuscleGroup.CARDIO,
                TargetSets = 4,
                TargetReps = 20
            },

            new TemplateExercise
            {
                Name = "Jump Squats",
                MuscleGroup = MuscleGroup.LEGS,
                TargetSets = 4,
                TargetReps = 15
            }
        }
        };

        return new List<WorkoutTemplate>
    {
        pushTemplate,
        pullTemplate,
        hiitTemplate
    };
    }
}