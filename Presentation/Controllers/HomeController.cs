using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class HomeController() : Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }
}
