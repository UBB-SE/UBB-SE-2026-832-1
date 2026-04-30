using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class RepositoryNotification : IRepositoryNotification
{
    private readonly AppDbContext databaseContext;

    public RepositoryNotification(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.Client.ClientId == clientId)
            .Include(notification => notification.Client)
            .OrderByDescending(notification => notification.DateCreated)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SaveNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await this.databaseContext.Notifications.AddAsync(notification, cancellationToken);
        return await this.databaseContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
