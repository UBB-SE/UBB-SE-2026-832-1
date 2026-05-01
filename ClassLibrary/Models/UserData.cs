using System;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public sealed class UserData
{
    private const int MIN_WEIGHT_KG = 1;
    private const int MAX_WEIGHT_KG = 500;
    private const int MIN_HEIGHT_CM = 1;
    private const int MAX_HEIGHT_CM = 300;
    private const string ERROR_WEIGHT_RANGE = "Weight must be a positive whole number, between 1 and 500";
    private const string ERROR_HEIGHT_RANGE = "Height must be a positive whole number, between 1 and 300";
    private const string ERROR_GENDER_REQUIRED = "Please select a gender";
    private const string ERROR_GOAL_REQUIRED = "Please select a goal";
    private const string ERROR_GENDER_INVALID = "Gender must be 'male' or 'female'";
    private const string ERROR_GOAL_INVALID = "Select a valid goal";
    private const string GENDER_MALE = "male";
    private const string GENDER_FEMALE = "female";
    private const string GOAL_BULK = "bulk";
    private const string GOAL_CUT = "cut";
    private const string GOAL_MAINTENANCE = "maintenance";
    private const string GOAL_WELL_BEING = "well-being";
    private const string REGEX_GENDER = @"^(male|female)$";
    private const string REGEX_GOAL = @"^(bulk|cut|maintenance|well-being)$";
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

    public int UserDataId { get; set; }

    public User User { get; set; } = default!;

    [Range(MIN_WEIGHT_KG, MAX_WEIGHT_KG, ErrorMessage = ERROR_WEIGHT_RANGE)]
    public int Weight { get; set; }

    [Range(MIN_HEIGHT_CM, MAX_HEIGHT_CM, ErrorMessage = ERROR_HEIGHT_RANGE)]
    public int Height { get; set; }

    public int Age { get; set; }

    [Required(ErrorMessage = ERROR_GENDER_REQUIRED)]
    [RegularExpression(REGEX_GENDER, ErrorMessage = ERROR_GENDER_INVALID)]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = ERROR_GOAL_REQUIRED)]
    [RegularExpression(REGEX_GOAL, ErrorMessage = ERROR_GOAL_INVALID)]
    public string Goal { get; set; } = string.Empty;

    public double BodyMassIndex { get; set; }

    public int CalorieNeeds { get; set; }

    public int ProteinNeeds { get; set; }

    public int CarbohydrateNeeds { get; set; }

    public int FatNeeds { get; set; }

    public int CalculateAge(DateTimeOffset? birthDate)
    {
        if (birthDate == null)
        {
            return 0;
        }

        var today = DateTime.Today;
        var birth = birthDate.Value.DateTime;

        int age = today.Year - birth.Year;
        if (birth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public double CalculateBodyMassIndex()
    {
        if (Height <= 0 || Weight <= 0)
        {
            return 0;
        }

        double heightMeters = Height / 100.0;
        double bodyMassIndex = Weight / (heightMeters * heightMeters);

        return (int)Math.Round(bodyMassIndex);
    }

    public int CalculateCalorieNeeds()
    {
        if (Weight <= 0 || Height <= 0 || Age <= 0)
        {
            return 0;
        }

        double basalMetabolicRate =
            Gender.Equals(GENDER_MALE, StringComparison.OrdinalIgnoreCase)
                ? (BASAL_METABOLIC_RATE_WEIGHT_FACTOR * Weight) +
                  (BASAL_METABOLIC_RATE_HEIGHT_FACTOR * Height) -
                  (BASAL_METABOLIC_RATE_AGE_FACTOR * Age) +
                  BASAL_METABOLIC_RATE_MALE_OFFSET
                : Gender.Equals(GENDER_FEMALE, StringComparison.OrdinalIgnoreCase)
                    ? (BASAL_METABOLIC_RATE_WEIGHT_FACTOR * Weight) +
                      (BASAL_METABOLIC_RATE_HEIGHT_FACTOR * Height) -
                      (BASAL_METABOLIC_RATE_AGE_FACTOR * Age) -
                      BASAL_METABOLIC_RATE_FEMALE_OFFSET
                    : 0;

        if (basalMetabolicRate <= 0)
        {
            return 0;
        }

        double totalDailyEnergyExpenditure = basalMetabolicRate * ACTIVITY_MULTIPLIER;

        double adjustedCalories = Goal.ToLower() switch
        {
            GOAL_BULK => totalDailyEnergyExpenditure + BULK_CALORIE_DELTA,
            GOAL_CUT => totalDailyEnergyExpenditure + CUT_CALORIE_DELTA,
            GOAL_MAINTENANCE => totalDailyEnergyExpenditure,
            GOAL_WELL_BEING => totalDailyEnergyExpenditure,
            _ => totalDailyEnergyExpenditure
        };

        return (int)Math.Round(adjustedCalories);
    }

    public int CalculateProteinNeeds()
    {
        if (Weight <= 0)
        {
            return 0;
        }

        double proteinPerKg = Goal.ToLower() switch
        {
            GOAL_BULK => PROTEIN_BULK,
            GOAL_CUT => PROTEIN_CUT,
            GOAL_MAINTENANCE => PROTEIN_MAINTENANCE,
            GOAL_WELL_BEING => PROTEIN_WELL_BEING,
            _ => PROTEIN_MAINTENANCE
        };

        return (int)Math.Round(Weight * proteinPerKg);
    }

    public int CalculateFatNeeds()
    {
        int calories = CalculateCalorieNeeds();
        if (calories <= 0)
        {
            return 0;
        }

        double fatRatio = Goal.ToLower() switch
        {
            GOAL_BULK or GOAL_CUT => FAT_BULK_CUT,
            GOAL_MAINTENANCE => FAT_MAINTENANCE,
            GOAL_WELL_BEING => FAT_WELL_BEING,
            _ => FAT_BULK_CUT
        };

        double fatCalories = calories * fatRatio;
        return (int)Math.Round(fatCalories / CALORIES_PER_GRAM_FAT);
    }

    public int CalculateCarbohydrateNeeds()
    {
        int calories = CalculateCalorieNeeds();
        int proteinCalories = CalculateProteinNeeds() * CALORIES_PER_GRAM_PROTEIN;
        int fatCalories = CalculateFatNeeds() * CALORIES_PER_GRAM_FAT;

        if (calories <= 0)
        {
            return 0;
        }

        int carbohydrateCalories = Math.Max(0, calories - proteinCalories - fatCalories);
        return (int)Math.Round(carbohydrateCalories / (double)CALORIES_PER_GRAM_CARBOHYDRATES);
    }
}
