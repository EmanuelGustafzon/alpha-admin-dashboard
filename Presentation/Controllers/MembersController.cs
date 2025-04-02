using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

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
}
