using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class DailyLog
{
    [Key]
    public int Id { get; set; }

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

    // Navigation properties - no explicit IDs
    [Required]
    public virtual User User { get; set; } = null!;

    [Required]
    public virtual Meal Meal { get; set; } = null!;
}
