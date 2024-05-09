using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoDBService<User> _userService;
        private readonly MongoDBService<Course> _courseService;
        private readonly IConfiguration _configuration;

        public AccountController( MongoDBService<User> settings, MongoDBService<Course> courseService, IConfiguration configuration)
        {
            _userService = settings;
            _courseService = courseService;
            _configuration = configuration;
        }
        
        [AllowAnonymous]
        [HttpGet("/account/signin")]
        public IActionResult SignIn()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost("/account/signin")]
        public async Task<ActionResult> SignInAsync([FromForm] User user)
        {
            User? foundedUser = await GetUserIfExists(user.Username, user.Password);

            if (foundedUser == null)
                return BadRequest("Invalid username or password.");

            await SignInUser(foundedUser);
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
        }

        private async Task<User?> GetUserIfExists(string username, string password)
        {
            List<User> users = await _userService.GetAsync();
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
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
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetAsync(id);
            if (user == null)
                return NotFound();

            var createdCourses = await GetCreatedCourses(user);
            var enrolledCourses = await GetEnrolledCourses(user);

            var model = new UserProfileViewModel
            {
                User = user,
                CreatedCourses = createdCourses,
                EnrolledCourses = enrolledCourses
            };

            return View(model);
        }

        private async Task<List<Course>> GetCreatedCourses(User user)
        {
            var createdCourses = new List<Course>();
            foreach (var courseId in user.CreatedCourses)
            {
                var course = await _courseService.GetAsync(courseId);
                if (course != null) 
                    createdCourses.Add(course);
            }
            return createdCourses;
        }

        private async Task<List<Course>> GetEnrolledCourses(User user)
        {
            var enrolledCourses = new List<Course>();
            foreach (var courseId in user.EnrolledCourses)
            {
                var course = await _courseService.GetAsync(courseId);
                if (course != null)
                    enrolledCourses.Add(course);
            }
            return enrolledCourses;
        }

        [Authorize]
        [HttpPost("user/{id}")]
        public async Task<IActionResult> UpdateProfile(User model, string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id)
                return Forbid();

            var user = await _userService.GetAsync(id);
            if (user == null)
                return NotFound();

            UpdateUserProperties(user, model);

            await _userService.PutAsync(user);
            return RedirectToAction("UserProfile", new { id = model.Id });
        }

        private void UpdateUserProperties(User user, User model)
        {
            var properties = typeof(User).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                if (value != null && property.CanWrite)
                    property.SetValue(user, value);
            }
        }

        [Authorize]
        [HttpGet("user/{id}/settings")]
        public async Task<IActionResult> UserSettings(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id)
                return Forbid();

            var user = await _userService.GetAsync(id);
            if (user == null) 
                return NotFound();

            var countries = await GetCountries();
            ViewData["Countries"] = countries;

            return View(user);
        }

        private async Task<List<Country>?> GetCountries()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_configuration["RestCountriesAPI"]);
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<List<Country>>(responseString, options);
        }

        [Authorize]
        [HttpPost("/account/changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetAsync(userId);
            if (user == null)
                return NotFound();

            if (user.Password != model.CurrentPassword)
                return Json(new { success = false, message = "Current password is incorrect." });

            if (model.NewPassword == model.CurrentPassword)
                return Json(new { success = false, message = "New password cannot be the same as the current password." });

            user.Password = model.NewPassword;
            
            await _userService.PutAsync(user);
            return Json(new { success = true });
        }

        [Authorize]
        [HttpPost("/account/updatesetting")]
        public async Task<IActionResult> UpdateSetting([FromBody] UpdateSettingModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetAsync(userId);
            if (user == null)
                return NotFound();

            switch (model.SettingName)
            {
                case "AllowAccessToAgeRestrictedContent":
                    user.AllowAccessToAgeRestrictedContent = model.SettingValue;
                    break;
                case "UseDataToImproveIShariu":
                    user.UseDataToImproveIShariu = model.SettingValue;
                    break;
                default:
                    return BadRequest();
            }

            await _userService.PutAsync(user);
            return Ok();
        }

        [Authorize]
        [HttpPost("/account/deleteaccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetAsync(userId);
            if (user == null)
                return NotFound();

            if (user.Password != model.Password)
                return Json(new { success = false, message = "Incorrect password." });

            await _userService.DeleteAsync(user.Id);
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { success = true });
        }
    }
}