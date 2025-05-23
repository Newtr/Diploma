using Microsoft.AspNetCore.Mvc;

namespace EvacProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HomeController: Index called");
            return Content("User Portal: Добро пожаловать в БГТУ");
        }
    }
}