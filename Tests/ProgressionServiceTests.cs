using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class ProgressionServiceTests
{
    private const double DefaultWeight = 50.0;
    private const int DefaultTargetReps = 10;
    private const int DefaultTemplateExerciseId = 1;

    private readonly Mock<IWorkoutTemplateRepository> templateRepo = new();
    private readonly Mock<INotificationRepository> notificationRepo = new();

    private ProgressionService CreateService() => new(this.templateRepo.Object, this.notificationRepo.Object);

    private static EvaluateWorkoutRequestDataTransferObject BuildRequest(
        int clientId,
        List<LoggedExerciseDataTransferObject> exercises) =>
        new()
        {
            ClientId = clientId,
            Exercises = exercises,
        };

    private static LoggedExerciseDataTransferObject BuildExercise(
        int templateExerciseId,
        string name,
        params (int actualReps, float actualWeight)[] sets) =>
        new()
        {
            ExerciseName = name,
            ParentTemplateExerciseId = templateExerciseId,
            Sets = sets.Select((s, i) => new LoggedSetDataTransferObject
            {
                ActualReps = s.actualReps,
                ActualWeight = s.actualWeight,
                TargetReps = DefaultTargetReps,
                TargetWeight = (float)DefaultWeight,
                SetIndex = i,
                SetNumber = i + 1,
            }).ToList(),
        };

    [Fact]
    public async Task EvaluateWorkoutAsync_EmptyExercises_DoesNothing()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>());

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.templateRepo.Verify(
            r => r.GetTemplateExerciseByIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateWorkoutAsync_ExerciseWithEmptySets_SkipsExercise()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>
        {
            new()
            {
                ParentTemplateExerciseId = DefaultTemplateExerciseId,
                ExerciseName = "Bench Press",
                Sets = new List<LoggedSetDataTransferObject>(),
            },
        });

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.templateRepo.Verify(
            r => r.GetTemplateExerciseByIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateWorkoutAsync_TemplateNotFound_SkipsExercise()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>
        {
            BuildExercise(DefaultTemplateExerciseId, "Bench Press", (DefaultTargetReps, (float)DefaultWeight)),
        });

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync((TemplateExercise?)null);

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.templateRepo.Verify(
            r => r.UpdateTemplateExerciseWeightAsync(It.IsAny<int>(), It.IsAny<double>()),
            Times.Never);
        this.notificationRepo.Verify(
            r => r.SaveNotificationAsync(It.IsAny<Notification>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateWorkoutAsync_PerfectPerformance_AppliesProgressiveOverload()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>
        {
            BuildExercise(DefaultTemplateExerciseId, "Bench Press", (DefaultTargetReps, (float)DefaultWeight)),
        });

        var template = new TemplateExercise
        {
            TemplateExerciseId = DefaultTemplateExerciseId,
            TargetReps = DefaultTargetReps,
            TargetWeight = DefaultWeight,
            MuscleGroup = MuscleGroup.CHEST,
        };

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync(template);
        this.templateRepo
            .Setup(r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, DefaultWeight + 2.5))
            .ReturnsAsync(true);

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.templateRepo.Verify(
            r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, DefaultWeight + 2.5),
            Times.Once);
    }

    [Fact]
    public async Task EvaluateWorkoutAsync_LegsExercise_IncrementsBy5Kg()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>
        {
            BuildExercise(DefaultTemplateExerciseId, "Squat", (DefaultTargetReps, (float)DefaultWeight)),
        });
        request.Exercises[0].TargetMuscles = "LEGS";

        var template = new TemplateExercise
        {
            TemplateExerciseId = DefaultTemplateExerciseId,
            TargetReps = DefaultTargetReps,
            TargetWeight = DefaultWeight,
            MuscleGroup = MuscleGroup.LEGS,
        };

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync(template);
        this.templateRepo
            .Setup(r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, DefaultWeight + 5.0))
            .ReturnsAsync(true);

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.templateRepo.Verify(
            r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, DefaultWeight + 5.0),
            Times.Once);
    }

    [Fact]
    public async Task EvaluateWorkoutAsync_PlateauDetected_SavesNotification()
    {
        var request = BuildRequest(1, new List<LoggedExerciseDataTransferObject>
        {
            BuildExercise(DefaultTemplateExerciseId, "Bench Press", (5, (float)DefaultWeight), (5, (float)DefaultWeight)),
        });

        var template = new TemplateExercise
        {
            TemplateExerciseId = DefaultTemplateExerciseId,
            TargetReps = DefaultTargetReps,
            TargetWeight = DefaultWeight,
            MuscleGroup = MuscleGroup.CHEST,
        };

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync(template);

        var service = this.CreateService();
        await service.EvaluateWorkoutAsync(request);

        this.notificationRepo.Verify(
            r => r.SaveNotificationAsync(It.Is<Notification>(n =>
                n.Type == NotificationType.Plateau &&
                n.RelatedId == DefaultTemplateExerciseId)),
            Times.Once);
    }

    [Fact]
    public async Task ProcessDeloadAsync_TemplateNotFound_ReturnsFalse()
    {
        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync((TemplateExercise?)null);

        var request = new ProcessDeloadRequestDataTransferObject
        {
            RelatedId = DefaultTemplateExerciseId,
        };

        var service = this.CreateService();
        var result = await service.ProcessDeloadAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessDeloadAsync_ValidRequest_UpdatesWeightAndReturnsTrue()
    {
        var template = new TemplateExercise
        {
            TemplateExerciseId = DefaultTemplateExerciseId,
            TargetWeight = 100,
        };

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync(template);
        this.templateRepo
            .Setup(r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, 90.0))
            .ReturnsAsync(true);

        var request = new ProcessDeloadRequestDataTransferObject
        {
            RelatedId = DefaultTemplateExerciseId,
        };

        var service = this.CreateService();
        var result = await service.ProcessDeloadAsync(request);

        Assert.True(result);
        this.templateRepo.Verify(
            r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, 90.0),
            Times.Once);
    }

    [Theory]
    [InlineData(100, 90.0)]
    [InlineData(50, 45.0)]
    [InlineData(33, 29.5)]
    public async Task ProcessDeloadAsync_DeloadWeight_IsRoundedCorrectly(double originalWeight, double expectedDeload)
    {
        var template = new TemplateExercise
        {
            TemplateExerciseId = DefaultTemplateExerciseId,
            TargetWeight = originalWeight,
        };

        this.templateRepo
            .Setup(r => r.GetTemplateExerciseByIdAsync(DefaultTemplateExerciseId))
            .ReturnsAsync(template);
        this.templateRepo
            .Setup(r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, expectedDeload))
            .ReturnsAsync(true);

        var request = new ProcessDeloadRequestDataTransferObject
        {
            RelatedId = DefaultTemplateExerciseId,
        };

        var service = this.CreateService();
        await service.ProcessDeloadAsync(request);

        this.templateRepo.Verify(
            r => r.UpdateTemplateExerciseWeightAsync(DefaultTemplateExerciseId, expectedDeload),
            Times.Once);
    }
}
