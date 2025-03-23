using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class MembersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
