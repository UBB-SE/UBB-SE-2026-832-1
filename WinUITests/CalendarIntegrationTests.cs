using ClassLibrary.Models;
using WinUI.Services;
using WinUI.Services.CalendarIntegration;
using WinUI.Services.CalendarIntegration.Interfaces;
using WinUI.ViewModels.CalendarIntegration;
using WinUserSession = WinUI.Services.UserSession;

namespace WinUITests;

public sealed class CalendarIntegrationTests
{
    [Fact]
    public void GenerateCalendar_WithValidInput_ReturnsIcsEnvelopeAndEvents()
    {
        CalendarExportService calendarExportService = new CalendarExportService();
        WorkoutTemplate workoutTemplate = CreateWorkoutTemplate("Strength Base");
        int[] selectedDays = [1, 3];
        DateTime startDate = new DateTime(2026, 5, 4, 9, 0, 0, DateTimeKind.Local);

        string calendarContent = calendarExportService.GenerateCalendar(workoutTemplate, 1, selectedDays, startDate);

        Assert.Contains("BEGIN:VCALENDAR", calendarContent);
        Assert.Contains("END:VCALENDAR", calendarContent);
        Assert.Equal(2, CountOccurrences(calendarContent, "BEGIN:VEVENT"));
        Assert.Contains("SUMMARY:Strength Base", calendarContent);
    }

    [Fact]
    public async Task SaveCalendarToDownloadsAsync_WithValidContent_CreatesFile()
    {
        CalendarExportService calendarExportService = new CalendarExportService();
        string calendarContent = "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nEND:VCALENDAR\r\n";
        string? savedPath = await calendarExportService.SaveCalendarToDownloadsAsync(calendarContent, "Calendar Test");

        try
        {
            Assert.False(string.IsNullOrWhiteSpace(savedPath));
            Assert.NotNull(savedPath);
            Assert.True(File.Exists(savedPath));
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                File.Delete(savedPath);
            }
        }
    }

    [Fact]
    public void GetFallbackWorkouts_ReturnsExpectedWorkoutsForClient()
    {
        CalendarWorkoutCatalogService calendarWorkoutCatalogService = new CalendarWorkoutCatalogService(new HttpClient());
        int clientId = 77;

        IReadOnlyList<WorkoutTemplate> fallbackWorkouts = calendarWorkoutCatalogService.GetFallbackWorkouts(clientId);

        Assert.Equal(4, fallbackWorkouts.Count);
        Assert.All(fallbackWorkouts, workoutTemplate => Assert.Equal(clientId, workoutTemplate.Client.ClientId));
    }

    [Fact]
    public async Task GenerateCalendarForExportAsync_WithValidSelection_ReturnsSuccess()
    {
        FakeCalendarWorkoutCatalogService workoutCatalogService = new FakeCalendarWorkoutCatalogService();
        CalendarExportService calendarExportService = new CalendarExportService();
        WinUserSession.SetCurrentSession(11, "Client");
        WinUserSession userSession = new WinUserSession();
        CalendarIntegrationViewModel calendarIntegrationViewModel = new CalendarIntegrationViewModel(workoutCatalogService, calendarExportService, userSession);

        calendarIntegrationViewModel.SelectedWorkout = workoutCatalogService.Workouts[0];
        calendarIntegrationViewModel.DurationWeeks = 2;

        CalendarIntegrationViewModel.CalendarGenerationResult generationResult = await calendarIntegrationViewModel.GenerateCalendarForExportAsync();

        Assert.True(generationResult.IsSuccessful);
        Assert.Contains("BEGIN:VCALENDAR", generationResult.GeneratedCalendarContent);
        Assert.False(string.IsNullOrWhiteSpace(calendarIntegrationViewModel.GeneratedIcsContent));
    }

    [Fact]
    public void UserSession_SetCurrentSession_NormalizesUserRoleToClient()
    {
        WinUserSession.SetCurrentSession(5, "User");
        WinUserSession userSession = new WinUserSession();

        Assert.Equal(5, userSession.CurrentClientId);
        Assert.Equal("Client", userSession.CurrentRole);
    }

    private static WorkoutTemplate CreateWorkoutTemplate(string workoutName)
    {
        return new WorkoutTemplate
        {
            WorkoutTemplateId = 1,
            Name = workoutName,
            Type = WorkoutType.CUSTOM,
            Client = new Client
            {
                ClientId = 1,
                Email = "client@test.local",
                FullName = "Calendar Tester",
                Height = 180,
                Weight = 80,
                PrimaryGoal = "Strength"
            },
            Exercises = new List<TemplateExercise>
            {
                new TemplateExercise
                {
                    TemplateExerciseId = 1,
                    Name = "Squat",
                    MuscleGroup = MuscleGroup.LEGS,
                    TargetSets = 4,
                    TargetReps = 6,
                    TargetWeight = 80
                }
            }
        };
    }

    private static int CountOccurrences(string sourceText, string token)
    {
        int index = 0;
        int count = 0;
        while (true)
        {
            int foundIndex = sourceText.IndexOf(token, index, StringComparison.Ordinal);
            if (foundIndex < 0)
            {
                return count;
            }

            count++;
            index = foundIndex + token.Length;
        }
    }

    private sealed class FakeCalendarWorkoutCatalogService : ICalendarWorkoutCatalogService
    {
        public IReadOnlyList<WorkoutTemplate> Workouts { get; } = new List<WorkoutTemplate>
        {
            CreateWorkoutTemplate("Client Plan A"),
            CreateWorkoutTemplate("Client Plan B")
        };

        public Task<IReadOnlyList<WorkoutTemplate>> GetAvailableWorkoutsAsync(int clientId, TimeSpan timeout)
        {
            _ = clientId;
            _ = timeout;
            return Task.FromResult(Workouts);
        }

        public IReadOnlyList<WorkoutTemplate> GetFallbackWorkouts(int clientId)
        {
            _ = clientId;
            return Workouts;
        }
    }
}
