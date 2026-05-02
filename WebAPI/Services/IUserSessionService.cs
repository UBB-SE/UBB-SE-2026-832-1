using ClassLibrary.Dtos;
using WebApi.Services.UserSession.Interfaces;

namespace WebApi.Services.UserSession;

public sealed class UserSessionService : IUserSessionService
{
    private long currentUserId = 1;
    private long currentClientId = 1;

    public Task<UserSessionResponseDto> GetCurrentSessionAsync()
    {
        var response = new UserSessionResponseDto
        {
            UserId = this.currentUserId,
            ClientId = this.currentClientId
        };

        return Task.FromResult(response);
    }

    public Task<UserSessionResponseDto> UpdateSessionAsync(UserSessionRequestDto request)
    {
        this.currentUserId = request.UserId;
        this.currentClientId = request.ClientId;

        return GetCurrentSessionAsync();
    }
}