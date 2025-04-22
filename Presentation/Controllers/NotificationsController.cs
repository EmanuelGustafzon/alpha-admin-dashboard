using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using System.Diagnostics.Contracts;
using System.Security.Claims;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IHubContext<NotificationHub> hubContext, INotificationService notificationService) : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = hubContext;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> AddNotification(NotificationForm form)
    {
        var result = await _notificationService.AddNotficationAsync(form);
        if(result.Data == null) return StatusCode(500, new { success = false });

        await _notificationHub.Clients.All.SendAsync("generalNotifications", result.Data);
        return Ok(new {success = true});
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if(string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _notificationService.GetNotificationsAsync(userId);
        if(result.Data == null) return StatusCode(500, new {success = false});

        return Ok(result.Data);
    }

    [HttpPost("dissmiss/{id}")]
    public async Task<IActionResult> DissmissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _notificationService.DissmissNotification(userId, id);
        if(!result.Success) return StatusCode(500, new {success = false});
        await _notificationHub.Clients.User(userId).SendAsync("NotificationDissmissed", id);
        return Ok();
    }
}
