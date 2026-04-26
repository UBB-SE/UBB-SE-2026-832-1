using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class UserService(IUserServiceProxy serviceProxy) : IUserService
{
    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return serviceProxy.GetUsersAsync(cancellationToken);
    }
}

