using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class UserService : IUserService
{
    private readonly IUserServiceProxy serviceProxy;

    public UserService(IUserServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return this.serviceProxy.GetUsersAsync(cancellationToken);
    }
}

