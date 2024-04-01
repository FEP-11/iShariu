using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using System.Threading.Tasks;

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
            return View(users);
        }

        [Route("Admin/OtherIndex")]
        public async Task<IActionResult> OtherIndex()
        {
            var users = await _dbService.GetAllUsers();
            return View(users);
        }
    }
}