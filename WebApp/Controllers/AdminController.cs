using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly MongoDBService _dbService;

        public AdminController(MongoDBService dbService)
        {
            _dbService = dbService;
        }

        [Route("Admin/Index")]
        public async Task<IActionResult> Index()
        {
            var users = (await _dbService.GetAllUsers())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var courses = await _dbService.GetBestSellingCoursesAsync();
            ViewData["Courses"] = courses;
            
            var totalSales = courses.Sum(c => c.Sales);
            ViewData["Sales"] = totalSales;
            
            var totalRevenue = courses.Sum(c => c.RevenueGenerated);
            ViewData["Revenue"] = totalRevenue;
            
            var creatorsCount = users.Count(u => u.Role == "creator");
            ViewData["Creators"] = creatorsCount;
            
            return View(users);
        }
    }
}