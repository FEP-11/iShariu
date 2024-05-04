using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly MongoDBService _users;

        public AccountController(ILogger<AccountController> logger, MongoDBService settings)
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
        [HttpGet]
        public ActionResult Register() => View();

        [Authorize]
        public async Task<ActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("user/{username}")]
        public async Task<IActionResult> UserProfile(string username)
        {
            var user = await _users.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            var model = new User
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Location = user.Location
            };

            return View(model);
        }
        
        [Authorize]
        [HttpPost("user/{username}")]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            // Check if the username of the currently logged in user matches the username provided in the URL
            if (User.Identity.Name != model.Username)
            {
                return Forbid();
            }

            var user = await _users.GetByUsernameAsync(model.Username);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.Location = model.Location;

            // If a new password is provided, update the password
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = model.Password;
            }

            await _users.PutAsync(user.Id, user);

            return RedirectToAction("UserProfile", new { username = model.Username });
        }
        
        [Authorize]
        [HttpGet("user/{username}/edit")]
        public async Task<IActionResult> EditProfile(string username)
        {
            if (User.Identity.Name != username)
            {
                return Forbid();
            }

            var user = await _users.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            // Fetch the list of countries
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://restcountries.com/v3.1/all");
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var countries = JsonSerializer.Deserialize<List<Country>>(responseString, options);

            // Pass the list of countries to the view
            ViewData["Countries"] = countries;

            return View(user);
        }
        
    }
}