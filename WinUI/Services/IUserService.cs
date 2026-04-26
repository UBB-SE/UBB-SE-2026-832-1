using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
}

