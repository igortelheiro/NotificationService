using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using NotificationService.Shared;
using RestSharp;

namespace ClientDemo;

public class NotificationClient
{
    private readonly ILogger<NotificationClient> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _notificationServiceUri;

    public List<Notification> Notifications { get; private set; }

    public NotificationClient(ILogger<NotificationClient> logger,
                              IServiceProvider serviceProvider,
                              IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _notificationServiceUri = configuration.GetConnectionString("NotificationService");
        
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(_notificationServiceUri);

        Notifications = new();
    }
    
    
    public async Task GetNotificationsAsync()
    {
        var userId = await GetUserIdAsync();

        var httpClient = new RestClient(_notificationServiceUri);
        var request = new RestRequest("Notifications")
            .AddQueryParameter(nameof(userId), userId);
        
        var response = await httpClient.ExecuteAsync<IEnumerable<Notification>>(request);

        if (response.ErrorException is not null)
            throw response.ErrorException;

        if (response.Data is not null)
            Notifications = Notifications
                .Union(response.Data)
                .ToList();
    }


    public async Task ListenNotificationsAsync(Action<Notification>? callback)
    {
        await using var hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_notificationServiceUri}/notifications")
            .Build();
        
        hubConnection.On<Notification>(nameof(Notification), async notification =>
        {
            var userId = await GetUserIdAsync();
            var shouldReceiveNotification = notification.UserId is null
                                              || notification.UserId == Guid.Empty
                                              || notification.UserId == userId;
            if (shouldReceiveNotification)
            {
                Notifications.Add(notification);
                callback?.Invoke(notification);
            }
        });

        await hubConnection.StartAsync();
    }


    private async Task<Guid> GetUserIdAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var authenticationStateProvider = scope.ServiceProvider.GetService<AuthenticationStateProvider>();
        ArgumentNullException.ThrowIfNull(authenticationStateProvider);

        var userId = Guid.Empty;
        
        try
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity?.IsAuthenticated ?? false)                                                           
            {
                var userIdClaim = user.FindFirst(c => c.Type == "id")?.Value;
                Guid.TryParse(userIdClaim, out userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar usuário");
        }

        return userId;
    }
}
