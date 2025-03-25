using Microsoft.AspNetCore.Mvc;

namespace EvacProject.Controllers;

public class HomeController : Controller
{
    public IActionResult CreatePage()
    {
        return View("HomePage");
    }
}