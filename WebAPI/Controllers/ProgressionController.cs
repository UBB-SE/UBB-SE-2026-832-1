using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/progression")]
public sealed class ProgressionController : ControllerBase
{
    private readonly IProgressionService progressionService;

    public ProgressionController(IProgressionService progressionService)
    {
        this.progressionService = progressionService;
    }

    [HttpPost("evaluate-workout")]
    public async Task<IActionResult> EvaluateWorkout([FromBody] EvaluateWorkoutRequestDataTransferObject request)
    {
        try
        {
            await this.progressionService.EvaluateWorkoutAsync(request);
            return this.Ok();
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("process-deload")]
    public async Task<IActionResult> ProcessDeload([FromBody] ProcessDeloadRequestDataTransferObject request)
    {
        try
        {
            var result = await this.progressionService.ProcessDeloadAsync(request);
            if (!result)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
