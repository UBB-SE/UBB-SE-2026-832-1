using System;

namespace ClassLibrary.Models;

public sealed class MealPlan
{
    public int MealPlanId { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public string GoalType { get; set; } = string.Empty;
}
