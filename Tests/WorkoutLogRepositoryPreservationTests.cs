using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public sealed class WorkoutLogRepositoryPreservationTests : IDisposable
{
    private readonly AppDbContext context;
    private readonly WorkoutLogRepository repository;

    public WorkoutLogRepositoryPreservationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"WorkoutPreservationTestDb_{Guid.NewGuid()}")
            .Options;

        this.context = new AppDbContext(options);
        this.repository = new WorkoutLogRepository(this.context);
    }

    [Fact]
    public async Task GetWorkoutHistoryAsync_WithExistingWorkouts_ReturnsWorkoutsWithIncludes()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        var exercise1 = new LoggedExercise
        {
            LoggedExerciseId = 1,
            ExerciseName = "Bench Press",
            Sets = new List<LoggedSet>
            {
                new LoggedSet { LoggedSetId = 1, ExerciseName = "Bench Press", ActualReps = 10, ActualWeight = 100 },
                new LoggedSet { LoggedSetId = 2, ExerciseName = "Bench Press", ActualReps = 8, ActualWeight = 110 }
            }
        };

        var exercise2 = new LoggedExercise
        {
            LoggedExerciseId = 2,
            ExerciseName = "Squats",
            Sets = new List<LoggedSet>
            {
                new LoggedSet { LoggedSetId = 3, ExerciseName = "Squats", ActualReps = 12, ActualWeight = 150 }
            }
        };

        var workoutLog = new WorkoutLog
        {
            WorkoutLogId = 1,
            Client = client,
            WorkoutName = "Upper Body",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(45),
            Type = WorkoutType.PREBUILT,
            Exercises = new List<LoggedExercise> { exercise1, exercise2 },
            TotalCaloriesBurned = 300,
            AverageMetabolicEquivalent = 7.0f,
            IntensityTag = "High",
            Rating = 4.5,
            TrainerNotes = "Great form"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        this.context.WorkoutLogs.Add(workoutLog);
        await this.context.SaveChangesAsync();

        var result = await this.repository.GetWorkoutHistoryAsync(clientId: 1);

        Assert.NotNull(result);
        Assert.Single(result);
        
        var workout = result[0];
        Assert.Equal("Upper Body", workout.WorkoutName);
        Assert.NotNull(workout.Client);
        Assert.Equal(1, workout.Client.ClientId);
        Assert.NotNull(workout.Exercises);
        Assert.Equal(2, workout.Exercises.Count);
        
        Assert.Equal(2, workout.Exercises[0].Sets.Count);
        Assert.Single(workout.Exercises[1].Sets);
    }

    [Fact]
    public async Task GetWorkoutHistoryAsync_WithNoWorkouts_ReturnsEmptyList()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        await this.context.SaveChangesAsync();

        var result = await this.repository.GetWorkoutHistoryAsync(clientId: 1);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWorkoutHistoryAsync_WithMultipleWorkouts_ReturnsInDescendingDateOrder()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        var workout1 = new WorkoutLog
        {
            WorkoutLogId = 1,
            Client = client,
            WorkoutName = "Workout 1",
            Date = DateTime.Today.AddDays(-2),
            Duration = TimeSpan.FromMinutes(30),
            Type = WorkoutType.CUSTOM,
            TotalCaloriesBurned = 200,
            AverageMetabolicEquivalent = 6.0f,
            IntensityTag = "Moderate",
            Rating = 4.0,
            TrainerNotes = ""
        };

        var workout2 = new WorkoutLog
        {
            WorkoutLogId = 2,
            Client = client,
            WorkoutName = "Workout 2",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(45),
            Type = WorkoutType.PREBUILT,
            TotalCaloriesBurned = 300,
            AverageMetabolicEquivalent = 7.0f,
            IntensityTag = "High",
            Rating = 5.0,
            TrainerNotes = ""
        };

        var workout3 = new WorkoutLog
        {
            WorkoutLogId = 3,
            Client = client,
            WorkoutName = "Workout 3",
            Date = DateTime.Today.AddDays(-1),
            Duration = TimeSpan.FromMinutes(40),
            Type = WorkoutType.CUSTOM,
            TotalCaloriesBurned = 250,
            AverageMetabolicEquivalent = 6.5f,
            IntensityTag = "Moderate",
            Rating = 4.5,
            TrainerNotes = ""
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        this.context.WorkoutLogs.AddRange(workout1, workout2, workout3);
        await this.context.SaveChangesAsync();

        var result = await this.repository.GetWorkoutHistoryAsync(clientId: 1);

        Assert.Equal(3, result.Count);
        Assert.Equal("Workout 2", result[0].WorkoutName);
        Assert.Equal("Workout 3", result[1].WorkoutName);
        Assert.Equal("Workout 1", result[2].WorkoutName);
    }

    [Fact]
    public async Task GetWorkoutHistoryAsync_WithMultipleClients_ReturnsOnlyClientWorkouts()
    {
        var user1 = new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            FullName = "User One",
            Password = "password123",
            Role = "User"
        };

        var user2 = new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            FullName = "User Two",
            Password = "password123",
            Role = "User"
        };

        var client1 = new Client
        {
            ClientId = 1,
            User = user1,
            Email = "user1@example.com",
            FullName = "User One",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        var client2 = new Client
        {
            ClientId = 2,
            User = user2,
            Email = "user2@example.com",
            FullName = "User Two",
            Weight = 70.0,
            Height = 175.0,
            PrimaryGoal = "Lose Weight"
        };

        var workoutForClient1 = new WorkoutLog
        {
            WorkoutLogId = 1,
            Client = client1,
            WorkoutName = "Client 1 Workout",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(30),
            Type = WorkoutType.CUSTOM,
            TotalCaloriesBurned = 200,
            AverageMetabolicEquivalent = 6.0f,
            IntensityTag = "Moderate",
            Rating = 4.0,
            TrainerNotes = ""
        };

        var workoutForClient2 = new WorkoutLog
        {
            WorkoutLogId = 2,
            Client = client2,
            WorkoutName = "Client 2 Workout",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(45),
            Type = WorkoutType.PREBUILT,
            TotalCaloriesBurned = 300,
            AverageMetabolicEquivalent = 7.0f,
            IntensityTag = "High",
            Rating = 5.0,
            TrainerNotes = ""
        };

        this.context.Users.AddRange(user1, user2);
        this.context.Clients.AddRange(client1, client2);
        this.context.WorkoutLogs.AddRange(workoutForClient1, workoutForClient2);
        await this.context.SaveChangesAsync();

        var client1Workouts = await this.repository.GetWorkoutHistoryAsync(clientId: 1);
        var client2Workouts = await this.repository.GetWorkoutHistoryAsync(clientId: 2);

        Assert.Single(client1Workouts);
        Assert.Equal("Client 1 Workout", client1Workouts[0].WorkoutName);
        
        Assert.Single(client2Workouts);
        Assert.Equal("Client 2 Workout", client2Workouts[0].WorkoutName);
    }

    [Fact]
    public async Task SaveWorkoutLogAsync_WithValidLog_SavesLogSuccessfully()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        await this.context.SaveChangesAsync();

        var workoutLog = new WorkoutLog
        {
            Client = client,
            WorkoutName = "Morning Run",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(30),
            Type = WorkoutType.CUSTOM,
            TotalCaloriesBurned = 300,
            AverageMetabolicEquivalent = 8.0f,
            IntensityTag = "Moderate",
            Rating = 4.5,
            TrainerNotes = "Good pace"
        };

        await this.repository.SaveWorkoutLogAsync(workoutLog);

        var savedLog = await this.context.WorkoutLogs.FirstOrDefaultAsync();
        Assert.NotNull(savedLog);
        Assert.Equal("Morning Run", savedLog.WorkoutName);
        Assert.Equal(300, savedLog.TotalCaloriesBurned);
        Assert.Equal(8.0f, savedLog.AverageMetabolicEquivalent);
        Assert.Equal("Moderate", savedLog.IntensityTag);
        Assert.Equal(4.5, savedLog.Rating);
        Assert.Equal("Good pace", savedLog.TrainerNotes);
    }

    [Fact]
    public async Task SaveWorkoutLogAsync_WithExercisesAndSets_SavesAllDataSuccessfully()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        await this.context.SaveChangesAsync();

        var workoutLog = new WorkoutLog
        {
            Client = client,
            WorkoutName = "Strength Training",
            Date = DateTime.Today,
            Duration = TimeSpan.FromMinutes(60),
            Type = WorkoutType.PREBUILT,
            Exercises = new List<LoggedExercise>
            {
                new LoggedExercise
                {
                    ExerciseName = "Bench Press",
                    Sets = new List<LoggedSet>
                    {
                        new LoggedSet { ExerciseName = "Bench Press", ActualReps = 10, ActualWeight = 100 },
                        new LoggedSet { ExerciseName = "Bench Press", ActualReps = 8, ActualWeight = 110 }
                    }
                }
            },
            TotalCaloriesBurned = 400,
            AverageMetabolicEquivalent = 7.5f,
            IntensityTag = "High",
            Rating = 5.0,
            TrainerNotes = "Excellent session"
        };

        await this.repository.SaveWorkoutLogAsync(workoutLog);

        var savedLog = await this.context.WorkoutLogs
            .Include(w => w.Exercises)
            .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync();
        
        Assert.NotNull(savedLog);
        Assert.Single(savedLog.Exercises);
        Assert.Equal("Bench Press", savedLog.Exercises[0].ExerciseName);
        Assert.Equal(2, savedLog.Exercises[0].Sets.Count);
    }

    [Fact]
    public async Task SaveWorkoutLogAsync_WithNullLog_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            this.repository.SaveWorkoutLogAsync(null!));
    }

    [Fact]
    public async Task GetClientWeightAsync_WithExistingClient_ReturnsCorrectWeight()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 82.5,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        await this.context.SaveChangesAsync();

        var weight = await this.repository.GetClientWeightAsync(clientId: 1);

        Assert.Equal(82.5, weight);
    }

    [Fact]
    public async Task GetClientWeightAsync_WithNonExistentClient_ReturnsZero()
    {
        var weight = await this.repository.GetClientWeightAsync(clientId: 999);

        Assert.Equal(0, weight);
    }

    [Fact]
    public async Task GetTotalCaloriesBurnedForRangeAsync_WithNoLogsInRange_ReturnsZero()
    {
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Password = "password123",
            Role = "User"
        };

        var client = new Client
        {
            ClientId = 1,
            User = user,
            Email = "test@example.com",
            FullName = "Test User",
            Weight = 75.0,
            Height = 180.0,
            PrimaryGoal = "Maintain"
        };

        this.context.Users.Add(user);
        this.context.Clients.Add(client);
        await this.context.SaveChangesAsync();

        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today.AddDays(-6);

        var result = await this.repository.GetTotalCaloriesBurnedForRangeAsync(userId: 1, startDate, endDate);

        Assert.Equal(0.0, result);
    }

    public void Dispose()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }
}
