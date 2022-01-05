namespace NotificationService.Shared;

public record Notification : IEntity
{
    public Guid Id { get; init; }
    public DateTime CreationDate { get; init; }
    public string Title { get; init; }
    public string Message { get; init; }
    public string? Link { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public Guid? ClientId { get; init; }
    public bool Received { get; protected set; }


    public Notification(Guid id,
                        DateTime creationDate,
                        string title,
                        string message,
                        string? link,
                        int? ttlInDays,
                        Guid? clientId,
                        bool received = false)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(creationDate);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(message);

        Id = id;
        CreationDate = creationDate;
        Title = title;
        Message = message;
        Link = link;
        ExpiresAt = ttlInDays is not null ? DateTime.UtcNow.AddDays((int)ttlInDays) : null;
        ClientId = clientId;
        Received = received;
    }


    public void MarkAsReceived() => Received = true;
}