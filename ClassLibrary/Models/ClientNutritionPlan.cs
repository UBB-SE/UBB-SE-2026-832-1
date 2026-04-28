using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Models;

[PrimaryKey(nameof(ClientId), nameof(NutritionPlanId))]
public class ClientNutritionPlan
{
    [Required]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Required]
    public int NutritionPlanId { get; set; }

    public NutritionPlan NutritionPlan { get; set; } = null!;
}
