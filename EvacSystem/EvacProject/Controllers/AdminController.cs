using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvacProject.GENERAL.Data;

namespace EvacProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITelegramBotService _botService; 
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext dbContext,
            ITelegramBotService botService,
            ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _botService = botService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            _logger.LogInformation("AdminController: Login called on port {Port}", HttpContext.Request.Host.Port);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(string userId)
        {
            _logger.LogInformation("AdminController: Authenticate called with userId={UserId}", userId);
            if (long.TryParse(userId, out long id))
            {
                var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.UserId == id);
                if (admin != null)
                {
                    HttpContext.Session.SetString("AdminAuthenticated", "true");
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Error = "Неверный UserID";
            return View("Login");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("AdminController: Index called on port {Port}", HttpContext.Request.Host.Port);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SimulateEmergency()
        {
            _logger.LogInformation("AdminController: SimulateEmergency called");
            try
            {
                await _botService.SendMessageToAllStudents("ВНИМАНИЕ! В здании университета произошло ЧС, немедленно эвакуируйтесь");
                ViewBag.Message = "Сообщение об эвакуации отправлено всем студентам.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending emergency message");
                ViewBag.Error = "Ошибка при отправке сообщения об эвакуации.";
            }
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestBot()
        {
            _logger.LogInformation("AdminController: TestBot called");
            try
            {
                await _botService.SendMessageToAllStudents("Тестовое сообщение от бота");
                return Content("Тестовое сообщение отправлено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestBot");
                return Content($"Ошибка: {ex.Message}");
            }
        }
    }
}