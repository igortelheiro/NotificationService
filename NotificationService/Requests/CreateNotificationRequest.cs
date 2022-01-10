using System.ComponentModel.DataAnnotations;

namespace NotificationService.Requests;

public class CreateNotificationRequest
{
    [Required] public string Title { get; set; }
    [Required] public string Message { get; set; }
    public string? Link { get; set; }
    public int? TtlInDays { get; set; }
    public Guid? UserId { get; set; }
}
