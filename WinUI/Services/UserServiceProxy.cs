using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class UserServiceProxy(HttpClient httpClient) : IUserServiceProxy
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _baseUrl = "https://localhost:7197";

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _httpClient.GetFromJsonAsync<List<UserDto>>($"{_baseUrl}/api/users", cancellationToken);
        return users ?? [];
    }
}

