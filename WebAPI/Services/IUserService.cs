using ClassLibrary.DTOs;

namespace WebAPI.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();
}

