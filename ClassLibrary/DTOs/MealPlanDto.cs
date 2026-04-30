using System;
using System.Collections.Generic;

namespace ClassLibrary.DTOs;

public sealed class MealPlanDto
{
    public int MealPlanId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string GoalType { get; set; } = string.Empty;

    public List<FoodItemDto> FoodItems { get; set; } = new();
}
