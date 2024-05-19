using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Models;
using ZstdSharp.Unsafe;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly MongoDBService<User> _userService;
        private readonly MongoDBService<Course> _courseService;
        private readonly EntityService _entityService;
        private readonly MongoDBService<Message> _messageService;

        public AdminController(MongoDBService<User> userService, MongoDBService<Course> courseService, 
            EntityService entityService, MongoDBService<Message> messageService)
        {
            _userService = userService;
            _courseService = courseService;
            _entityService = entityService;
            _messageService = messageService;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var bestSellingCreators = (await _entityService.GetAllCreators())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var courses = await _entityService.GetBestSellingCoursesAsync();
            ViewData["Courses"] = courses;

            var totalSales = courses.Sum(c => c.Sales);
            ViewData["Sales"] = totalSales;

            var totalRevenue = courses.Sum(c => c.RevenueGenerated);
            ViewData["Revenue"] = totalRevenue;

            var creators = (await _entityService.GetAllCreators());
            var creatorsCount = creators.Count(u => u.Role == "creator");
            ViewData["Creators"] = creatorsCount;

            return View(bestSellingCreators);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users(string role, int page = 1)
        {
            const int pageSize = 9;
            var users = await _userService.GetAsync();

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role).ToList();
            }

            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = page;
            ViewData["HasMorePages"] = users.Count > page * pageSize;

            return View("Users", pagedUsers);
        }

        [HttpGet("messages")]
        public async Task<IActionResult> Messages()
        {
            var messages = await _messageService.GetAsync();
            return View(messages);
        }
        
        [HttpPost("deleteMessage")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            await _messageService.DeleteAsync(messageId);
            return RedirectToAction("Messages");
        }
        
        [HttpPost("changeUserDetails")]
        public async Task<IActionResult> ChangeUserDetails(string userId, string username, string email, string password, string role)
        {
            var user = await _userService.GetAsync(userId);
            if (user == null) 
                return NotFound();

            user.Username = username;
            user.Email = email;
            
            if (!string.IsNullOrEmpty(password))
                user.Password = password;
            
            user.Role = role;

            await _userService.PutAsync(user);

            return RedirectToAction("Users");
        }
        
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUserAsync(string username, string email, string password, string role)
        {
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password, 
                Role = role,
            };
            
            await _userService.PostAsync(newUser);
            
            return RedirectToAction("Users");
        }
        
        [HttpPost("deleteUser")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            await _userService.DeleteAsync(userId);
            
            return RedirectToAction("Users");
        }
        
        [HttpGet("courses")]
        public async Task<IActionResult> CoursesAsync(int page = 1, string sortOrder = "courseName")
        {
            const int pageSize = 9;
            var courses = await _courseService.GetAsync();
            
            switch (sortOrder)
            {
                case "courseName":
                    courses = courses.OrderBy(c => c.CourseName).ToList();
                    break;
                case "coursePrice":
                    courses = courses.OrderByDescending(c => c.CoursePrice).ToList();
                    break;
                case "sales":
                    courses = courses.OrderByDescending(c => c.Sales).ToList();
                    break;
                default:
                    courses = courses.OrderBy(c => c.CourseName).ToList();
                    break;
            }

            var pagedCourses = courses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = page;
            ViewData["HasMorePages"] = courses.Count > page * pageSize;

            return View(pagedCourses);
        }
        
        [HttpPost("changeCourseDetails")]
        public async Task<IActionResult> ChangeCourseDetails(string courseId, string courseName, decimal coursePrice)
        {
            var course = await _courseService.GetAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            course.CourseName = courseName;
            course.CoursePrice = coursePrice;

            await _courseService.PutAsync(course);

            return RedirectToAction("Courses");
        }
        
        [HttpPost("deleteCourse")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            Console.WriteLine($"DeleteCourse method called with courseId: {courseId}");

            await _courseService.DeleteAsync(courseId);

            return RedirectToAction("Courses");
        }
        
        [HttpPost("addCourse")]
        public async Task<IActionResult> AddCourse(string courseName, decimal coursePrice)
        {
            var newCourse = new Course
            {
                CourseName = courseName,
                CoursePrice = coursePrice,
            };
            
            await _courseService.PostAsync(newCourse);
            
            return RedirectToAction("Courses");
        }
        
        [Route("sales")]
        public async Task<IActionResult> Sales()
        {
            return View();
        }
        
        [Route("settings")]
        public async Task<IActionResult> Settings()
        {
            return View();
        }
        
    }
}