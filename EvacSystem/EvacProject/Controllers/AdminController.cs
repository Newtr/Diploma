using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EvacProject.GENERAL.Data;
using EvacProject.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

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
                if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
                {
                    _logger.LogWarning("SimulateEmergency: Session invalid, redirecting to Login");
                    return RedirectToAction("Login");
                }

                await _botService.StartEvacuationTestAsync(_dbContext, CancellationToken.None);
                ViewBag.Message = "Тестовая эвакуация успешно запущена.";
                _logger.LogInformation("SimulateEmergency: Evacuation test started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SimulateEmergency: Error starting evacuation test");
                ViewBag.Error = "Произошла ошибка при запуске тестовой эвакуации.";
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

        [HttpGet]
        public async Task<IActionResult> HelpMessages()
        {
            _logger.LogInformation("AdminController: HelpMessages called");
            var messages = await _dbContext.HelpMessages
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> ClearHelpMessages()
        {
            _logger.LogInformation("AdminController: ClearHelpMessages called, Session AdminAuthenticated={Session}, Method={Method}",
                HttpContext.Session.GetString("AdminAuthenticated"), HttpContext.Request.Method);
            try
            {
                if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
                {
                    _logger.LogWarning("ClearHelpMessages: Session invalid, redirecting to Login");
                    return RedirectToAction("Login");
                }

                if (!await _dbContext.Database.CanConnectAsync())
                {
                    _logger.LogError("ClearHelpMessages: Database connection failed");
                    TempData["Error"] = "Ошибка: нет соединения с базой данных.";
                    return RedirectToAction("HelpMessages");
                }

                var messages = await _dbContext.HelpMessages.ToListAsync();
                _logger.LogInformation("ClearHelpMessages: Found {Count} help messages to remove", messages.Count);

                if (messages.Any())
                {
                    _dbContext.HelpMessages.RemoveRange(messages);
                    int changes = await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("ClearHelpMessages: Removed {Count} help messages, changes saved: {Changes}", messages.Count, changes);
                    TempData["Message"] = $"Все сообщения ({messages.Count}) очищены.";
                }
                else
                {
                    _logger.LogInformation("ClearHelpMessages: No help messages found to remove");
                    TempData["Message"] = "Нет сообщений для очистки.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearHelpMessages: Error clearing help messages");
                TempData["Error"] = $"Ошибка при очистке сообщений: {ex.Message}";
            }
            _logger.LogInformation("ClearHelpMessages: Redirecting to HelpMessages");
            return RedirectToAction("HelpMessages");
        }
    }
}