using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly MongoDBService<User> _dbService;

        public AdminController(MongoDBService<User> dbService)
        {
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            var users = (await _dbService.GetCreatorsAsync())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();
            return View(users);
        }
    }
}