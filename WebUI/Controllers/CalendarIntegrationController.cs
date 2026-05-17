using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Proxies.Interfaces;
using ClassLibrary.Models;
using WebUI.Models;

namespace WebUI.Controllers;

public class CalendarIntegrationController : Controller
{
    private readonly ICalendarWorkoutCatalogProxy calendarWorkoutCatalogProxy;
    private readonly ICalendarExportProxy calendarExportProxy;
    private readonly IUserSession userSession;

    public CalendarIntegrationController(
        ICalendarWorkoutCatalogProxy calendarWorkoutCatalogProxy,
        ICalendarExportProxy calendarExportProxy,
        IUserSession userSession)
    {
        this.calendarWorkoutCatalogProxy = calendarWorkoutCatalogProxy ?? throw new ArgumentNullException(nameof(calendarWorkoutCatalogProxy));
        this.calendarExportProxy = calendarExportProxy ?? throw new ArgumentNullException(nameof(calendarExportProxy));
        this.userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    }

    public IActionResult Index()
    {
       
        if (!userSession.IsClient)
        {
            return RedirectToAction("Index", "Home");
        }

        int clientId = userSession.CurrentClientId;
        var availableWorkouts = calendarWorkoutCatalogProxy.GetFallbackWorkouts(clientId);

        var viewModel = new CalendarIntegrationViewModel
        {
            AvailableWorkouts = availableWorkouts,
            SelectedWorkout = availableWorkouts.FirstOrDefault(),
            DurationWeeks = 4,
            SelectedDays = GetDefaultDaySelections()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateCalendar(int? selectedWorkoutId, CalendarIntegrationViewModel model)
    {
        
        if (!userSession.IsClient)
        {
            return RedirectToAction("Index", "Home");
        }

        int clientId = userSession.CurrentClientId;
        model.AvailableWorkouts = calendarWorkoutCatalogProxy.GetFallbackWorkouts(clientId);

        
        if (selectedWorkoutId.HasValue)
        {
            model.SelectedWorkout = model.AvailableWorkouts.FirstOrDefault(w => w.WorkoutTemplateId == selectedWorkoutId.Value);
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var selectedDays = model.SelectedDays.Where(d => d.IsSelected).Select(d => d.DayOfWeekIndex).ToArray();

        if (model.SelectedWorkout == null)
        {
            ModelState.AddModelError("", "Please select a workout.");
            return View("Index", model);
        }

        
        if (model.DurationWeeks < 1 || model.DurationWeeks > 52)
        {
            ModelState.AddModelError("", "Duration must be between 1 and 52 weeks.");
            return View("Index", model);
        }

        if (selectedDays.Length == 0)
        {
            ModelState.AddModelError("", "Please select at least one training day.");
            return View("Index", model);
        }

        try
        {
            string generatedCalendar = calendarExportProxy.GenerateCalendar(
                model.SelectedWorkout,
                model.DurationWeeks,
                selectedDays);

            model.GeneratedIcsContent = generatedCalendar;
            model.IsSuccess = true;
            model.StatusMessage = "Calendar generated successfully!";
        }
        catch (Exception ex)
        {
            model.IsSuccess = false;
            model.StatusMessage = $"Error generating calendar: {ex.Message}";
        }

        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadCalendar(string icsContent, string workoutName)
    {
        
        if (!userSession.IsClient)
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(icsContent))
        {
            return RedirectToAction("Index");
        }

        try
        {
            string? filePath = await calendarExportProxy.SaveCalendarToDownloadsAsync(icsContent, workoutName);
            
            if (!string.IsNullOrEmpty(filePath))
            {
                return File(System.Text.Encoding.UTF8.GetBytes(icsContent), "text/calendar", $"{workoutName}_calendar.ics");
            }

            TempData["Error"] = "Failed to save calendar file.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error downloading calendar: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    private static List<DaySelectionItem> GetDefaultDaySelections()
    {
        var dayNames = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        var defaultSelections = new[] { false, true, true, true, true, true, false };

        return dayNames.Select((day, index) => new DaySelectionItem
        {
            DayOfWeekIndex = index,
            DayName = day,
            IsSelected = defaultSelections[index]
        }).ToList();
    }
}
