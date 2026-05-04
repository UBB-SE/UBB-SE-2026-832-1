namespace WebAPI.Services.Interfaces;

using ClassLibrary.DTOs;

public interface IWorkoutDataForwarder
{
    Task<WorkoutForwardResponseDto> ForwardCompletedWorkoutAsync(
        WorkoutLogRequestDto request,
        CancellationToken cancellationToken = default);
}