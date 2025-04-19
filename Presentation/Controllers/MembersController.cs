using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

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

    [HttpGet]
    public async Task<IActionResult> AllMembers([FromQuery] string? query = null)
    {
        try
        {
            if (query is not null)
            {
                var queryResult = await _memberService.GetAllMembersAsync(query);
                if (queryResult.Data is null)
                {
                    return NotFound("No matching member found.");
                }
                return Ok(queryResult.Data);
            }

            var result = await _memberService.GetAllMembersAsync();
            if (result.Data is null)
            {
                return NotFound("No matching member found.");
            }
            return Ok(result.Data);
        }
        catch
        {
            return StatusCode(500, "An error occurred while fetching members.");
        }
    }

    [HttpGet("member/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        try
        {
            var result = await _memberService.GetMemberByIdAsync(id);

            if (result.Data is null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Could not fetch memebr");
        }
    }

    [HttpGet("memberUseExternalProvider/{id}")]
    public async Task<IActionResult> MemberUseExternalProvider(string id)
    {
        try
        {
            bool? useExternalprovider = await _memberService.MemberUseExternalProvider(id);

            return Ok(new {success = true, message = useExternalprovider != false || useExternalprovider != null });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Could not fetch memebr");
        }
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
        if (result.Data is null || result.Success is false) return StatusCode(500, "Failed to add member.");

        ViewBag.GeneratedPassword = $"{result.Data}";
        return Ok(new { success = true, message = $"Send this genereted password {result.Data} to the new memeber and tell the member to change password" });

    }

    [HttpPost("updateMember/{id}")]
    public async Task<IActionResult> UpdateMember(string id, [Bind(Prefix = "MemberWithRoleForm")] MemberWithRoleForm form)
    {
        try
        {
            var result = await _memberService.UpdateMemberAsync(form, id);
            if (result.StatusCode == 404) return NotFound(new { success = false, message = $"Member not found" });
            if (result.Data is null) return NotFound(new { success = false, message = $"Something went wrong, {result.ErrorMessage}" });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return StatusCode(500, "Something went wrong. Failed to update memeber");
        }
    }

    [HttpDelete("deleteMember/{id}")]
    public async Task<IActionResult> DeleteMember(string id)
    {
        var result = await _memberService.DeleteMemberAsync(id);
        if (!result.Success) return StatusCode(result.StatusCode, $"Something went wrong {result.ErrorMessage}");
        return NoContent();
    }
}
