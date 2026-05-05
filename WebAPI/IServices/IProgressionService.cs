using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IProgressionService
{
    Task EvaluateWorkoutAsync(EvaluateWorkoutRequestDataTransferObject request);

    Task<bool> ProcessDeloadAsync(ProcessDeloadRequestDataTransferObject request);
}
