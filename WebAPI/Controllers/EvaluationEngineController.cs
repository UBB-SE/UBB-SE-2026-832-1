using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/evaluation")]
public sealed class EvaluationEngineController : ControllerBase
{
    private readonly IEvaluationEngineService evaluationEngineService;

    public EvaluationEngineController(IEvaluationEngineService evaluationEngineService)
    {
        this.evaluationEngineService = evaluationEngineService;
    }

    [HttpPost("{clientId}/evaluate")]
    public async Task<IActionResult> Evaluate(int clientId)
    {
        if (clientId <= 0)
        {
            return this.BadRequest("Invalid client ID.");
        }

        try
        {
            var titles = await this.evaluationEngineService.EvaluateAsync(clientId);
            return this.Ok(titles);
        }
        catch
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{clientId}/rank-showcase")]
    public async Task<IActionResult> GetRankShowcase(int clientId)
    {
        if (clientId <= 0)
        {
            return this.BadRequest("Invalid client ID.");
        }

        try
        {
            var snapshot = await this.evaluationEngineService.BuildRankShowcaseAsync(clientId);
            return this.Ok(snapshot);
        }
        catch
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
