using ClassLibrary.Models;

namespace WinUI.Services.Interfaces;

public interface IAchievementService
{
    Task<List<Achievement>> GetAchievementsAsync(int clientId, CancellationToken cancellationToken = default);
}