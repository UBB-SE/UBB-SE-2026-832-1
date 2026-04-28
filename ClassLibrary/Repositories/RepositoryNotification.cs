using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class RepositoryNotification(AppDbContext dbContext) : IRepositoryNotification
{
    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.Client.Id == clientId)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SaveNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await dbContext.Notifications.AddAsync(notification, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
