using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Shared;

[BsonIgnoreExtraElements]
public class Notification
{
    public string Message { get; }
    public string? Link { get; }
    public int? TtlInDays { get; }
    public Guid? ClientId { get; }
    public DateTime NotificationTime { get; }
    public bool Received { get; private set; }


    public Notification(string message, string? link, int? ttl, Guid? clientId)
    {
        ArgumentNullException.ThrowIfNull(message);

        Message = message;
        Link = link;
        TtlInDays = ttl;
        ClientId = clientId;
        
        NotificationTime = DateTime.UtcNow;
        Received = false;
    }


    public void MarkAsReceived() => Received = true;
}