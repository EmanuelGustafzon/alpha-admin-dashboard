using Business.Interfaces;
using Business.Models;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers;

[Authorize]
public class HomeController(IMemberService memberService, IProjectService projectService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    private readonly IProjectService _projectService = projectService;
    public async Task<IActionResult> Index()
    {
        var projectResult = await _projectService.GetProjectsAsync();
        var memberResult = await _memberService.GetAllMembersAsync();

        var model = new ProjectViewModel();
        model.Members = memberResult.Data;
        model.Projects = projectResult.Data;
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
        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _projectService.CreateProjectAsync(form, userId);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> Projects()
    {
        try
        {
            var result = await _projectService.GetProjectsAsync();
            return Ok(result.Data);
        } catch
        {
            return BadRequest("");
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
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpPost("updateProject/{id}")]
    public async Task<IActionResult> UpdateProject(string id, [Bind(Prefix = "ProjectForm")] ProjectForm form)
    {
        await _projectService.GetProjectAsync(id);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProject([Bind(Prefix = "DeleteProject")] DeleteProject form )
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
        var result = await _projectService.DeleteProjectAsync(form.Id);
        if(!result.Success) return StatusCode(result.StatusCode, $"Failed to Delete project :: {result.ErrorMessage}");
        return NoContent();     
    }
}
