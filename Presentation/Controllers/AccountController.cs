using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
public class AccountController(IMemberService memberService, IWebHostEnvironment webHostEnv) : Controller
{
    private readonly IWebHostEnvironment _webHostEnv = webHostEnv;
    private readonly IMemberService _memberService = memberService;
    public async Task<IActionResult> Index()
    {
        var model = new AccountViewModel();
        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _memberService.GetMemberByIdAsync(userId);
        model.CurrentUserAccount = result.Data;

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
        if(form.Image != null && form.Image.Length > 0)
        {
            var imageFolder = Path.Combine(_webHostEnv.WebRootPath, "image-uploads");
            Directory.CreateDirectory(imageFolder);

            var imagePath = Path.Combine(imageFolder, $"{Guid.NewGuid().ToString()}_{Path.GetFileName(form.Image.FileName)}");
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await form.Image.CopyToAsync(stream);
            }
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _memberService.UpdateMemberAsync(form, userId);
        return Ok();
    }
}
