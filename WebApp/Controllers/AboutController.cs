using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AboutController : Controller
    {
        private readonly MongoDBService<User> _userService;

        public AboutController( MongoDBService<User> userService)
        {
            _userService = userService;
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private async Task<IActionResult> RenderViewAsync(string viewName)
        {
            var user = await GetCurrentUserAsync();
            return View(viewName, user);
        }

        public Task<IActionResult> Index() => RenderViewAsync(nameof(Index));
        
        [Route("/about/terms")]
        public Task<IActionResult> Terms() => RenderViewAsync(nameof(Terms));

        [Route("/about/privacy")]
        public Task<IActionResult> Privacy() => RenderViewAsync(nameof(Privacy));
    }
}