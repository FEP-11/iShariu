using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MongoDBService<User> _userCollection;

    public HomeController(ILogger<HomeController> logger, MongoDBService<User> userCollection)
    {
        _logger = logger;
        _userCollection = userCollection;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userCollection.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(user);
    }
     
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
