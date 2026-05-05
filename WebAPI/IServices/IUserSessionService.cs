using ClassLibrary.Dtos;

namespace WebAPI.IServices;

public interface IUserSessionService
{
    Task<UserSessionResponseDto> GetCurrentSessionAsync();

    Task<UserSessionResponseDto> UpdateSessionAsync(UserSessionRequestDto request);
}
