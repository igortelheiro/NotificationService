using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDbAdapter.DataAccess.Base;
using NotificationService.Extensions;
using NotificationService.Requests;
using NotificationService.Shared;

namespace NotificationService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IStore<Notification> _notificationStore;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public NotificationsController(IStore<Notification> notificationStore, IHubContext<NotificationHub> notificationHub)
    {
        _notificationStore = notificationStore;
        _notificationHub = notificationHub;
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateNotification(CreateNotificationRequest request)
    {
        try
        {
            var newNotification = request.ToNotification();
            
            await _notificationStore.CreateAsync(newNotification);
            await _notificationHub.Clients.All.SendAsync(nameof(Notification), newNotification);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Erro ao criar notificação",
                    Detail = ex.Message
                });
        }
    }


    // [HttpPost("{notificationId}")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task<ActionResult<IEnumerable<Notification>>> MarkNotificationAsReceived([FromRoute] Guid notificationId)
    // {
    //     try
    //     {
    //         var notification = (await _notificationStore.GetAsync(n => n.Id == notificationId)).FirstOrDefault();
    //         if (notification is null)
    //             return BadRequest(new ProblemDetails
    //             {
    //                 Title = "Notificação não encontrada"
    //             });
    //
    //         if (notification.Received)
    //             return BadRequest(new ProblemDetails
    //             {
    //                 Title = "Notificação já declarada como lida"
    //             });
    //
    //         notification.MarkAsReceived();
    //         await _notificationStore.UpdateAsync(notificationId, notification);
    //
    //         return Ok();
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(StatusCodes.Status500InternalServerError,
    //             new ProblemDetails
    //             {
    //                 Title = "Erro ao atualizar notificação",
    //                 Detail = ex.Message
    //             });
    //     }
    // }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications([FromQuery] Guid? userId)
    {
        try
        {
            var notifications = await _notificationStore.GetAllAsync();

            if (userId is not null && userId != Guid.Empty)
            {
                var clientNotifications = await _notificationStore.GetAsync(n => n.UserId == userId);
                notifications = notifications.Concat(clientNotifications);
            }

            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Erro ao consultar notificações",
                    Detail = ex.Message
                });
        }
    }
}