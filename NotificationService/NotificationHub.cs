using Microsoft.AspNetCore.SignalR;
using MongoDbAdapter.DataAccess.Base;
using NotificationService.Shared;

namespace NotificationService;

public class NotificationHub : Hub
{
    private readonly IStore<Notification> _notificationStore;

    public NotificationHub(IStore<Notification> notificationStore) =>
        _notificationStore = notificationStore;


    //TODO: Receive clientId
    public async Task<IEnumerable<Notification>> GetNotifications(CancellationToken cancellationToken)
    {
        var notifications = await _notificationStore.GetAllAsync();

        Guid clientId = Guid.Empty;
        if (clientId != Guid.Empty)
        {
            var clientNotifications = await _notificationStore.GetAsync(n => n.ClientId == clientId);
            notifications = notifications.Concat(clientNotifications);
        }

        return notifications;
    }
}