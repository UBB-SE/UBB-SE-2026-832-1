using System.Text;
using ClassLibrary.Models;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class CalendarExportService : ICalendarExportService
{
    private const int MIN_DURATION_WEEKS = 1;
    private const int MAX_DURATION_WEEKS = 52;
    private const int DAYS_IN_WEEK = 7;
    private const int DEFAULT_START_HOUR = 10;
    private const int DEFAULT_DURATION_HOURS = 1;

    public string GenerateCalendar(WorkoutTemplate workoutTemplate, int durationWeeks, int[] selectedDays, DateTime? startDate = null)
    {
        if (workoutTemplate == null)
        {
            throw new ArgumentNullException(nameof(workoutTemplate));
        }

        if (durationWeeks < MIN_DURATION_WEEKS || durationWeeks > MAX_DURATION_WEEKS)
        {
            throw new ArgumentOutOfRangeException(nameof(durationWeeks), "Duration must be between 1 and 52 weeks.");
        }

        if (selectedDays == null || selectedDays.Length == 0)
        {
            throw new ArgumentException("At least one day must be selected.", nameof(selectedDays));
        }

        DateTime baseDate = startDate ?? DateTime.Now;
        StringBuilder iCalendarBuilder = new StringBuilder();
        iCalendarBuilder.AppendLine("BEGIN:VCALENDAR");
        iCalendarBuilder.AppendLine("VERSION:2.0");
        iCalendarBuilder.AppendLine("PRODID:-//VibeCoders//Fitness//EN");
        iCalendarBuilder.AppendLine("CALSCALE:GREGORIAN");
        iCalendarBuilder.AppendLine("METHOD:PUBLISH");

        List<string> generatedEvents = GenerateWorkoutEvents(workoutTemplate, durationWeeks, selectedDays, baseDate);
        for (int eventIndex = 0; eventIndex < generatedEvents.Count; eventIndex++)
        {
            iCalendarBuilder.AppendLine(generatedEvents[eventIndex]);
        }

        iCalendarBuilder.AppendLine("END:VCALENDAR");
        return iCalendarBuilder.ToString();
    }

    public async Task<string?> SaveCalendarToDownloadsAsync(string calendarContent, string? workoutName)
    {
        if (string.IsNullOrWhiteSpace(calendarContent))
        {
            return null;
        }

        try
        {
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(downloadsPath);

            string safeWorkoutName = BuildSafeWorkoutName(workoutName);
            string fileName = $"{safeWorkoutName}-{DateTime.Now:yyyyMMdd-HHmmss}.ics";
            string fullPath = Path.Combine(downloadsPath, fileName);

            await File.WriteAllTextAsync(fullPath, calendarContent);
            return fullPath;
        }
        catch
        {
            return null;
        }
    }

    private List<string> GenerateWorkoutEvents(WorkoutTemplate workoutTemplate, int durationWeeks, int[] selectedDays, DateTime baseDate)
    {
        List<string> events = new List<string>();
        HashSet<int> selectedDaysHash = new HashSet<int>(selectedDays);

        for (int weekIndex = 0; weekIndex < durationWeeks; weekIndex++)
        {
            for (int dayOffset = 0; dayOffset < DAYS_IN_WEEK; dayOffset++)
            {
                DateTime currentDate = baseDate.AddDays((weekIndex * DAYS_IN_WEEK) + dayOffset);
                int dayOfWeek = (int)currentDate.DayOfWeek;

                if (!selectedDaysHash.Contains(dayOfWeek))
                {
                    continue;
                }

                string eventContent = CreateVirtualEvent(workoutTemplate, currentDate);
                events.Add(eventContent);
            }
        }

        return events;
    }

    private string CreateVirtualEvent(WorkoutTemplate workoutTemplate, DateTime eventDate)
    {
        StringBuilder eventBuilder = new StringBuilder();
        DateTime eventStart = eventDate.Date.AddHours(DEFAULT_START_HOUR);
        DateTime eventEnd = eventStart.AddHours(DEFAULT_DURATION_HOURS);
        eventBuilder.AppendLine("BEGIN:VEVENT");
        eventBuilder.AppendLine($"DTSTART:{FormatIcsDateTime(eventStart)}");
        eventBuilder.AppendLine($"DTEND:{FormatIcsDateTime(eventEnd)}");
        eventBuilder.AppendLine($"SUMMARY:{EscapeIcsText(workoutTemplate.Name)}");
        string exerciseDescription = BuildExerciseDescription(workoutTemplate.Exercises);
        eventBuilder.AppendLine($"DESCRIPTION:{EscapeIcsText(exerciseDescription)}");
        string uid = $"{workoutTemplate.WorkoutTemplateId}-{eventDate:yyyyMMdd}@vibecode.local";
        eventBuilder.AppendLine($"UID:{uid}");
        eventBuilder.AppendLine($"DTSTAMP:{FormatIcsDateTime(DateTime.UtcNow)}");
        eventBuilder.AppendLine("END:VEVENT");
        return eventBuilder.ToString();
    }

    private string BuildExerciseDescription(ICollection<TemplateExercise> exercises)
    {
        if (exercises == null || exercises.Count == 0)
        {
            return "No exercises specified.";
        }

        List<string> lines = new List<string>();
        foreach (TemplateExercise exercise in exercises)
        {
            string line = $"{exercise.Name} - {exercise.TargetSets}x{exercise.TargetReps} @ {exercise.TargetWeight}kg";
            lines.Add(line);
        }

        return string.Join("\n", lines);
    }

    private string FormatIcsDateTime(DateTime dateTime)
    {
        DateTime utcDateTime = dateTime.ToUniversalTime();
        return utcDateTime.ToString("yyyyMMddTHHmmssZ");
    }

    private string EscapeIcsText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return text
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
    }

    private static string BuildSafeWorkoutName(string? workoutName)
    {
        string fallbackWorkoutName = string.IsNullOrWhiteSpace(workoutName) ? "Workout" : workoutName;
        StringBuilder safeNameBuilder = new StringBuilder(fallbackWorkoutName.Length);
        char[] invalidCharacters = Path.GetInvalidFileNameChars();

        for (int index = 0; index < fallbackWorkoutName.Length; index++)
        {
            char currentCharacter = fallbackWorkoutName[index];
            if (currentCharacter == ' ' || currentCharacter == '/' || currentCharacter == '\\')
            {
                safeNameBuilder.Append('-');
                continue;
            }

            bool isInvalidCharacter = false;
            for (int invalidCharacterIndex = 0; invalidCharacterIndex < invalidCharacters.Length; invalidCharacterIndex++)
            {
                if (currentCharacter == invalidCharacters[invalidCharacterIndex])
                {
                    isInvalidCharacter = true;
                    break;
                }
            }

            safeNameBuilder.Append(isInvalidCharacter ? '-' : currentCharacter);
        }

        if (safeNameBuilder.Length == 0)
        {
            return "Workout";
        }

        return safeNameBuilder.ToString();
    }
}
