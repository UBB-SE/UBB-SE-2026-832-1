using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class DailyLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int MealId { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; }

    [Required]
    public double Calories { get; set; }

    [Required]
    public double Protein { get; set; }

    [Required]
    public double Carbohydrates { get; set; }

    [Required]
    public double Fats { get; set; }
}
