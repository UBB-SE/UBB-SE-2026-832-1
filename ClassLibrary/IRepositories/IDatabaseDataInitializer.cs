namespace ClassLibrary.IRepositories
{
    /// <summary>
    /// Seeds initial application data like prebuilt workouts and achievements.
    ///
    /// Note: original external implementation included direct SQL and some
    /// non-repository logic. This interface represents repository responsibilities
    /// only. Implementation must use EF Core for data access.
    /// </summary>
    public interface IDatabaseDataInitializer
    {
        void SeedPrebuiltWorkouts();
        void SeedAchievementCatalog();
        void SeedWorkoutMilestoneAchievements();
        void SeedEvaluationEngineAchievements();
        void SeedTestData();
    }
}
