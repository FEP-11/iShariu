using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using System.Threading.Tasks;
using WebApp.Models;
using System.Linq;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly MongoDBService _dbService;

        public AdminController(MongoDBService dbService)
        {
            _dbService = dbService;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var bestSellingCreators = (await _dbService.GetAllCreators())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var courses = await _dbService.GetBestSellingCoursesAsync();
            ViewData["Courses"] = courses;

            var totalSales = courses.Sum(c => c.Sales);
            ViewData["Sales"] = totalSales;

            var totalRevenue = courses.Sum(c => c.RevenueGenerated);
            ViewData["Revenue"] = totalRevenue;

            var creators = (await _dbService.GetAllCreators());
            var creatorsCount = creators.Count(u => u.Role == "creator");
            ViewData["Creators"] = creatorsCount;

            return View(bestSellingCreators);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users(string role, int page = 1)
        {
            const int pageSize = 11;
            var users = await _dbService.GetAsync();

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role).ToList();
            }

            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = page;
            ViewData["HasMorePages"] = users.Count > page * pageSize;

            return View("Users", pagedUsers);
        }
        
        [HttpPost("changeUserDetails")]
        public async Task<IActionResult> ChangeUserDetails(string userId, string username, string email, string password, string role)
        {
            var user = await _dbService.GetAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = username;
            user.Email = email;
            if (!string.IsNullOrEmpty(password))
            {
                user.Password = password; // You should hash the password before storing it
            }
            user.Role = role;

            await _dbService.PutAsync(user.Id, user);

            return RedirectToAction("Users");
        }
        
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(string username, string email, string password, string role)
        {
            // Create a new user with the provided details
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password, // Set the password
                Role = role,
                // Set other properties as needed
            };

            // Add the new user to the database
            await _dbService.PostAsync(newUser);

            // Redirect back to the users page
            return RedirectToAction("Users");
        }
    }
}