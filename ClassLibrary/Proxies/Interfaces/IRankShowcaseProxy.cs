using ClassLibrary.Models;

namespace ClassLibrary.Proxies.Interfaces;

public interface IRankShowcaseProxy
{
    Task<RankShowcaseSnapshot> GetRankShowcaseAsync(int clientId);
}



