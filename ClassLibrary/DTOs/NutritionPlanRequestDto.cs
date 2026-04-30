namespace ClassLibrary.DTOs;

using System;

public sealed class NutritionPlanRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
}
