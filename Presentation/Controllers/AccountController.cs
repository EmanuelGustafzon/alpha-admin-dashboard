using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
public class AccountController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    public async Task<IActionResult> Index()
    {
        var model = new AccountViewModel();
        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _memberService.GetMemberByIdAsync(userId);
        if(result.Data != null)
        {
            model.CurrentUserAccount = result.Data;
            bool? useExternalprovider = await _memberService.MemberUseExternalProvider(userId);
            model.CurrentUserHasExternalprovider = useExternalprovider != false || useExternalprovider != null;
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
        var result = await _memberService.UpdateMemberAsync(form, userId);
        return Ok();
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
