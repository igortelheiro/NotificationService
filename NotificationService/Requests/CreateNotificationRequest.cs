﻿using System.ComponentModel.DataAnnotations;

namespace NotificationService.Requests
{
    public class CreateNotificationRequest
    {
        [Required]
        public string Message { get; set; }
        public string? Link { get; set; }
        public int? TtlInDays { get; set; }
        public Guid? ClientId { get; set; }
    }
}