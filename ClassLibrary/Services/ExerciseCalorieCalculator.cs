using System;
using System.Collections.Generic;

namespace ClassLibrary.Services;

public static class ExerciseCalorieCalculator
{
    private const double DEFAULT_METABOLIC_EQUIVALENT = 5.0;

    private static readonly Dictionary<string, double> MetabolicEquivalentByExercise =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Bench Press", 5.0 },
            { "Incline Dumbbell Press", 5.0 },
            { "Barbell Squat", 6.0 },
            { "Leg Press", 5.0 },
            { "Deadlift", 6.0 },
            { "Pull-Ups", 8.0 },
            { "Overhead Press", 5.0 },
            { "Side Laterals", 2.5 },
            { "Bicep Curls", 2.5 },
            { "Tricep Pushdowns", 2.5 },
        };

    public static double GetMetabolicEquivalent(string exerciseName)
    {
        return MetabolicEquivalentByExercise.TryGetValue(exerciseName, out var met)
            ? met
            : DEFAULT_METABOLIC_EQUIVALENT;
    }

    public static int CalculateCalories(double metabolicEquivalent, double weightKg, TimeSpan duration)
    {
        return (int)Math.Round(metabolicEquivalent * weightKg * duration.TotalHours);
    }
}
