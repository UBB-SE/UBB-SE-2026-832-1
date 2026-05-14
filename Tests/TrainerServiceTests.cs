using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class TrainerServiceTests
{
    private readonly Mock<ITrainerRepository> trainerRepo = new();
    private readonly Mock<IWorkoutTemplateRepository> templateRepo = new();
    private readonly Mock<IWorkoutLogRepository> workoutLogRepo = new();
    private readonly Mock<INutritionRepository> nutritionRepo = new();

    private TrainerService CreateService() => new(
        this.trainerRepo.Object,
        this.templateRepo.Object,
        this.workoutLogRepo.Object,
        this.nutritionRepo.Object);

    [Fact]
    public async Task SaveWorkoutFeedbackAsync_ValidRequest_ReturnsTrue()
    {
        var request = new SaveWorkoutFeedbackRequestDataTransferObject
        {
            WorkoutLogId = 10,
            Rating = 5,
            TrainerNotes = "Good job",
        };

        var service = this.CreateService();
        var result = await service.SaveWorkoutFeedbackAsync(request);

        Assert.True(result);
        this.workoutLogRepo.Verify(r => r.UpdateWorkoutLogAsync(It.Is<WorkoutLog>(
            l => l.WorkoutLogId == 10 && l.Rating == 5)), Times.Once);
    }

    [Fact]
    public async Task SaveWorkoutFeedbackAsync_LogNotFound_ReturnsFalse()
    {
        this.workoutLogRepo
            .Setup(r => r.UpdateWorkoutLogAsync(It.IsAny<WorkoutLog>()))
            .ThrowsAsync(new KeyNotFoundException());

        var request = new SaveWorkoutFeedbackRequestDataTransferObject
        {
            WorkoutLogId = 999,
            Rating = 3,
        };

        var service = this.CreateService();
        var result = await service.SaveWorkoutFeedbackAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task AssignNewRoutineAsync_NewTemplate_SavesAndReturnsSuccess()
    {
        var request = new AssignNewRoutineRequestDataTransferObject
        {
            EditingTemplateId = null,
            ClientId = 1,
            RoutineName = "Leg Day",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new() { Name = "Squat", MuscleGroup = "LEGS", TargetSets = 3, TargetReps = 10, TargetWeight = 60 },
            },
        };

        var service = this.CreateService();
        var (success, error) = await service.AssignNewRoutineAsync(request);

        Assert.True(success);
        Assert.Equal(string.Empty, error);
        this.trainerRepo.Verify(r => r.SaveTrainerWorkoutAsync(
            It.Is<WorkoutTemplate>(t => t.Name == "Leg Day")), Times.Once);
    }

    [Fact]
    public async Task AssignNewRoutineAsync_EditTemplateNotFound_ReturnsError()
    {
        this.templateRepo
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((WorkoutTemplate?)null);

        var request = new AssignNewRoutineRequestDataTransferObject
        {
            EditingTemplateId = 99,
            ClientId = 1,
            RoutineName = "Updated Routine",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new() { Name = "Pushup", MuscleGroup = "CHEST" },
            },
        };

        var service = this.CreateService();
        var (success, error) = await service.AssignNewRoutineAsync(request);

        Assert.False(success);
        Assert.Equal("Template not found.", error);
    }

    [Fact]
    public async Task AssignNewRoutineAsync_EditExistingTemplate_UpdatesAndReturnsSuccess()
    {
        var existingTemplate = new WorkoutTemplate
        {
            WorkoutTemplateId = 5,
            Name = "Old Name",
            Exercises = new List<TemplateExercise>(),
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(existingTemplate);

        var request = new AssignNewRoutineRequestDataTransferObject
        {
            EditingTemplateId = 5,
            ClientId = 1,
            RoutineName = "Updated Routine",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new() { Name = "Deadlift", MuscleGroup = "BACK" },
            },
        };

        var service = this.CreateService();
        var (success, _) = await service.AssignNewRoutineAsync(request);

        Assert.True(success);
        Assert.Equal("Updated Routine", existingTemplate.Name);
        this.trainerRepo.Verify(r => r.SaveTrainerWorkoutAsync(existingTemplate), Times.Once);
    }

    [Fact]
    public async Task AssignNutritionPlanAsync_ValidRequest_ReturnsTrue()
    {
        var request = new AssignNutritionPlanRequestDataTransferObject
        {
            ClientId = 5,
            NutritionPlanId = 1,
        };

        var service = this.CreateService();
        var result = await service.AssignNutritionPlanAsync(request);

        Assert.True(result);
        this.nutritionRepo.Verify(
            r => r.AssignNutritionPlanToClientAsync(5, 1), Times.Once);
    }

    [Fact]
    public async Task AssignNutritionPlanAsync_RepositoryThrows_ReturnsFalse()
    {
        this.nutritionRepo
            .Setup(r => r.AssignNutritionPlanToClientAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var request = new AssignNutritionPlanRequestDataTransferObject
        {
            ClientId = 5,
            NutritionPlanId = 1,
        };

        var service = this.CreateService();
        var result = await service.AssignNutritionPlanAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAndAssignNutritionPlanAsync_ValidRequest_InsertsAndAssigns()
    {
        var request = new CreateNutritionPlanRequestDataTransferObject
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 7),
            ClientId = 99,
        };

        var service = this.CreateService();
        var result = await service.CreateAndAssignNutritionPlanAsync(request);

        Assert.True(result);
        this.nutritionRepo.Verify(r => r.InsertNutritionPlanAsync(
            It.Is<NutritionPlan>(p =>
                p.StartDate == new DateTime(2026, 1, 1) &&
                p.EndDate == new DateTime(2026, 1, 7))), Times.Once);
    }

    [Fact]
    public async Task CreateAndAssignNutritionPlanAsync_RepositoryThrows_ReturnsFalse()
    {
        this.nutritionRepo
            .Setup(r => r.InsertNutritionPlanAsync(It.IsAny<NutritionPlan>()))
            .ThrowsAsync(new Exception("db error"));

        var request = new CreateNutritionPlanRequestDataTransferObject
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7),
            ClientId = 1,
        };

        var service = this.CreateService();
        var result = await service.CreateAndAssignNutritionPlanAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task GetClientWorkoutHistoryAsync_ReturnsWorkoutLogDtos()
    {
        var logs = new List<WorkoutLog>
        {
            new()
            {
                WorkoutLogId = 1,
                WorkoutName = "Upper Body",
                Client = new Client { ClientId = 1 },
                Exercises = new List<LoggedExercise>(),
            },
        };

        this.workoutLogRepo
            .Setup(r => r.GetWorkoutHistoryAsync(1))
            .ReturnsAsync(logs);

        var service = this.CreateService();
        var result = await service.GetClientWorkoutHistoryAsync(1);

        Assert.Single(result);
        Assert.Equal("Upper Body", result[0].WorkoutName);
    }

    [Fact]
    public async Task GetAvailableWorkoutsAsync_ReturnsTemplateDtos()
    {
        var templates = new List<WorkoutTemplate>
        {
            new()
            {
                WorkoutTemplateId = 1,
                Name = "Full Body",
                Client = new Client { ClientId = 1 },
                Exercises = new List<TemplateExercise>(),
            },
        };

        this.templateRepo
            .Setup(r => r.GetAvailableWorkoutsAsync(1))
            .ReturnsAsync(templates);

        var service = this.CreateService();
        var result = await service.GetAvailableWorkoutsAsync(1);

        Assert.Single(result);
        Assert.Equal("Full Body", result[0].Name);
    }

    [Fact]
    public async Task DeleteWorkoutTemplateAsync_ExistingTemplate_ReturnsTrue()
    {
        var service = this.CreateService();
        var result = await service.DeleteWorkoutTemplateAsync(1);

        Assert.True(result);
        this.trainerRepo.Verify(r => r.DeleteWorkoutTemplateAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteWorkoutTemplateAsync_NotFound_ReturnsFalse()
    {
        this.trainerRepo
            .Setup(r => r.DeleteWorkoutTemplateAsync(999))
            .ThrowsAsync(new KeyNotFoundException());

        var service = this.CreateService();
        var result = await service.DeleteWorkoutTemplateAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveTrainerWorkoutAsync_ValidDto_ReturnsTrue()
    {
        var dto = new WorkoutTemplateDataTransferObject
        {
            WorkoutTemplateId = 0,
            ClientId = 1,
            Name = "Push Day",
            Exercises = new List<TemplateExerciseDataTransferObject>
            {
                new() { Name = "Bench Press", MuscleGroup = "CHEST", TargetSets = 3, TargetReps = 8, TargetWeight = 80 },
            },
        };

        var service = this.CreateService();
        var result = await service.SaveTrainerWorkoutAsync(dto);

        Assert.True(result);
        this.trainerRepo.Verify(r => r.SaveTrainerWorkoutAsync(
            It.Is<WorkoutTemplate>(t => t.Name == "Push Day")), Times.Once);
    }

    [Fact]
    public async Task GetAssignedClientsAsync_ReturnsClientDtos()
    {
        var clients = new List<Client>
        {
            new() { ClientId = 1, FullName = "John Doe", Email = "john@test.com" },
        };

        this.trainerRepo
            .Setup(r => r.GetTrainerClientsAsync(10))
            .ReturnsAsync(clients);

        var service = this.CreateService();
        var result = await service.GetAssignedClientsAsync(10);

        Assert.Single(result);
        Assert.Equal("John Doe", result[0].FullName);
    }
}
