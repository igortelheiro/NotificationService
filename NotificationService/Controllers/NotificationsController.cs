using Microsoft.AspNetCore.Mvc;
using MongoDbAdapter.DataAccess.Base;
using NotificationService.Requests;
using NotificationService.Shared;

namespace NotificationService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IStore<Notification> _notificationStore;

    public NotificationsController(IStore<Notification> notificationStore) =>
        _notificationStore = notificationStore;


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateNotification(CreateNotificationRequest request)
    {
        try
        {
            var newNotification = new Notification(Guid.NewGuid(),
                                                    DateTime.UtcNow,
                                                    request.Message,
                                                    request.Link,
                                                    request.TtlInDays,
                                                    request.ClientId);

            await _notificationStore.CreateAsync(newNotification);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Erro ao criar notificação",
                Detail = ex.Message
            });
        }
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(Guid? clientId)
    {
        try
        {
            var notifications = await _notificationStore.GetAllAsync();

            if (clientId is not null)
            {
                var clientNotifications = await _notificationStore.GetAsync(n => n.ClientId == clientId);
                notifications = notifications.Concat(clientNotifications);
            }

            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Erro ao consultar notificações",
                Detail = ex.Message
            });
        }
    }
}