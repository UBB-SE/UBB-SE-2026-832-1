using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IEvaluationEngineService
{
    Task<IReadOnlyList<string>> EvaluateAsync(int clientId);

    Task<RankShowcaseSnapshotDataTransferObject> BuildRankShowcaseAsync(int clientId);
}
