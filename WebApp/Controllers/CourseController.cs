using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using System.Linq;
using MongoDB.Bson;

namespace WebApp.Controllers;

public class CourseController : Controller
{
    private readonly MongoDBService<Course> _courseService;
    private readonly MongoDBService<User> _userService;
    private readonly MongoDBService<Lesson> _lessonService;

    public CourseController(MongoDBService<Course> courseService, MongoDBService<User> userService, MongoDBService<Lesson> lessonService)
    {
        _courseService = courseService;
        _userService = userService;
        _lessonService = lessonService;
    }
    
    [HttpGet]
    [Route("/courses")]
    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var courses = await _courseService.GetAsync();
        ViewBag.User = await _userService.GetAsync(userId);
        
        ViewBag.Creator = new List<User>();
        
        foreach (var course in courses)
        {
            var creator = await _userService.GetAsync(course.CreatorId);
            ViewBag.Creator.Add(creator); 
        }
        
        return View(courses);   
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ViewBag.User = await _userService.GetAsync(userId);
        var languages = await GetLanguagesAsync();
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

    [NonAction]
    private async Task<List<string>> GetLanguagesAsync()
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
    
    [Route("/courses/lessons/{id}")]
    public async Task<IActionResult> Lessons(string id)
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ViewBag.User = await _userService.GetAsync(userId);
        var course = await _courseService.GetAsync(id);
        var lessons = new List<Lesson>();
        foreach (var lessonId in course.LessonIds)
        {
            var lesson = await _lessonService.GetAsync(lessonId);
            lessons.Add(lesson);
        }
        ViewBag.Lessons = lessons;
        
        return View(course);
    }
    
    public async Task<IActionResult> EditCourseAsync(string courseId)
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ViewBag.User = await _userService.GetAsync(userId);
        var course = await _courseService.GetAsync(courseId);
        var languages = await GetLanguagesAsync();
        ViewBag.Categories = new List<string> { "Programming", "Design", "Marketing", "Business", "Photography", "Music" };
        ViewBag.Languages = languages;
        
        return View(course);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditCourseAsync(Course course)
    {
        await _courseService.PutAsync(course);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> AddToLibraryAsync(string userId, string courseId)
    {
        ViewBag.User = await _userService.GetAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var fetchedUser = await _userService.GetAsync(userId);

        if (fetchedUser.EnrolledCourses.Any(c => c == courseId)) return BadRequest();
        
        fetchedUser.EnrolledCourses.Add(courseId);
        await _userService.PutAsync(fetchedUser);
        
        return RedirectToAction("index", "course");
    }
}
