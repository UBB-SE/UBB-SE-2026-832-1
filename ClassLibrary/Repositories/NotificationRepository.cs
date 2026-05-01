using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext databaseContext;

    public NotificationRepository(AppDbContext databaseContext)
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

    public async Task SaveNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        if (notification.DateCreated == default)
        {
            notification.DateCreated = DateTime.Now;
        }
        await this.databaseContext.Notifications.AddAsync(notification, cancellationToken);
        var rowsAffected = await this.databaseContext.SaveChangesAsync(cancellationToken);
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Notification could not be saved.");
        }
    }
}
