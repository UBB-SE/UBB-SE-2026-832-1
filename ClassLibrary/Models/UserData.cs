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
    private const string REGEX_GENDER = @"^(male|female)$";
    private const string REGEX_GOAL = @"^(bulk|cut|maintenance|well-being)$";

    public int UserDataId { get; set; }

    public int UserId { get; set; }

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
}
