using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WebApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly MongoDBService<Course> _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(MongoDBService<Course> courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Signin", "Account");
                course.CreatorId = userId;
                
                if (string.IsNullOrEmpty(course.Id))
                    course.Id = ObjectId.GenerateNewId().ToString();
                await _courseService.PostAsync(course);
                
                return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAsync();
            return View(courses);
        }
    }
}