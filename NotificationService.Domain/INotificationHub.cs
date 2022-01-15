namespace NotificationService.Shared;

public interface INotificationHub
{
    Task SendNotification(Notification notification);
}