using Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        var form = new EditAccountForm();
        return View(form);
    }

    [HttpPost]
    public IActionResult EditAccount(EditAccountForm form)
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
        return View("index");
    }
}
