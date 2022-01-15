using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using NotificationService.Shared;
using RestSharp;

namespace ClientDemo;

public class NotificationClient
{
    private readonly ILogger<NotificationClient> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _notificationServiceUrl;

    public List<Notification> Notifications { get; private set; }

    public NotificationClient(ILogger<NotificationClient> logger,
                              IServiceProvider serviceProvider,
                              IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _notificationServiceUrl = configuration.GetConnectionString("NotificationService");
        
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(_notificationServiceUrl);

        Notifications = new();
    }
    
    
    public async Task GetNotificationsAsync()
    {
        var userId = await GetUserIdAsync();

        var httpClient = new RestClient(_notificationServiceUrl);
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
        var hubConnection = BuildHubConnection();
        
        hubConnection.On<Notification>(nameof(Notification), async notification =>
        {
            _logger.LogTrace("Notificação recebida: {notification}", notification);
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
        _logger.LogTrace("Trigger definido para Notification");

        await hubConnection.StartAsync();
        _logger.LogInformation("");
    }
    
    
    private HubConnection BuildHubConnection() =>
        new HubConnectionBuilder()
            .WithUrl($"{_notificationServiceUrl}/pushNotifications")
            .WithAutomaticReconnect()
            .Build();


    private async Task<Guid> GetUserIdAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
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
