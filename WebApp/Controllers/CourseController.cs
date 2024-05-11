using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;

namespace WebApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly MongoDBService<Course> _courseService;
        private readonly MongoDBService<User> _userService;
        private readonly MongoDBService<Lesson> _lessonService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(MongoDBService<Course> courseService, MongoDBService<User> userService, MongoDBService<Lesson> lessonService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _userService = userService;
            _lessonService = lessonService;
            _logger = logger;
        }
        
        [Authorize(Policy = "Creator")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var languages = await GetLanguages();
            ViewBag.Categories = new List<string> { "Programming", "Design", "Marketing", "Business", "Photography", "Music" };
            ViewBag.Languages = languages;
            
            return View(new Course());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course, List<Lesson> lessons)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Signin", "Account");
            course.CreatorId = userId;

            if (string.IsNullOrEmpty(course.Id)) course.Id = ObjectId.GenerateNewId().ToString();

            foreach (var lesson in lessons)
            {
                if (string.IsNullOrEmpty(lesson.Id))
                    lesson.Id = ObjectId.GenerateNewId().ToString();
                course.LessonIds.Add(lesson.Id);
                await _lessonService.PostAsync(lesson); 
            }

            await _courseService.PostAsync(course); 

            var user = await _userService.GetAsync(userId);
            user.CreatedCourses.Add(course.Id);
            await _userService.PutAsync(user);

            return RedirectToAction("Index");
        }

        private async Task<List<string>> GetLanguages()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://restcountries.com/v3.1/all");
            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);
            var languages = new List<string>();

            foreach (var country in document.RootElement.EnumerateArray())
            {
                if (country.TryGetProperty("languages", out JsonElement languagesElement))
                {
                    foreach (var language in languagesElement.EnumerateObject())
                    {
                        if (!languages.Contains(language.Value.GetString()))
                            languages.Add(language.Value.GetString());
                    }
                }
            }

            return languages;
        }

        [HttpGet]
        [Route("/courses")]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAsync();
            return View(courses);
        }
    }
}