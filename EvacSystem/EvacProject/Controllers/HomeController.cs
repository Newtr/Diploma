using Microsoft.AspNetCore.Mvc;

namespace EvacProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Map()
        {
            return View();
        }

        public IActionResult Simulator()
        {
            return View();
        }

        public IActionResult Statistics()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}