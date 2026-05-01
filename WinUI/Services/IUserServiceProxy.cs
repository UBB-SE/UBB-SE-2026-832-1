using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IUserServiceProxy
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();
}

