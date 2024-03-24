using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}