using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<ClientAchievement> ClientAchievements { get; set; } = default!;

    public DbSet<WorkoutLog> WorkoutLogs { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientAchievement>()
            .HasKey(ca => new { ca.ClientId, ca.AchievementId });

        modelBuilder.Entity<ClientAchievement>()
            .HasOne(ca => ca.Achievement)
            .WithMany(a => a.ClientAchievements)
            .HasForeignKey(ca => ca.AchievementId);
    }
}
