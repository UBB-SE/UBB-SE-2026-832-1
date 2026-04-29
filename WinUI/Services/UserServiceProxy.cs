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

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await this.httpClient.GetFromJsonAsync<List<UserDto>>($"{API_BASE_ADDRESS}/api/users", cancellationToken);
        return users ?? [];
    }
}

