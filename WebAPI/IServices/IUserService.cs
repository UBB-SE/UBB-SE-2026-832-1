using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();
}
