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

    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId)
    {
        return await this.databaseContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.Client.ClientId == clientId)
            .Include(notification => notification.Client)
            .OrderByDescending(notification => notification.DateCreated)
            .ToListAsync();
    }

    public async Task SaveNotificationAsync(Notification notification)
    {
        notification.DateCreated = DateTime.Now;
        notification.IsRead = false;
        await this.databaseContext.Notifications.AddAsync(notification);
        await this.databaseContext.SaveChangesAsync();
    }
}
