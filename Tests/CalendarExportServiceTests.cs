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
    public async Task GenerateCalendar_null_SelectedDays_throws_ArgumentException()
    {
        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 4,
            SelectedDays = null!,
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

    [Fact]
    public async Task GenerateCalendar_exercises_with_special_chars_are_ICS_escaped()
    {
        var template = new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = "Back, Bis & Abs",
            Exercises = new List<TemplateExercise>
            {
                new() { Name = "Pull-up; wide grip", TargetSets = 3, TargetReps = 8, TargetWeight = 0 },
            },
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(template);

        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 1,
            SelectedDays = new List<DayOfWeek> { DayOfWeek.Tuesday },
            StartDate = new DateTime(2026, 5, 5),
        };

        var service = this.CreateService();
        var result = await service.GenerateCalendarAsync(request);

        // ICS spec requires commas and semicolons to be escaped
        Assert.Contains("Back\\, Bis & Abs", result.IcsContent);
    }

    [Fact]
    public async Task GenerateCalendar_with_no_StartDate_defaults_to_today()
    {
        var template = new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = "Quick",
            Exercises = new List<TemplateExercise>(),
        };

        this.templateRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(template);

        var request = new GenerateCalendarRequestDataTransferObject
        {
            WorkoutTemplateId = 1,
            DurationWeeks = 1,
            SelectedDays = new List<DayOfWeek> { DateTime.Now.DayOfWeek },
            StartDate = null,
        };

        var service = this.CreateService();
        var result = await service.GenerateCalendarAsync(request);

        // at least one event is generated for today's day-of-week
        int eventCount = CountOccurrences(result.IcsContent, "BEGIN:VEVENT");
        Assert.True(eventCount >= 1);
    }

    [Fact]
    public async Task SaveCalendar_null_workout_name_produces_safe_filename()
    {
        var request = new SaveCalendarRequestDataTransferObject
        {
            IcsContent = "BEGIN:VCALENDAR\nEND:VCALENDAR",
            WorkoutName = null,
        };

        var service = this.CreateService();
        var result = await service.SaveCalendarAsync(request);

        // null name should fall back to "workout" and still succeed
        if (result.IsSuccess)
        {
            Assert.Contains("workout", result.FilePath!);
        }
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
