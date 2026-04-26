using ClassLibrary.IRepositories;
using ClassLibrary.Models;

namespace WebAPI.Repositories;

public sealed class UserRepository : IUserRepository
{
    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<User> users =
        [
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FullName = "test user"
            }
        ];

        return Task.FromResult(users);
    }
}

