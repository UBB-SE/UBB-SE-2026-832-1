using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class UserService : IUserService
{
    private readonly HttpClient httpClient;
    private readonly IUserServiceProxy? legacyProxy;

    public UserService()
    {
        this.httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
    }

    public UserService(IUserServiceProxy serviceProxy)
    {
        this.legacyProxy = serviceProxy;
        this.httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await this.httpClient.GetFromJsonAsync<List<UserDto>>($"{ApiBaseUrl.BASE_URL}/api/users");
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var request = new { Username = username, Password = password };
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/users/login", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> RegisterAsync(string username, string password, string role)
    {
        var request = new { Username = username, Password = password, Role = role };
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/users/register", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var user = await response.Content.ReadFromJsonAsync<UserDto>();

        if (user != null && role == "Client")
        {
            try
            {
                var clientRequest = new { Id = user.Id, UserId = user.Id };
                await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/clients", clientRequest);
            }
            catch
            {
            }
        }

        return user;
    }

    public async Task<bool> CheckIfUsernameExistsAsync(string username)
    {
        return await this.httpClient.GetFromJsonAsync<bool>(
            $"{ApiBaseUrl.BASE_URL}/api/users/exists/{Uri.EscapeDataString(username)}");
    }

    public async Task<UserDataDto?> GetUserDataAsync(int userId)
    {
        try
        {
            return await this.httpClient.GetFromJsonAsync<UserDataDto>(
                $"{ApiBaseUrl.BASE_URL}/api/users/{userId}/data");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task AddUserDataAsync(UserDataDto userDataDto)
    {
        await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/users/data", userDataDto);
    }

    public async Task UpdateUserDataAsync(UserDataDto userDataDto)
    {
        var response = await this.httpClient.PutAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/users/data", userDataDto);
        response.EnsureSuccessStatusCode();
    }
}