namespace NotificationService.Shared
{
    public class NotificationRequest
    {
        public string Message { get; set; }
        public string? Link { get; set; }
        public int? TtlInDays { get; set; }
        public Guid? ClientId { get; set; }
    }
}
