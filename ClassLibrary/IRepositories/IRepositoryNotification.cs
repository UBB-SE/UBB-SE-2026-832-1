using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IRepositoryNotification
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default);

    Task<bool> SaveNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}
