using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
public class HomeController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    public async Task<IActionResult> Index()
    {
        var model = new ProjectViewModel();
        var result = await _memberService.GetAllMembersAsync();
        model.Members = result.Data;
        return View(model);
    }
}
