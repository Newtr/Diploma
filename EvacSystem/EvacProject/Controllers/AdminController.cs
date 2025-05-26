using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EvacProject.GENERAL.Data;
using EvacProject.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using EvacProject.GENERAL.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        
        [HttpGet]
        public async Task<IActionResult> StudentManagement()
        {
            _logger.LogInformation("AdminController: StudentManagement called");
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("StudentManagement: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            var students = await _dbContext.Students
                .Include(s => s.Faculty)
                .Include(s => s.FormOfStudy)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> StudentDetails(long id)
        {
            _logger.LogInformation("AdminController: StudentDetails called with UserId={id}", id);
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("StudentDetails: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            var student = await _dbContext.Students
                .Include(s => s.Faculty)
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(s => s.UserId == id);
            if (student == null)
            {
                _logger.LogWarning("Student not found with UserId={id}", id);
                return NotFound();
            }
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStudent()
        {
            _logger.LogInformation("AdminController: CreateStudent called");
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("CreateStudent: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            ViewBag.Faculties = await _dbContext.Faculties
                .Select(f => new SelectListItem { Value = f.FacultyId.ToString(), Text = f.Name })
                .ToListAsync();
            ViewBag.FormsOfStudy = await _dbContext.FormsOfStudy
                .Select(f => new SelectListItem { Value = f.FormOfStudyId.ToString(), Text = f.Name })
                .ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            _logger.LogInformation("AdminController: CreateStudent POST called");
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("CreateStudent: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            // Преобразуем даты в UTC
            if (student.AdmissionDate.HasValue)
                student.AdmissionDate = DateTime.SpecifyKind(student.AdmissionDate.Value, DateTimeKind.Utc);
            if (student.TicketIssueDate.HasValue)
                student.TicketIssueDate = DateTime.SpecifyKind(student.TicketIssueDate.Value, DateTimeKind.Utc);
            if (student.TicketExpiryDate.HasValue)
                student.TicketExpiryDate = DateTime.SpecifyKind(student.TicketExpiryDate.Value, DateTimeKind.Utc);

            if (ModelState.IsValid)
            {
                _dbContext.Students.Add(student);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Student created successfully, UserId={id}", student.UserId);
                TempData["Message"] = "Студент успешно добавлен.";
                return RedirectToAction("StudentManagement");
            }

            ViewBag.Faculties = await _dbContext.Faculties
                .Select(f => new SelectListItem { Value = f.FacultyId.ToString(), Text = f.Name })
                .ToListAsync();
            ViewBag.FormsOfStudy = await _dbContext.FormsOfStudy
                .Select(f => new SelectListItem { Value = f.FormOfStudyId.ToString(), Text = f.Name })
                .ToListAsync();
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(long id)
        {
            _logger.LogInformation("AdminController: EditStudent called with UserId={id}", id);
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("EditStudent: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            var student = await _dbContext.Students
                .FirstOrDefaultAsync(s => s.UserId == id);
            if (student == null)
            {
                _logger.LogWarning("Student not found with UserId={id}", id);
                return NotFound();
            }

            ViewBag.Faculties = await _dbContext.Faculties
                .Select(f => new SelectListItem { Value = f.FacultyId.ToString(), Text = f.Name })
                .ToListAsync();
            ViewBag.FormsOfStudy = await _dbContext.FormsOfStudy
                .Select(f => new SelectListItem { Value = f.FormOfStudyId.ToString(), Text = f.Name })
                .ToListAsync();
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(Student student)
        {
            _logger.LogInformation("AdminController: EditStudent POST called with UserId={id}", student.UserId);
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("EditStudent: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            // Преобразуем даты в UTC
            if (student.AdmissionDate.HasValue)
                student.AdmissionDate = DateTime.SpecifyKind(student.AdmissionDate.Value, DateTimeKind.Utc);
            if (student.TicketIssueDate.HasValue)
                student.TicketIssueDate = DateTime.SpecifyKind(student.TicketIssueDate.Value, DateTimeKind.Utc);
            if (student.TicketExpiryDate.HasValue)
                student.TicketExpiryDate = DateTime.SpecifyKind(student.TicketExpiryDate.Value, DateTimeKind.Utc);

            if (ModelState.IsValid)
            {
                var existingStudent = await _dbContext.Students.FindAsync(student.UserId);
                if (existingStudent == null)
                {
                    _logger.LogWarning("Student not found with UserId={id}", student.UserId);
                    return NotFound();
                }

                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
                existingStudent.Patronymic = student.Patronymic;
                existingStudent.StudentNumber = student.StudentNumber;
                existingStudent.FacultyId = student.FacultyId;
                existingStudent.FormOfStudyId = student.FormOfStudyId;
                existingStudent.AdmissionDate = student.AdmissionDate;
                existingStudent.TicketIssueDate = student.TicketIssueDate;
                existingStudent.TicketExpiryDate = student.TicketExpiryDate;
                existingStudent.TelegramChatId = student.TelegramChatId;
                existingStudent.CurrentState = student.CurrentState;
                existingStudent.SelectedCampus = student.SelectedCampus;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Student updated successfully, UserId={id}", student.UserId);
                TempData["Message"] = "Данные студента успешно обновлены.";
                return RedirectToAction("StudentManagement");
            }

            ViewBag.Faculties = await _dbContext.Faculties
                .Select(f => new SelectListItem { Value = f.FacultyId.ToString(), Text = f.Name })
                .ToListAsync();
            ViewBag.FormsOfStudy = await _dbContext.FormsOfStudy
                .Select(f => new SelectListItem { Value = f.FormOfStudyId.ToString(), Text = f.Name })
                .ToListAsync();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            _logger.LogInformation("AdminController: DeleteStudent called with UserId={id}", id);
            if (HttpContext.Session.GetString("AdminAuthenticated") != "true")
            {
                _logger.LogWarning("DeleteStudent: Session invalid, redirecting to Login");
                return RedirectToAction("Login");
            }

            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Student not found with UserId={id}", id);
                TempData["Error"] = "Студент не найден.";
                return RedirectToAction("StudentManagement");
            }

            try
            {
                _dbContext.Students.Remove(student);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Student deleted successfully, UserId={id}", id);
                TempData["Message"] = "Студент успешно удален.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with UserId={id}", id);
                TempData["Error"] = "Ошибка при удалении студента.";
            }
            return RedirectToAction("StudentManagement");
        }
    }
}