using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IEntityService<User> _userService;
        private readonly IEntityService<Course> _courseService;
        private const string UsersView = "Users";
        private const string CoursesView = "Courses";

        public AdminController(IEntityService<User> userService, IEntityService<Course> courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }

        [Route("dashboard")]
        public async Task<IActionResult> Index()
        {
            var bestSellingCreators = (await _userService.GetAsync())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var bestSellingCourses = (await _courseService.GetAsync())
                .OrderByDescending(u => u.RevenueGenerated)
                .Take(3)
                .ToList();

            var courses = await _courseService.GetAsync();
            ViewData["Courses"] = bestSellingCourses;

            var totalSales = courses.Sum(c => c.Sales);
            ViewData["Sales"] = totalSales;

            var totalRevenue = courses.Sum(c => c.RevenueGenerated);
            ViewData["Revenue"] = totalRevenue;

            var creators = (await _userService.GetAsync());
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

        [HttpPost("changeUserDetails")]
        public async Task<IActionResult> ChangeUserDetails(string userId, string username, string email, string password, string role)
        {
            var user = await _userService.GetAsync(userId);
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

            await _userService.PutAsync(user.Id, user);

            return RedirectToAction(UsersView);
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

            await _userService.PostAsync(newUser);

            return RedirectToAction(UsersView);
        }

        [HttpPost("deleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteAsync(userId);

            return RedirectToAction(UsersView);
        }

        [HttpGet("courses")]
        public async Task<IActionResult> Courses(int page = 1, string sortOrder = "courseName")
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

            await _courseService.PutAsync(course.Id, course);

            return RedirectToAction(CoursesView);
        }

        [HttpPost("deleteCourse")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            Console.WriteLine($"DeleteCourse method called with courseId: {courseId}");

            await _courseService.DeleteAsync(courseId);

            return RedirectToAction(CoursesView);
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

            return RedirectToAction(CoursesView);
        }
    }
}