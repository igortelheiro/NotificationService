using Microsoft.AspNetCore.Components;
using NotificationService.Shared;

namespace ClientDemo.Pages;

public class IndexComponent : ComponentBase
{
    [Inject] private NotificationClient _notificationClient { get; set; }
    [Inject] private ILogger<IndexComponent> _logger { get; set; }

    protected string? Error = null;
    protected List<Notification> Notifications => _notificationClient.Notifications;

    protected override async Task OnInitializedAsync()
    {
        await GetNotificationsAsync();
    }


    private async Task GetNotificationsAsync()
    {
        try
        {
            await _notificationClient.GetNotificationsAsync();
            await _notificationClient.ListenNotificationsAsync(_ => StateHasChanged());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetNotificationsAsync));
            Error = ex.Message;
        }
    }
}