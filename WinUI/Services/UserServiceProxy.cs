using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class UserServiceProxy : IUserServiceProxy
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";
    private readonly HttpClient httpClient;

    public UserServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await this.httpClient.GetFromJsonAsync<List<UserDto>>($"{API_BASE_ADDRESS}/api/users");
        return users ?? [];
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var request = new { Username = username, Password = password };
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/users/login", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> RegisterAsync(string username, string password, string role)
    {
        var request = new { Username = username, Password = password, Role = role };
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/users/register", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        var result = await this.httpClient.GetFromJsonAsync<bool>(
            $"{API_BASE_ADDRESS}/api/users/exists/{Uri.EscapeDataString(username)}");
        return result;
    }

    public async Task<UserDataDto?> GetUserDataAsync(int userId)
    {
        try
        {
            return await this.httpClient.GetFromJsonAsync<UserDataDto>(
                $"{API_BASE_ADDRESS}/api/users/{userId}/data");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task AddUserDataAsync(UserDataDto userDataDto)
    {
        await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/users/data", userDataDto);
    }

    public async Task UpdateUserDataAsync(UserDataDto userDataDto)
    {
        await this.httpClient.PutAsJsonAsync($"{API_BASE_ADDRESS}/api/users/data", userDataDto);
    }
}
