using ClassLibrary.Models;

namespace WinUI.Services;

public interface IRankShowcaseService
{
    Task<RankShowcaseSnapshot> GetRankShowcaseAsync(int clientId);
}
