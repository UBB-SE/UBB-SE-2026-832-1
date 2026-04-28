using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public double Weight { get; set; }

    public double Height { get; set; }

    [MaxLength(500)]
    public string PrimaryGoal { get; set; } = string.Empty;

    public List<WorkoutLog> WorkoutLogs { get; set; } = new();

    public ICollection<ClientAchievement> ClientAchievements { get; set; } = new List<ClientAchievement>();

    public ICollection<ClientNutritionPlan> ClientNutritionPlans { get; set; } = new List<ClientNutritionPlan>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
