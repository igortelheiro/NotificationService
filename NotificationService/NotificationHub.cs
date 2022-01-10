﻿using Microsoft.AspNetCore.SignalR;
using NotificationService.Shared;

namespace NotificationService;

public class NotificationHub : Hub
{
    public async Task SendNotification(Notification notification) =>
        await Clients.All.SendAsync(nameof(Notification), notification);
}