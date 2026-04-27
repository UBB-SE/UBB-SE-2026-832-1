using System;

namespace ClassLibrary.Models;

public sealed class MealPlanMeal
{
    public int MealPlanId { get; set; }

    public int MealId { get; set; }

    public string MealType { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; }

    public bool IsConsumed { get; set; }
}
