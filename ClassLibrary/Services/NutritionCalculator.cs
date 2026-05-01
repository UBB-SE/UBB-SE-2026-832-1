using System;
using ClassLibrary.Models;

namespace ClassLibrary.Services;

public static class NutritionCalculator
{
    private const string GENDER_MALE = "male";
    private const string GENDER_FEMALE = "female";
    private const string GOAL_BULK = "bulk";
    private const string GOAL_CUT = "cut";
    private const string GOAL_MAINTENANCE = "maintenance";
    private const string GOAL_WELL_BEING = "well-being";
    private const double BASAL_METABOLIC_RATE_WEIGHT_FACTOR = 10.0;
    private const double BASAL_METABOLIC_RATE_HEIGHT_FACTOR = 6.25;
    private const double BASAL_METABOLIC_RATE_AGE_FACTOR = 5.0;
    private const double BASAL_METABOLIC_RATE_MALE_OFFSET = 5.0;
    private const double BASAL_METABOLIC_RATE_FEMALE_OFFSET = 161.0;
    private const double ACTIVITY_MULTIPLIER = 1.55;
    private const int BULK_CALORIE_DELTA = 300;
    private const int CUT_CALORIE_DELTA = -300;
    private const double PROTEIN_BULK = 2.0;
    private const double PROTEIN_CUT = 2.2;
    private const double PROTEIN_MAINTENANCE = 1.8;
    private const double PROTEIN_WELL_BEING = 1.6;
    private const double FAT_BULK_CUT = 0.25;
    private const double FAT_MAINTENANCE = 0.28;
    private const double FAT_WELL_BEING = 0.30;
    private const int CALORIES_PER_GRAM_PROTEIN = 4;
    private const int CALORIES_PER_GRAM_CARBOHYDRATES = 4;
    private const int CALORIES_PER_GRAM_FAT = 9;

    public static int CalculateAge(DateTimeOffset? birthDate)
    {
        if (birthDate == null)
        {
            return 0;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var birth = DateOnly.FromDateTime(birthDate.Value.Date);

        int age = today.Year - birth.Year;
        if (birth > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public static double CalculateBodyMassIndex(UserData userData)
    {
        if (userData.Height <= 0 || userData.Weight <= 0)
        {
            return 0;
        }

        double heightMeters = userData.Height / 100.0;
        double bodyMassIndex = userData.Weight / (heightMeters * heightMeters);

        return Math.Round(bodyMassIndex);
    }

    public static int CalculateCalorieNeeds(UserData userData)
    {
        if (userData.Weight <= 0 || userData.Height <= 0 || userData.Age <= 0)
        {
            return 0;
        }

        string gender = userData.Gender ?? string.Empty;

        double basalMetabolicRate =
            gender.Equals(GENDER_MALE, StringComparison.OrdinalIgnoreCase)
                ? (BASAL_METABOLIC_RATE_WEIGHT_FACTOR * userData.Weight) +
                  (BASAL_METABOLIC_RATE_HEIGHT_FACTOR * userData.Height) -
                  (BASAL_METABOLIC_RATE_AGE_FACTOR * userData.Age) +
                  BASAL_METABOLIC_RATE_MALE_OFFSET
                : gender.Equals(GENDER_FEMALE, StringComparison.OrdinalIgnoreCase)
                    ? (BASAL_METABOLIC_RATE_WEIGHT_FACTOR * userData.Weight) +
                      (BASAL_METABOLIC_RATE_HEIGHT_FACTOR * userData.Height) -
                      (BASAL_METABOLIC_RATE_AGE_FACTOR * userData.Age) -
                      BASAL_METABOLIC_RATE_FEMALE_OFFSET
                    : 0;

        if (basalMetabolicRate <= 0)
        {
            return 0;
        }

        double totalDailyEnergyExpenditure = basalMetabolicRate * ACTIVITY_MULTIPLIER;

        string goal = (userData.Goal ?? string.Empty).ToLowerInvariant();

        double adjustedCalories = goal switch
        {
            GOAL_BULK => totalDailyEnergyExpenditure + BULK_CALORIE_DELTA,
            GOAL_CUT => totalDailyEnergyExpenditure + CUT_CALORIE_DELTA,
            GOAL_MAINTENANCE => totalDailyEnergyExpenditure,
            GOAL_WELL_BEING => totalDailyEnergyExpenditure,
            _ => totalDailyEnergyExpenditure
        };

        return (int)Math.Round(adjustedCalories);
    }

    public static int CalculateProteinNeeds(UserData userData)
    {
        if (userData.Weight <= 0)
        {
            return 0;
        }

        string goal = (userData.Goal ?? string.Empty).ToLowerInvariant();

        double proteinPerKg = goal switch
        {
            GOAL_BULK => PROTEIN_BULK,
            GOAL_CUT => PROTEIN_CUT,
            GOAL_MAINTENANCE => PROTEIN_MAINTENANCE,
            GOAL_WELL_BEING => PROTEIN_WELL_BEING,
            _ => PROTEIN_MAINTENANCE
        };

        return (int)Math.Round(userData.Weight * proteinPerKg);
    }

    public static int CalculateFatNeeds(UserData userData)
    {
        int calories = CalculateCalorieNeeds(userData);
        if (calories <= 0)
        {
            return 0;
        }

        string goal = (userData.Goal ?? string.Empty).ToLowerInvariant();

        double fatRatio = goal switch
        {
            GOAL_BULK or GOAL_CUT => FAT_BULK_CUT,
            GOAL_MAINTENANCE => FAT_MAINTENANCE,
            GOAL_WELL_BEING => FAT_WELL_BEING,
            _ => FAT_BULK_CUT
        };

        double fatCalories = calories * fatRatio;
        return (int)Math.Round(fatCalories / CALORIES_PER_GRAM_FAT);
    }

    public static int CalculateCarbohydrateNeeds(UserData userData)
    {
        int calories = CalculateCalorieNeeds(userData);
        int proteinCalories = CalculateProteinNeeds(userData) * CALORIES_PER_GRAM_PROTEIN;
        int fatCalories = CalculateFatNeeds(userData) * CALORIES_PER_GRAM_FAT;

        if (calories <= 0)
        {
            return 0;
        }

        int carbohydrateCalories = Math.Max(0, calories - proteinCalories - fatCalories);
        return (int)Math.Round(carbohydrateCalories / (double)CALORIES_PER_GRAM_CARBOHYDRATES);
    }
}
