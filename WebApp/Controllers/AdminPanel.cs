using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AdminPanel : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}