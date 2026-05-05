using ClassLibrary.Models;

namespace WinUI.Services;

public interface IAchievementsService
{
    Task<IReadOnlyList<Achievement>> GetAchievementsAsync(int clientId);
}
