using BGProcess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TestMVC.Data;
using TestMVC.Models;
using TestMVC.Services;
using BGProcess.Interface;

namespace TestMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersService _usersService;
        public readonly EmailService _emailService;
        public readonly TestMVCContext _context;
        private readonly IEmailQueue _emailQueue;

        public UsersController(UsersService usersService, EmailService emailService, TestMVCContext context, IEmailQueue emailQueue)
        {
            _usersService = usersService;
            _emailService = emailService;
            _context = context;
            _emailQueue = emailQueue;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? keyword = "")
        {
            IQueryable<User> query = _context.User.Where(u => u.DeletedAt == null);

            // If a keyword is provided, filter the users accordingly
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.Name.ToLower() == keyword.ToLower());
            }

            // Get the total count of users based on the filtered query
            int totalUsers = await query.CountAsync();

            // Apply pagination
            List<User> users = await query
                .OrderByDescending(u => u.Id) // Order by ID or any specific column
                .Skip((page - 1) * pageSize) // Skip the previous pages
                .Take(pageSize) // Take the current page size
                .ToListAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            UserViewModel model = new()
            {
                Users = users,
                CurrentPage = page,
                TotalPages = totalPages
            };

            // Pass the keyword to the view
            ViewBag.Keyword = keyword;

            return View(model);
        }



        public async Task<IActionResult> Details(int id)
        {
            var user = await _usersService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Address")] User user)
        {
            if (ModelState.IsValid)
            {
                await _usersService.CreateUserAsync(user);
                TempData["AlertMessage"] = "Your data has been saved successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _usersService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Address")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _usersService.EditUserAsync(user);
                TempData["AlertMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _usersService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _usersService.RemoveUserAsync(id);
            TempData["AlertMessage"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Send(int id)
        {
            var user = await _usersService.GetUserAsync(id);
            return View(user);
        }

        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string email, string subject, string message)
        {
            await _emailQueue.EnqueueEmail(email, subject, message); // Enqueue email for background processing
            TempData["AlertMessage"] = "Email has been sent successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailQueue.EnqueueEmail("mochamadaris112@gmail.com", "WELCOME TO ARS OFFICE", "Welcome");
            Thread.Sleep(100);
            return Ok("Email queued.");
        }

        public IActionResult SendBulkEmails()
        {
            for (int i = 0; i < 5; i++)
            {
                _emailQueue.EnqueueEmail("mochamadaris112@gmail.com", $"Test Subject {i}", $"Test Message {i}");
            }
            return Ok("Bulk emails have been queued.");
        }      
    }
}
