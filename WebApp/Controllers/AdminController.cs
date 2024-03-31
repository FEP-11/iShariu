using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize(Policy = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}