using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class DailyLog
{
    public int Id { get; set; }

    public DateTime LoggedAt { get; set; }

    public double Calories { get; set; }

    public double Protein { get; set; }

    public double Carbohydrates { get; set; }

    public double Fats { get; set; }

    // Foreign key properties - EF Core recognizes by ClassNameId convention
    public Guid UserId { get; set; }

    public int MealId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Meal Meal { get; set; } = null!;
}
