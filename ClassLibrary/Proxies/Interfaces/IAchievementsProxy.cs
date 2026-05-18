using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface IAchievementsProxy
{
    Task<IReadOnlyList<Achievement>> GetAchievementsAsync(int clientId);
}



