using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class DailyLog
{
    public int DailyLogId { get; set; }

    public DateTime LoggedAt { get; set; }

    public double Calories { get; set; }

    public double Protein { get; set; }

    public double Carbohydrates { get; set; }

    public double Fats { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Meal Meal { get; set; } = null!;
    public virtual FoodItem FoodItem { get; set; } = null!;
}
