﻿using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]
public class AccountController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    public async Task<IActionResult> Index()
    {
        var model = new AccountViewModel();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _memberService.GetMemberByIdAsync(userId);
        if(result.Data != null)
        {
            model.CurrentUserAccount = result.Data;
            bool? useExternalprovider = await _memberService.MemberUseExternalProvider(userId);
            
            model.CurrentUserHasExternalprovider = useExternalprovider == false ? false : true;
        }  
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount([Bind(Prefix = "EditAccountForm")] MemberForm form)
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
        if (userId == null) return StatusCode(403, $"UnAuthorized to edit this account");

        var result = await _memberService.UpdateMemberAsync(form, userId);
        if (!result.Success) return StatusCode(result.StatusCode, $"Something went wrong, {result.ErrorMessage}");

        return Ok(new {success = true});
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePasswordForm")] ChangePasswordForm form)
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
        if (userId == null) return StatusCode(403, $"UnAuthorized to change this password");

        var result = await _memberService.ChangePasswordAsync(userId, form);
        if(!result.Success) return StatusCode(result.StatusCode, $"Something went wrong, {result.ErrorMessage}");

        return Ok(new {success = true, message = "Password changed successfully"});
    }

    [HttpDelete("deleteAccount/{id}")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        var result = await _memberService.DeleteMemberAsync(id);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, $"Failed to delete account, {result.ErrorMessage}");
        }

        return NoContent();
    }
}
