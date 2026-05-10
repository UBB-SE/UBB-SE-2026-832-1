using Microsoft.AspNetCore.Mvc;
using WebAPI.IServices;
using ClassLibrary.DTOs;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/reminder-management")]
public class RemindersController : ControllerBase
{
    private readonly IReminderService reminderService;

    public RemindersController(IReminderService reminderService)
    {
        this.reminderService = reminderService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReminders(int userId)
    {
        var result = await this.reminderService.GetUserRemindersAsync(userId);
        return Ok(result);
    }

    [HttpGet("user/{userId}/next")]
    public async Task<IActionResult> GetNextReminder(int userId)
    {
        var result = await this.reminderService.GetNextReminderAsync(userId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("save-reminder")]
    public async Task<IActionResult> SaveReminder([FromBody] SaveReminderRequestDataTransferObject request)
    {
        var success = await this.reminderService.SaveReminderAsync(request);
        return success ? Ok() : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReminder(int id)
    {
        await this.reminderService.DeleteReminderAsync(id);
        return NoContent();
    }
}
