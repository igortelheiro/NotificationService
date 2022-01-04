using Microsoft.AspNetCore.SignalR.Client;
using NotificationService.Shared;

namespace ClientDemo;

public class NotificationClient
{
    public static async IAsyncEnumerable<Notification> GetNotificationsAsync()
    {
        var uri = "https://localhost:7142/notifications";

        await using var connection = new HubConnectionBuilder().WithUrl(uri).Build();

        await connection.StartAsync();

        await foreach (var notification in connection.StreamAsync<Notification>("GetNotifications"))
        {
            yield return notification;
        }
    }
}
