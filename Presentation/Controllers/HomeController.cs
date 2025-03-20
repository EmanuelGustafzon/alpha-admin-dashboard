using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]

public class HomeController : Controller
{
    private ProjectViewModel _projectViewModel { get; set; }

    public HomeController()
    {
        _projectViewModel = new ProjectViewModel();
        _projectViewModel.Projects = [
             new Project{ProjectName = "cool project", ClientName = "GitLab", Description = "awsome", StartDate = new DateTime(2025, 01, 05), EndDate = new DateTime(2025, 01, 06), Budget = 10}
             ];
    }
    public IActionResult Index()
    {
        return View(_projectViewModel);
    }

    [HttpPost]
    public IActionResult Create(ProjectCreateFormModel form)
    {
        _projectViewModel.CreateProjectForm = form;
        return View("index", _projectViewModel);
    }
}
