using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class Client
{
    public int ClientId { get; set; }
    public User User { get; set; } = null!;
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
    public ICollection<WorkoutTemplate> WorkoutTemplates { get; set; }
    = new List<WorkoutTemplate>();
    public ICollection<WorkoutLog> WorkoutLogs { get; set; } = [];

    public ICollection<Achievement> UnlockedAchievements { get; set; } = [];

    public ICollection<ClientNutritionPlan> ClientNutritionPlans { get; set; } = [];

    public ICollection<Notification> Notifications { get; set; } = [];
}
