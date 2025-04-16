using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class MembersController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    public async Task<IActionResult> Index()
    {
        var model = new MembersViewModel();
        var result = await _memberService.GetAllMembersAsync();
        if(result.Success)
        {
            model.Members = result.Data!;
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddMember([Bind(Prefix = "MemberWithRoleForm")] MemberWithRoleForm form)
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
        var result = await _memberService.CreateMemberAsync(form);
        if (result.Data is null || result.Success is false) return StatusCode(500, "Failed to Adding projects.");

        ViewBag.GeneratedPassword = $"{result.Data}";
        return Ok(new { success = true, message = $"Send this genereted password {result.Data} to the memeber and change password" });

    }
}
