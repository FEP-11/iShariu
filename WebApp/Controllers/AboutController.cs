using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers
{

    public class AboutController : Controller
    {
        private readonly MongoDBService<User> _userService;
        private readonly MongoDBService<Course> _courseService;

        public AboutController(MongoDBService<User> userService, MongoDBService<Course> courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private async Task<IActionResult> RenderViewAsync(string viewName, object model)
        {
            var user = await GetCurrentUserAsync();
            return View(viewName, new { User = user, Metrics = model });
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAsync();
            var courses = await _courseService.GetAsync();

            var metrics = new
            {
                UserCount = users.Count,
                CourseCount = courses.Count,
                CreatorCount = users.Count(u => u.Role == "creator")
            };

            return await RenderViewAsync(nameof(Index), metrics);
        }

        [Route("/about/terms")]
        public Task<IActionResult> Terms() => RenderViewAsync(nameof(Terms), null);

        [Route("/about/privacy")]
        public Task<IActionResult> Privacy() => RenderViewAsync(nameof(Privacy), null);
    }
}
        