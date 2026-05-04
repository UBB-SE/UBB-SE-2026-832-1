using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/calendar-export")]
public sealed class CalendarExportController : ControllerBase
{
    private readonly ICalendarExportService calendarExportService;

    public CalendarExportController(ICalendarExportService calendarExportService)
    {
        this.calendarExportService = calendarExportService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCalendar([FromBody] GenerateCalendarRequestDataTransferObject request)
    {
        try
        {
            var response = await this.calendarExportService.GenerateCalendarAsync(request);
            return this.Ok(response);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return this.BadRequest(exception.Message);
        }
        catch (ArgumentException exception)
        {
            return this.BadRequest(exception.Message);
        }
        catch (ArgumentNullException)
        {
            return this.NotFound();
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveCalendar([FromBody] SaveCalendarRequestDataTransferObject request)
    {
        if (string.IsNullOrWhiteSpace(request.IcsContent))
        {
            return this.BadRequest("ICS content must not be empty.");
        }

        try
        {
            var response = await this.calendarExportService.SaveCalendarAsync(request);
            if (response.IsSuccess == false)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return this.Ok(response);
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
