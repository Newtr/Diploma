using Microsoft.AspNetCore.Mvc;

namespace EvacProject.Controllers;

public class ModelSituationController : Controller
{
    public IActionResult CreatePage()
    {
        return View("ModelSituationPage");
    }
}