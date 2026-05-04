using ClassLibrary.Services;

namespace Tests;

public sealed class ExerciseCalorieCalculatorTests
{
    [Fact]
    public void GetMetabolicEquivalent_KnownExercise_ReturnsCorrectValue()
    {
        double met = ExerciseCalorieCalculator.GetMetabolicEquivalent("Bench Press");

        Assert.Equal(5.0, met);
    }

    [Fact]
    public void GetMetabolicEquivalent_UnknownExercise_ReturnsDefault()
    {
        double met = ExerciseCalorieCalculator.GetMetabolicEquivalent("Underwater Basket Weaving");

        Assert.Equal(5.0, met);
    }

    [Fact]
    public void GetMetabolicEquivalent_IsCaseInsensitive()
    {
        double lower = ExerciseCalorieCalculator.GetMetabolicEquivalent("bench press");
        double upper = ExerciseCalorieCalculator.GetMetabolicEquivalent("BENCH PRESS");
        double mixed = ExerciseCalorieCalculator.GetMetabolicEquivalent("Bench Press");

        Assert.Equal(lower, upper);
        Assert.Equal(upper, mixed);
    }

    [Fact]
    public void GetMetabolicEquivalent_HighIntensityExercise_ReturnsHigherValue()
    {
        double pullUps = ExerciseCalorieCalculator.GetMetabolicEquivalent("Pull-Ups");
        double bicepCurls = ExerciseCalorieCalculator.GetMetabolicEquivalent("Bicep Curls");

        Assert.Equal(8.0, pullUps);
        Assert.Equal(2.5, bicepCurls);
        Assert.True(pullUps > bicepCurls);
    }

    [Fact]
    public void CalculateCalories_StandardInputs_ReturnsExpectedResult()
    {
        // MET 5.0 * 70kg * 1 hour = 350
        int calories = ExerciseCalorieCalculator.CalculateCalories(5.0, 70.0, TimeSpan.FromHours(1));

        Assert.Equal(350, calories);
    }

    [Fact]
    public void CalculateCalories_ZeroDuration_ReturnsZero()
    {
        int calories = ExerciseCalorieCalculator.CalculateCalories(5.0, 80.0, TimeSpan.Zero);

        Assert.Equal(0, calories);
    }

    [Fact]
    public void CalculateCalories_ThirtyMinutes_ReturnsHalfOfHourly()
    {
        int hourly = ExerciseCalorieCalculator.CalculateCalories(5.0, 70.0, TimeSpan.FromHours(1));
        int halfHour = ExerciseCalorieCalculator.CalculateCalories(5.0, 70.0, TimeSpan.FromMinutes(30));

        Assert.Equal(175, halfHour);
        Assert.Equal(hourly / 2, halfHour);
    }

    [Fact]
    public void CalculateCalories_RoundsToNearestInteger()
    {
        // MET 5.0 * 70kg * 0.333 hours = 116.55 -> rounds to 117
        int calories = ExerciseCalorieCalculator.CalculateCalories(5.0, 70.0, TimeSpan.FromMinutes(20));

        Assert.Equal(117, calories);
    }
}
