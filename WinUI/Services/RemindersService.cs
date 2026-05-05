using System.Net.Http.Json;
using ClassLibrary.DTOs;
namespace WinUI.Services;

public sealed class ReminderService : IReminderService
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";

    private readonly HttpClient httpClient;

    public ReminderService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ReminderDto>> GetUserRemindersAsync(int userId)
    {
        var reminders = await this.httpClient.GetFromJsonAsync<List<ReminderDto>>($"{API_BASE_ADDRESS}/api/reminders/user/{userId}");
        return reminders ?? [];
    }

    public async Task<ReminderDto?> GetReminderByIdAsync(int id)
    {
        return await this.httpClient.GetFromJsonAsync<ReminderDto>($"{API_BASE_ADDRESS}/api/reminders/{id}");
    }

    public async Task<ReminderDto?> GetNextReminderAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<ReminderDto>($"{API_BASE_ADDRESS}/api/reminders/user/{userId}/next");
    }

    public async Task<bool> SaveReminderAsync(ReminderDto reminder)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/reminders", reminder);
        return response.IsSuccessStatusCode;
    }

    public async Task DeleteReminderAsync(int id)
    {
        var response = await this.httpClient.DeleteAsync($"{API_BASE_ADDRESS}/api/reminders/{id}");
        response.EnsureSuccessStatusCode();
    }
}