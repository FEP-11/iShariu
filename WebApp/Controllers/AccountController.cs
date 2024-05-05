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
                new Claim(ClaimTypes.NameIdentifier, foundedUser.Id),
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
        [HttpGet("user/{id}")]
        public async Task<IActionResult> UserProfile(string id = null)
        {
            if (id == null)
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            
            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var createdCourses = new List<Course>();
            foreach (var courseId in user.CreatedCourses)
            {
                var course = await _users.GetCourseAsync(courseId);
                if (course != null)
                {
                    createdCourses.Add(course);
                }
            }

            var enrolledCourses = new List<Course>();
            foreach (var courseId in user.EnrolledCourses)
            {
                var course = await _users.GetCourseAsync(courseId);
                if (course != null)
                {
                    enrolledCourses.Add(course);
                }
            }

            var model = new UserProfileViewModel
            {
                User = user,
                CreatedCourses = createdCourses,
                EnrolledCourses = enrolledCourses
            };

            return View(model);
        }
        
        [Authorize]
        [HttpPost("user/{id}")]
        public async Task<IActionResult> UpdateProfile(User model, string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id)
            {
                return Forbid();
            }

            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.Location = model.Location;
            user.PhoneNumber = model.PhoneNumber;
            user.Username = model.Username;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = model.Password;
            }

            await _users.PutAsync(user.Id, user);

            return RedirectToAction("UserProfile", new { id = model.Id });
        }
        
        [Authorize]
        [HttpGet("user/{id}/settings")]
        public async Task<IActionResult> UserSettings(string id)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id)
            {
                return Forbid();
            }

            var user = await _users.GetByIdAsync(id);
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

        [HttpGet("courses")]
        public async Task<IActionResult> Courses()
        {
            // Implement your courses logic here...
            return View();
        }
        
        
    }
}