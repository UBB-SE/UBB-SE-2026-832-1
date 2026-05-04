using System.Text;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class CalendarExportService : ICalendarExportService
{
    private const int MIN_DURATION_WEEKS = 1;
    private const int MAX_DURATION_WEEKS = 52;
    private const int DAYS_IN_WEEK = 7;
    private const int DEFAULT_START_HOUR = 10;
    private const int DEFAULT_DURATION_HOURS = 1;

    private readonly IWorkoutTemplateRepository workoutTemplateRepository;

    public CalendarExportService(IWorkoutTemplateRepository workoutTemplateRepository)
    {
        this.workoutTemplateRepository = workoutTemplateRepository;
    }

    public async Task<CalendarExportResponseDataTransferObject> GenerateCalendarAsync(GenerateCalendarRequestDataTransferObject request)
    {
        // Validate DurationWeeks
        if (request.DurationWeeks < MIN_DURATION_WEEKS || request.DurationWeeks > MAX_DURATION_WEEKS)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.DurationWeeks),
                $"DurationWeeks must be between {MIN_DURATION_WEEKS} and {MAX_DURATION_WEEKS}.");
        }

        // Validate SelectedDays
        if (request.SelectedDays == null || request.SelectedDays.Count == 0)
        {
            throw new ArgumentException("At least one day must be selected.", nameof(request.SelectedDays));
        }

        // Retrieve workout template
        var workoutTemplate = await this.workoutTemplateRepository.GetByIdAsync(request.WorkoutTemplateId);
        if (workoutTemplate == null)
        {
            throw new ArgumentNullException(nameof(workoutTemplate), "Workout template not found.");
        }

        // Resolve start date
        var startDate = request.StartDate ?? DateTime.Now;

        // Generate VEVENT blocks
        var events = GenerateWorkoutEvents(workoutTemplate, request.DurationWeeks, request.SelectedDays, startDate);

        // Wrap in ICS envelope
        var icsContent = new StringBuilder();
        icsContent.AppendLine("BEGIN:VCALENDAR");
        icsContent.AppendLine("VERSION:2.0");
        icsContent.AppendLine("PRODID:-//CalendarExportService//EN");
        icsContent.AppendLine("CALSCALE:GREGORIAN");
        icsContent.AppendLine("METHOD:PUBLISH");
        icsContent.Append(events);
        icsContent.AppendLine("END:VCALENDAR");

        return new CalendarExportResponseDataTransferObject
        {
            IcsContent = icsContent.ToString()
        };
    }

    public async Task<SaveCalendarResponseDataTransferObject> SaveCalendarAsync(SaveCalendarRequestDataTransferObject request)
    {
        // Validate IcsContent
        if (string.IsNullOrWhiteSpace(request.IcsContent))
        {
            return new SaveCalendarResponseDataTransferObject
            {
                IsSuccess = false,
                FilePath = null
            };
        }

        // Build safe filename
        var safeWorkoutName = BuildSafeWorkoutName(request.WorkoutName);
        var fileName = $"{safeWorkoutName}-{DateTime.Now:yyyyMMdd-HHmmss}.ics";

        // Resolve Downloads folder
        var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";
        var fullPath = Path.Combine(downloadsPath, fileName);

        try
        {
            await File.WriteAllTextAsync(fullPath, request.IcsContent);
            return new SaveCalendarResponseDataTransferObject
            {
                IsSuccess = true,
                FilePath = fullPath
            };
        }
        catch
        {
            return new SaveCalendarResponseDataTransferObject
            {
                IsSuccess = false,
                FilePath = null
            };
        }
    }

    private static string FormatIcsDateTime(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
    }

    private static string EscapeIcsText(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r\n", "\\n")
            .Replace("\r", "\\n");
    }

    private static string BuildSafeWorkoutName(string? workoutName)
    {
        if (string.IsNullOrWhiteSpace(workoutName))
        {
            return "workout";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = workoutName;

        foreach (var invalidChar in invalidChars)
        {
            safeName = safeName.Replace(invalidChar, '-');
        }

        safeName = safeName.Trim('-');

        if (string.IsNullOrEmpty(safeName))
        {
            return "workout";
        }

        return safeName;
    }

    private static string BuildExerciseDescription(ICollection<TemplateExercise> exercises)
    {
        var lines = exercises.Select(e =>
            $"{e.Name}: {e.TargetSets}x{e.TargetReps} @ {e.TargetWeight}kg");

        return string.Join("\\n", lines);
    }

    private static string CreateVirtualEvent(WorkoutTemplate workoutTemplate, DateTime eventDate)
    {
        var eventBuilder = new StringBuilder();
        eventBuilder.AppendLine("BEGIN:VEVENT");
        eventBuilder.AppendLine($"UID:{Guid.NewGuid()}@calendarexport");
        eventBuilder.AppendLine($"DTSTART:{FormatIcsDateTime(eventDate.Date.AddHours(DEFAULT_START_HOUR))}");
        eventBuilder.AppendLine($"DTEND:{FormatIcsDateTime(eventDate.Date.AddHours(DEFAULT_START_HOUR + DEFAULT_DURATION_HOURS))}");
        eventBuilder.AppendLine($"SUMMARY:{EscapeIcsText(workoutTemplate.Name)}");
        eventBuilder.AppendLine($"DESCRIPTION:{BuildExerciseDescription(workoutTemplate.Exercises)}");
        eventBuilder.AppendLine("END:VEVENT");

        return eventBuilder.ToString();
    }

    private static string GenerateWorkoutEvents(
        WorkoutTemplate workoutTemplate,
        int durationWeeks,
        List<DayOfWeek> selectedDays,
        DateTime startDate)
    {
        var eventsBuilder = new StringBuilder();

        for (int week = 0; week < durationWeeks; week++)
        {
            for (int day = 0; day < DAYS_IN_WEEK; day++)
            {
                var currentDate = startDate.AddDays(week * DAYS_IN_WEEK + day);

                if (selectedDays.Contains(currentDate.DayOfWeek))
                {
                    eventsBuilder.Append(CreateVirtualEvent(workoutTemplate, currentDate));
                }
            }
        }

        return eventsBuilder.ToString();
    }
}
