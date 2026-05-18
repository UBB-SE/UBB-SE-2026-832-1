using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models.Workout;

namespace WebUI.Controllers;

public class WorkoutController : Controller
{
    private readonly ICreateWorkoutProxy createWorkoutProxy;
    private readonly IActiveWorkoutProxy activeWorkoutProxy;
    private readonly IWorkoutLogProxy workoutLogProxy;
    private readonly IUserSession userSession;

    public WorkoutController(
        ICreateWorkoutProxy createWorkoutProxy,
        IActiveWorkoutProxy activeWorkoutProxy,
        IWorkoutLogProxy workoutLogProxy,
        IUserSession userSession)
    {
        this.createWorkoutProxy = createWorkoutProxy;
        this.activeWorkoutProxy = activeWorkoutProxy;
        this.workoutLogProxy = workoutLogProxy;
        this.userSession = userSession;
    }

    public async Task<IActionResult> Index()
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var clientId = userSession.CurrentClientId;
        TempData.Remove("WorkoutFinalized");
        var logs = await workoutLogProxy.GetWorkoutHistoryAsync(clientId);

        var model = new WorkoutHistoryViewModel
        {
            Logs = logs.ToList(),
        };
        return View(model);
    }

    public async Task<IActionResult> Dashboard(string? goal)
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var clientId = userSession.CurrentClientId;
        var workouts = await activeWorkoutProxy.GetAvailableWorkoutsForClient(clientId);
        var exerciseNames = await createWorkoutProxy.GetAllExerciseNamesAsync();

        var available = workouts.Where(workout => workout.Type != WorkoutType.CUSTOM);
        if (!string.IsNullOrEmpty(goal))
        {
            available = available.Where(workout => workout.Name == goal);
        }

        var model = new WorkoutDashboardViewModel
        {
            AvailableWorkouts = available.ToList(),
            SavedWorkouts = workouts.ToList(),
            SelectedGoal = goal,
            AvailableExercises = exerciseNames.ToList(),
        };
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var exerciseNames = await createWorkoutProxy.GetAllExerciseNamesAsync();
        var model = new CreateWorkoutFormModel
        {
            AvailableExercises = exerciseNames.ToList(),
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWorkoutFormModel form)
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        if (form.Exercises is null || form.Exercises.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Add at least one exercise.");
        }

        if (!ModelState.IsValid)
        {
            form.AvailableExercises = (await createWorkoutProxy.GetAllExerciseNamesAsync()).ToList();
            return View(form);
        }

        var clientId = userSession.CurrentClientId;
        var template = new WorkoutTemplate
        {
            Name = form.WorkoutName,
            Type = WorkoutType.CUSTOM,
            Client = new Client { ClientId = clientId },
            Exercises = form.Exercises!.Select(exercise => new TemplateExercise
            {
                Name = exercise.Name,
                TargetSets = exercise.TargetSets,
                TargetReps = exercise.TargetReps,
                TargetWeight = exercise.TargetWeight,
                MuscleGroup = MuscleGroup.OTHER,
            }).ToList(),
        };

        await createWorkoutProxy.SaveTrainerWorkoutAsync(template);
        TempData["WorkoutSaved"] = template.Name;
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Focus(int? templateId)
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var clientId = userSession.CurrentClientId;
        var availableWorkouts = await activeWorkoutProxy.GetAvailableWorkoutsForClient(clientId);
        var selectedTemplate = templateId.HasValue
            ? await activeWorkoutProxy.FindWorkoutTemplateById(clientId, templateId)
            : null;

        var model = new FocusModeViewModel
        {
            AvailableWorkouts = availableWorkouts.ToList(),
            SelectedTemplate = selectedTemplate,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteSet(CompleteSetFormModel form)
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var clientId = userSession.CurrentClientId;
        var template = await activeWorkoutProxy.FindWorkoutTemplateById(clientId, form.WorkoutTemplateId);
        if (template is null)
        {
            return RedirectToAction(nameof(Focus));
        }

        var workoutLog = BuildWorkoutLog(clientId, template);
        var loggedSet = new LoggedSet
        {
            ExerciseName = form.ExerciseName,
            ActualReps = form.Reps,
            ActualWeight = form.Weight,
            SetIndex = form.SetIndex,
            SetNumber = form.SetIndex,
            TargetReps = form.TargetReps,
        };

        await activeWorkoutProxy.SaveSetAsync(workoutLog, loggedSet);
        return RedirectToAction(nameof(Focus), new { templateId = form.WorkoutTemplateId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalize(int templateId, int durationSeconds = 0, string? completedSetsJson = null)
    {
        if (!userSession.IsClient)
        {
            return Forbid();
        }

        var clientId = userSession.CurrentClientId;
        var template = await activeWorkoutProxy.FindWorkoutTemplateById(clientId, templateId);
        if (template is null)
        {
            return RedirectToAction(nameof(Focus));
        }

        var workoutLog = BuildWorkoutLog(clientId, template, durationSeconds);

        if (!string.IsNullOrEmpty(completedSetsJson))
        {
            var sets = System.Text.Json.JsonSerializer.Deserialize<List<CompletedSetItem>>(completedSetsJson);
            if (sets is not null)
            {
                foreach (var group in sets.GroupBy(s => s.exerciseName))
                {
                    var exercise = new LoggedExercise { ExerciseName = group.Key };
                    foreach (var s in group.OrderBy(x => x.setNumber))
                    {
                        exercise.Sets.Add(new LoggedSet
                        {
                            ExerciseName = s.exerciseName,
                            SetNumber = s.setNumber,
                            SetIndex = s.setNumber,
                            ActualReps = s.actualReps,
                            ActualWeight = s.actualWeight,
                            TargetReps = s.targetReps,
                        });
                    }
                    workoutLog.Exercises.Add(exercise);
                }
            }
        }

        await activeWorkoutProxy.FinalizeWorkoutAsync(workoutLog);
        TempData["WorkoutFinalized"] = template.Name;
        return RedirectToAction(nameof(Index));
    }

    private sealed record CompletedSetItem(string exerciseName, int setNumber, int actualReps, double actualWeight, int targetReps);

    private static WorkoutLog BuildWorkoutLog(int clientId, WorkoutTemplate template, int durationSeconds = 0)
    {
        return new WorkoutLog
        {
            ClientId = clientId,
            Client = new Client { ClientId = clientId },
            WorkoutName = template.Name,
            SourceTemplateId = template.WorkoutTemplateId,
            Type = template.Type,
            Date = DateTime.UtcNow,
            Duration = TimeSpan.FromSeconds(durationSeconds),
        };
    }
}
