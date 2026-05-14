using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class CalendarExportServiceTests
{
    private readonly Mock<IWorkoutTemplateRepository> templateRepo = new();

    private CalendarExportService CreateService() => new(this.templateRepo.Object);

    [Fact]
    public async Task GenerateCalendarAsync_TemplateNotFound_ThrowsArgumentNullException()
    {
        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((WorkoutTemplate?)null);

        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 4,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Monday },
        };

        var service = this.CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.GenerateCalendarAsync(request));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(53)]
    public async Task GenerateCalendarAsync_InvalidDurationWeeks_ThrowsArgumentOutOfRange(int invalidWeeks)
    {
        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = invalidWeeks,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Monday },
        };

        var service = this.CreateService();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GenerateCalendarAsync(request));
    }

    [Fact]
    public async Task GenerateCalendarAsync_NoDaysSelected_ThrowsArgumentException()
    {
        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 4,
            SelectedDays = new List<DayOfWeek>(),
        };

        var service = this.CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.GenerateCalendarAsync(request));
    }

    [Fact]
    public async Task GenerateCalendarAsync_ValidTemplate_GeneratesCorrectEventCount()
    {
        var template = new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = "Full Body",
            Exercises = new List<TemplateExercise>
            {
                new() { Name = "Squat", TargetSets = 3, TargetReps = 10, TargetWeight = 60 },
            },
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(template);

        var startDate = new DateTime(2026, 5, 5); // Monday
        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 3,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            StartDate = startDate,
        };

        var service = this.CreateService();
        var result = await service.GenerateCalendarAsync(request);

        int eventCount = CountOccurrences(result.IcsContent, "BEGIN:VEVENT");
        Assert.Equal(9, eventCount);
    }

    [Fact]
    public async Task GenerateCalendarAsync_IncludesExerciseDescription()
    {
        var template = new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = "Strength",
            Exercises = new List<TemplateExercise>
            {
                new() { Name = "Deadlift", TargetSets = 5, TargetReps = 5, TargetWeight = 100 },
            },
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(template);

        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 1,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Monday },
            StartDate = new DateTime(2026, 5, 5),
        };

        var service = this.CreateService();
        var result = await service.GenerateCalendarAsync(request);

        Assert.Contains("Deadlift: 5x5 @ 100kg", result.IcsContent);
    }

    [Fact]
    public async Task SaveCalendarAsync_EmptyContent_ReturnsFailure()
    {
        var request = new SaveCalendarRequestDataTransferObject
        {
            IcsContent = string.Empty,
            WorkoutName = "Test",
        };

        var service = this.CreateService();
        var result = await service.SaveCalendarAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Null(result.FilePath);
    }

    [Fact]
    public async Task GenerateCalendarAsync_ContainsVCalendarEnvelope()
    {
        var template = new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = "Test",
            Exercises = new List<TemplateExercise>(),
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(template);

        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 1,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Monday },
            StartDate = new DateTime(2026, 5, 5),
        };

        var service = this.CreateService();
        var result = await service.GenerateCalendarAsync(request);

        Assert.Contains("BEGIN:VCALENDAR", result.IcsContent);
        Assert.Contains("END:VCALENDAR", result.IcsContent);
    }

    private static int CountOccurrences(string text, string pattern)
    {
        int count = 0;
        int position = 0;
        while ((position = text.IndexOf(pattern, position, StringComparison.Ordinal)) != -1)
        {
            position += pattern.Length;
            count++;
        }

        return count;
    }
}
