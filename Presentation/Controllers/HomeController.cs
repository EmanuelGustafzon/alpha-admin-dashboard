using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using Presentation.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]
public class HomeController(IMemberService memberService, IProjectService projectService, INotificationService notificationService, IHubContext<NotificationHub> hub) : Controller
{
    private readonly IMemberService _memberService = memberService;
    private readonly IProjectService _projectService = projectService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _hub = hub;
    public async Task<IActionResult> Index()
    {
        var projectResult = await _projectService.GetProjectsAsync();
        var memberResult = await _memberService.GetAllMembersAsync();

        var model = new ProjectViewModel();
        model.Members = memberResult.Data ?? [];
        model.Projects = projectResult.Data ?? [];
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject([Bind(Prefix = "ProjectForm")]  ProjectForm form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, errors });
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId)) return Unauthorized(new {success = false});

        var result = await _projectService.CreateProjectAsync(form, userId);
        if(result.Data is null || result.Success is false) return StatusCode(500, "Failed to Adding projects.");

        NotificationForm notificationForm = NotificationFactory.CreateForm($"{result.Data.ProjectName} Added", result.Data.ImageUrl);
        var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
        if(notificationResult.Data is not null)
        {
            await _hub.Clients.All.SendAsync("generalNotifications", notificationResult.Data);
        }

        return Ok(new { success = true});
    }

    [HttpGet]
    public async Task<IActionResult> Projects([FromQuery] string? query = null)
    {
        try
        {
            if(query is not null)
            {
                var queryResult = await _projectService.GetProjectsAsync(query);
                if (queryResult.Data is null)
                {
                    return NotFound("No matching projects found.");
                }
                return Ok(queryResult.Data);
            }
            var result = await _projectService.GetProjectsAsync();
            if (result.Data is null)
            {
                return NotFound("No matching projects found.");
            }
            return Ok(result.Data);
        } catch
        {
            return StatusCode(500, "An error occurred while fetching projects.");
        }
    }

    [HttpGet("project/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        try
        {
            var result = await _projectService.GetProjectAsync(id);

            if (result == null || result.Data == null)
            {
                return NotFound(); 
            }

            return Ok(result.Data); 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpPost("updateProject/{id}")]
    public async Task<IActionResult> UpdateProject(string id, [Bind(Prefix = "ProjectForm")] ProjectForm form)
    {
        try
        {
            var result = await _projectService.UpdateProjectAsync(form, id);
            if(result.Data is null) return NotFound("No matching projects found.");

            NotificationForm notificationForm = NotificationFactory.CreateForm($"{result.Data.ProjectName} Updated", result.Data.ImageUrl);
            var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
            if (notificationResult.Data is not null)
            {
                await _hub.Clients.All.SendAsync("generalNotifications", notificationResult.Data);
            }
            return Ok(new {success = true});
        } catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpPost("updateStatus/{id}")]
    public async Task<IActionResult> UpdateStatus(string id, string status)
    {
        try
        {
            var result = await _projectService.UpdateStatusAsync(id, status);
            if(!result.Success || result.Data is null) return StatusCode(result.StatusCode, $"{result.ErrorMessage}");

            NotificationForm notificationForm = NotificationFactory.CreateForm($"{result.Data.ProjectName} Updated", result.Data.ImageUrl);
            var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
            if (notificationResult.Data is not null)
            {
                await _hub.Clients.All.SendAsync("generalNotifications", notificationResult.Data);
            }

            return Ok("Successfully updated the status");

        } catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpPost("updateProjectMembers/{id}")]
    public async Task<IActionResult> UpdateMembers(string id, [Bind(Prefix = "ProjectMembersForm")] ProjectMembersForm form)
    {
        try
        {
            var result = await _projectService.UpdateProjectMembersAsync(form, id);
            if (!result.Success || result.Data is null) return StatusCode(result.StatusCode, $"{result.ErrorMessage}");

            NotificationForm notificationForm = NotificationFactory.CreateForm($"{result.Data.ProjectName} Updated", result.Data.ImageUrl);
            var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
            if (notificationResult.Data is not null)
            {
                await _hub.Clients.All.SendAsync("generalNotifications", notificationResult.Data);
            }

            return Ok(new {success = true});

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpDelete("deleteProject/{id}")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if(!result.Success) return StatusCode(result.StatusCode, $"Failed to Delete project :: {result.ErrorMessage}");

        return NoContent();     
    }
}
