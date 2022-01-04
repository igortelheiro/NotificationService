using Microsoft.AspNetCore.Mvc;
using MongoDbAdapter;
using NotificationService.Shared;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IStore<Notification> _notificationStore;

        public NotificationController(IStore<Notification> notificationStore) =>
            _notificationStore = notificationStore;


        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> CreateNotification(NotificationRequest request)
        {
            try
            {
                var newNotification = new Notification(request.Message, request.Link, request.TtlInDays, request.ClientId);

                await _notificationStore.CreateAsync(newNotification);
                return new CreatedResult(string.Empty, newNotification);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Erro ao criar notificação",
                    Detail = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IEnumerable<Notification>> GetNotifications() =>
            await _notificationStore.GetAsync();
    }
}
