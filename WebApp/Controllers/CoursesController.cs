using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class CoursesController : Controller
    {
        // GET
        public IActionResult CurrentCourse()
        {
            return View();
        }
        
        public IActionResult LessonsCourse()
        {
            return View("LessonsCourse");
        }
    }
}