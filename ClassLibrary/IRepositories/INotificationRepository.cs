using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(int clientId);

    Task SaveNotificationAsync(Notification notification);
}
