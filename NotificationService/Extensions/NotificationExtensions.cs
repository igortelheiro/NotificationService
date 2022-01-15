using Microsoft.AspNetCore.SignalR.Client;
using NotificationService.Requests;
using NotificationService.Shared;

namespace NotificationService.Extensions;

public static class NotificationExtensions
{
    public static Notification ToNotification(this CreateNotificationRequest request) =>
        new Notification(Guid.NewGuid(),
                         DateTime.UtcNow,
                         request.Title,
                         request.Message,
                         request.Link,
                         request.TtlInDays,
                         request.UserId);
}