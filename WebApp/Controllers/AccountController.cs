using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly MongoDBService<User> _users; 

        public AccountController(ILogger<AccountController> logger, MongoDBService<User> settings)
        {
            _logger = logger;
            _users = settings;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignIn() => View();
        
        [AllowAnonymous]    
        [HttpPost("/account/signin")]
        public async Task<ActionResult> SignInAsync([FromForm] User user)
        {
            List<User> users = await _users.GetAsync();
            User? foundedUser = users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (foundedUser == null) return BadRequest();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, foundedUser.Username),
                new Claim(ClaimTypes.Role, UserRole.Admin)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            _logger.LogInformation("Authorization has been successful");
            
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register() => View();

        [Authorize]
        public async Task<ActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}