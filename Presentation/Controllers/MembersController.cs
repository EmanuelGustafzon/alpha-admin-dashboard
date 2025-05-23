﻿using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class MembersController(IMemberService memberService, INotificationService notificationService, IHubContext<NotificationHub> hub) : Controller
{
    private readonly IMemberService _memberService = memberService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _hub = hub;
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
                return NotFound( new {success = false});
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

            return Ok(new {success = true, message = useExternalprovider == false ? false : true });
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
        if (result.Data is null) return StatusCode(500, new { message = "Failed to add member." });

        var findMemberResult = await _memberService.GetMemberByEmailAsync(form.Email);
        if(findMemberResult.Data is not null)
        {
            await SendMessage($"{findMemberResult.Data.FirstName} {findMemberResult.Data.LastName} Added", findMemberResult.Data.ImageUrl);
        }

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

            await SendMessage($"{result.Data.FirstName} {result.Data.LastName} Updated", result.Data.ImageUrl);

            return Ok(new {success = true });
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

    private async Task<bool> SendMessage(string message, string? icon)
    {
        NotificationForm notificationForm = NotificationFactory.CreateForm(message, "Admin", icon ?? "/images/default-profile-picture.png");
        var notificationResult = await _notificationService.AddNotficationAsync(notificationForm);
        if (notificationResult.Data is not null)
        {
            var adminsResult = await _memberService.GetAllAdminsAsync();
            if (adminsResult.Data is not null)
            {
                foreach (var admin in adminsResult.Data)
                {
                    await _hub.Clients.User(admin.Id).SendAsync("adminNotifications", notificationResult.Data);
                }
            }
            return true;
        }
        return false;
    }
}
