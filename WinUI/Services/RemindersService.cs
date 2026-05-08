using System.Net.Http.Json;
using ClassLibrary.DTOs;
namespace WinUI.Services;

public sealed class ReminderService : IReminderService
{

    private readonly HttpClient httpClient;

    public ReminderService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ReminderDto>> GetUserRemindersAsync(int userId)
    {
        var reminders = await this.httpClient.GetFromJsonAsync<List<ReminderDto>>($"{ApiBaseUrl.BASE_URL}/api/reminders/user/{userId}");
        return reminders ?? [];
    }

    public async Task<ReminderDto?> GetReminderByIdAsync(int id)
    {
        return await this.httpClient.GetFromJsonAsync<ReminderDto>($"{ApiBaseUrl.BASE_URL}/api/reminders/{id}");
    }

    public async Task<ReminderDto?> GetNextReminderAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<ReminderDto>($"{ApiBaseUrl.BASE_URL}/api/reminders/user/{userId}/next");
    }

    public async Task<bool> SaveReminderAsync(ReminderDto reminder)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/reminders", reminder);
        return response.IsSuccessStatusCode;
    }

    public async Task DeleteReminderAsync(int id)
    {
        var response = await this.httpClient.DeleteAsync($"{ApiBaseUrl.BASE_URL}/api/reminders/{id}");
        response.EnsureSuccessStatusCode();
    }
}