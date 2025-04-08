using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

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
            var r = await _projectService.GetProjectsAsync();
            return Ok(r.Data);
        } catch
        {
            return BadRequest("");
        }
    }
}
