using System;

namespace ClassLibrary.Models;

public sealed class MealPlan
{
    public int MealPlanId { get; set; }
    public ICollection<MealPlanFoodItem> MealPlanFoodItems { get; set; }
    = new List<MealPlanFoodItem>();
    public User User { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public string GoalType { get; set; } = string.Empty;
}
