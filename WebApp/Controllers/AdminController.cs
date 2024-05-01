using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using System.Threading.Tasks;
using WebApp.Models;
using System.Linq;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly MongoDBService _dbService;

        public AdminController(MongoDBService dbService)
        {
            _dbService = dbService;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var bestSellingCreators = (await _dbService.GetAllCreators())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var courses = await _dbService.GetBestSellingCoursesAsync();
            ViewData["Courses"] = courses;

            var totalSales = courses.Sum(c => c.Sales);
            ViewData["Sales"] = totalSales;

            var totalRevenue = courses.Sum(c => c.RevenueGenerated);
            ViewData["Revenue"] = totalRevenue;

            var creators = (await _dbService.GetAllCreators());
            var creatorsCount = creators.Count(u => u.Role == "creator");
            ViewData["Creators"] = creatorsCount;

            return View(bestSellingCreators);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users(string role, int page = 1)
        {
            const int pageSize = 9;
            var users = await _dbService.GetAsync();

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role).ToList();
            }

            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = page;
            ViewData["HasMorePages"] = users.Count > page * pageSize;

            return View("Users", pagedUsers);
        }
        
        [HttpPost("changeUserDetails")]
        public async Task<IActionResult> ChangeUserDetails(string userId, string username, string email, string password, string role)
        {
            var user = await _dbService.GetAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = username;
            user.Email = email;
            if (!string.IsNullOrEmpty(password))
            {
                user.Password = password;
            }
            user.Role = role;

            await _dbService.PutAsync(user.Id, user);

            return RedirectToAction("Users");
        }
        
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(string username, string email, string password, string role)
        {
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password, 
                Role = role,
            };
            
            await _dbService.PostAsync(newUser);
            
            return RedirectToAction("Users");
        }
        
        [HttpPost("deleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _dbService.DeleteUserAsync(userId);
            
            return RedirectToAction("Users");
        }
        
        [HttpGet("courses")]
        public async Task<IActionResult> Courses(int page = 1, string sortOrder = "courseName")
        {
            const int pageSize = 9;
            var courses = await _dbService.GetCoursesAsync();
            
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
            var course = await _dbService.GetCourseAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            course.CourseName = courseName;
            course.CoursePrice = coursePrice;

            await _dbService.PutCourseAsync(course.Id, course);

            return RedirectToAction("Courses");
        }
        
        [HttpPost("deleteCourse")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            Console.WriteLine($"DeleteCourse method called with courseId: {courseId}");

            await _dbService.DeleteCourseAsync(courseId);

            return RedirectToAction("Courses");
        }
        
        [HttpPost("addCourse")]
        public async Task<IActionResult> AddCourse(string courseName, decimal coursePrice)
        {
            // Create a new course with the provided details
            var newCourse = new Course
            {
                CourseName = courseName,
                CoursePrice = coursePrice,
                // Set other properties as needed
            };

            // Add the new course to the database
            await _dbService.PostCourseAsync(newCourse);

            // Redirect back to the courses page
            return RedirectToAction("Courses");
        }
    }
}