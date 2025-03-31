using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
public class AccountController(IWebHostEnvironment webHostEnv) : Controller
{
    private readonly IWebHostEnvironment _webHostEnv = webHostEnv;
    public IActionResult Index()
    {
        var model = new AccountViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount([Bind(Prefix = "EditAccountForm")] EditAccountForm form)
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
        return Ok();
    }
}
