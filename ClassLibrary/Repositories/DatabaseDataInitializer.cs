using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// EF Core based implementation of the data seeding previously provided by
    /// the external repository. This implementation intentionally removes any
    /// application-level (UI or workflow) logic and exposes repository-style
    /// methods for seeding initial data. All database access uses the
    /// application's EF Core model.
    /// </summary>
    public sealed class DatabaseDataInitializer : IDatabaseDataInitializer
    {
        private readonly AppDbContext dbContext;

        public DatabaseDataInitializer(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void SeedPrebuiltWorkouts()
        {
            // Create a small set of pre-built workout templates if they don't
            // already exist. This mirrors the original repository but operates
            // through EF Core.
            SeedTemplate("HIIT Fat Burner", new[]
            {
                ("Jumping Jacks", MuscleGroup.LEGS, 3, 20),
                ("Burpees", MuscleGroup.CORE, 3, 15),
                ("Mountain Climbers", MuscleGroup.CORE, 3, 20),
            });

            SeedTemplate("Full Body Mass", new[]
            {
                ("Back Squat", MuscleGroup.LEGS, 4, 8),
                ("Bench Press", MuscleGroup.CHEST, 4, 8),
                ("Barbell Rows", MuscleGroup.BACK, 4, 8),
            });

            SeedTemplate("Full Body Power", new[]
            {
                ("Deadlift", MuscleGroup.BACK, 4, 5),
                ("Overhead Press", MuscleGroup.SHOULDERS, 4, 5),
                ("Weighted Pull-Ups", MuscleGroup.BACK, 4, 5),
            });

            SeedTemplate("Endurance Circuit", new[]
            {
                ("Push-Ups", MuscleGroup.CHEST, 3, 20),
                ("Bodyweight Squats", MuscleGroup.LEGS, 3, 25),
                ("Plank", MuscleGroup.CORE, 3, 60),
            });
        }

        private void SeedTemplate(string name, IEnumerable<(string ExerciseName, MuscleGroup MuscleGroup, int Sets, int Reps)> exercises)
        {
            if (this.dbContext.WorkoutTemplates.Any(wt => wt.Name == name && wt.Type == WorkoutType.PreBuilt))
            {
                // Normalize existing matching template exercises' target weights
                var templates = this.dbContext.WorkoutTemplates
                    .Where(wt => wt.Name == name && wt.Type == WorkoutType.PreBuilt)
                    .Include(wt => wt.Exercises)
                    .ToList();

                foreach (var t in templates)
                {
                    foreach (var ex in t.Exercises)
                    {
                        ex.TargetWeight = 0;
                    }
                }

                this.dbContext.SaveChanges();
                return;
            }

            var template = new WorkoutTemplate
            {
                Client = null!, // Pre-built templates are not tied to a client in the original schema
                Name = name,
                Type = WorkoutType.PreBuilt,
            };

            this.dbContext.WorkoutTemplates.Add(template);
            this.dbContext.SaveChanges();

            foreach (var (exerciseName, muscleGroup, sets, reps) in exercises)
            {
                var te = new TemplateExercise
                {
                    WorkoutTemplate = template,
                    Name = exerciseName,
                    MuscleGroup = muscleGroup,
                    TargetSets = sets,
                    TargetReps = reps,
                };

                this.dbContext.Add(te);
            }

            this.dbContext.SaveChanges();
        }

        public void SeedAchievementCatalog()
        {
            if (this.dbContext.Achievements.Any())
            {
                return;
            }

            void Insert(string title, string description, string criteria)
            {
                this.dbContext.Achievements.Add(new Achievement
                {
                    Title = title,
                    Description = description,
                    Criteria = criteria,
                });
            }

            Insert("First Steps", "Prove that you have what it takes to begin.", "Complete your first workout.");
            Insert("Week Warrior", "Show that you can maintain consistency.", "Log workouts on 5 different days.");
            Insert("Dedicated", "Demonstrate your long-term commitment.", "Reach 50 hours of total active time.");

            this.dbContext.SaveChanges();
        }

        public void SeedWorkoutMilestoneAchievements()
        {
            foreach (var milestone in ClassLibrary.Models.TotalWorkoutsMilestoneEvaluator.DefaultMilestones)
            {
                var existing = this.dbContext.Achievements.FirstOrDefault(a => a.Title == milestone.title);
                if (existing == null)
                {
                    this.dbContext.Achievements.Add(new Achievement
                    {
                        Title = milestone.title,
                        Description = milestone.description,
                        ThresholdWorkouts = milestone.threshold,
                    });
                }
                else if (!existing.ThresholdWorkouts.HasValue)
                {
                    existing.ThresholdWorkouts = milestone.threshold;
                }
            }

            this.dbContext.SaveChanges();
        }

        public void SeedEvaluationEngineAchievements()
        {
            void Upsert(string title, string description, string criteria)
            {
                var existing = this.dbContext.Achievements.FirstOrDefault(a => a.Title == title);
                if (existing == null)
                {
                    this.dbContext.Achievements.Add(new Achievement
                    {
                        Title = title,
                        Description = description,
                        Criteria = criteria,
                    });
                }
                else
                {
                    existing.Description = description;
                    existing.Criteria = criteria;
                }
            }

            Upsert(
                "Week Warrior",
                "Prove you can train every day for a full week.",
                "Log a workout on 7 consecutive calendar days.");

            Upsert(
                "3-Day Streak",
                "Keep the momentum — three days in a row.",
                "Log a workout on 3 consecutive calendar days.");

            Upsert(
                "Iron Week",
                "Push your weekly limits to the top.",
                "Complete 5 workouts within any rolling 7-day window.");

            this.dbContext.SaveChanges();
        }

        public void SeedTestData()
        {
            // Create a trainer and client with some workout logs and sets that
            // are useful for local development. The original implementation
            // contained many direct SQL statements; this version uses EF Core
            // and intentionally omits any UI or side-effect logic.
            if (this.dbContext.Users.Any(u => u.Username == "TestTrainer"))
            {
                return;
            }

            var trainerUser = new User { Username = "TestTrainer" };
            this.dbContext.Users.Add(trainerUser);
            this.dbContext.SaveChanges();

            var trainer = new Trainer { User = trainerUser };
            this.dbContext.Trainers.Add(trainer);
            this.dbContext.SaveChanges();

            var clientUser = new User { Username = "TestClient" };
            this.dbContext.Users.Add(clientUser);
            this.dbContext.SaveChanges();

            var client = new Client { User = clientUser, Trainer = trainer, Weight = 85.5, Height = 180.0 };
            this.dbContext.Clients.Add(client);
            this.dbContext.SaveChanges();

            int CreateLog(DateTime date, TimeSpan duration, int cals)
            {
                var log = new WorkoutLog
                {
                    Client = client,
                    Date = date,
                    Duration = duration,
                    TotalCaloriesBurned = cals,
                };

                this.dbContext.WorkoutLogs.Add(log);
                this.dbContext.SaveChanges();
                return log.WorkoutLogId;
            }

            void AddSet(int logId, string exName, int setIndex, int reps, double weight)
            {
                var log = this.dbContext.WorkoutLogs.Find(logId) ?? throw new InvalidOperationException();
                var set = new LoggedSet
                {
                    ExerciseName = exName,
                    SetIndex = setIndex,
                    ActualReps = reps,
                    ActualWeight = weight,
                    Exercise = null!,
                    WorkoutLog = log,
                };

                this.dbContext.Add(set);
                this.dbContext.SaveChanges();
            }

            var log1 = CreateLog(DateTime.Now, TimeSpan.Parse("01:15:00"), 450);
            AddSet(log1, "Barbell Squat", 1, 10, 100.0);
            AddSet(log1, "Barbell Squat", 2, 8, 105.0);
            AddSet(log1, "Barbell Squat", 3, 6, 110.0);
            AddSet(log1, "Romanian Deadlift", 1, 12, 80.0);
            AddSet(log1, "Romanian Deadlift", 2, 12, 80.0);
            AddSet(log1, "Romanian Deadlift", 3, 10, 85.0);
            AddSet(log1, "Romanian Deadlift", 4, 8, 90.0);
            AddSet(log1, "Calf Raises", 1, 15, 60.0);
            AddSet(log1, "Calf Raises", 2, 15, 60.0);

            var log2 = CreateLog(DateTime.Now.AddDays(-3), TimeSpan.Parse("00:55:00"), 320);
            AddSet(log2, "Bench Press", 1, 10, 80.0);
            AddSet(log2, "Bench Press", 2, 8, 85.0);
            AddSet(log2, "Bench Press", 3, 8, 85.0);
            AddSet(log2, "Overhead Press", 1, 10, 40.0);
            AddSet(log2, "Overhead Press", 2, 10, 40.0);

            var log3 = CreateLog(DateTime.Now.AddDays(-7), TimeSpan.Parse("01:05:00"), 400);
            AddSet(log3, "Pull-ups", 1, 12, 0.0);
            AddSet(log3, "Pull-ups", 2, 10, 0.0);
            AddSet(log3, "Pull-ups", 3, 8, 0.0);
            AddSet(log3, "Barbell Row", 1, 10, 60.0);
            AddSet(log3, "Barbell Row", 2, 10, 60.0);
            AddSet(log3, "Barbell Row", 3, 8, 65.0);
        }
    }
}
