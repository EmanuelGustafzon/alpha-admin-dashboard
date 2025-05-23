﻿using Business.Models;

namespace Business.Factories
{
    public static class NotificationFactory
    {
        public static NotificationForm CreateForm(string message, string target = "All", string? icon = null, string? action = null)
        {
            return new NotificationForm
            {
                Icon = icon,
                Message = message,
                Action = action,
                Target = target
            };
        }
    }
}
