using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class CourseController : Controller
{
    public IActionResult Index() => View();
    
    
}