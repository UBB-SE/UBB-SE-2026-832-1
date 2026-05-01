using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId, CancellationToken cancellationToken = default);

    Task SaveNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}
