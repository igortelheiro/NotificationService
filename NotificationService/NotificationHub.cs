using Microsoft.AspNetCore.SignalR;
using MongoDbAdapter;
using NotificationService.Shared;

namespace NotificationService;

public class NotificationHub : Hub
{
    private readonly IStore<Notification> _notificationStore;

    public NotificationHub(IStore<Notification> notificationStore) =>
        _notificationStore = notificationStore;


    public async Task<IEnumerable<Notification>> GetNotifications(CancellationToken cancellationToken)
    {
        var generalNotifications = await _notificationStore.GetAsync();

        return generalNotifications;
    }
}