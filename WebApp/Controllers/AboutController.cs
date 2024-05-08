using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class AboutController : Controller
{
    private readonly ILogger<AboutController> _logger;
    private readonly MongoDBService<User> _userService;
    
    public AboutController(ILogger<AboutController> logger, MongoDBService<User> userService)
    {
        _logger = logger;
        _userService = userService;
    }
    
    [Route("/about/terms")]
    public async Task<IActionResult> Terms()
    {
        var user = await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(user);
    }
    
    public async Task<IActionResult> Index()
    {
        var user = await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(user);
    }
    
    [Route("/about/privacy")]
    public async Task<IActionResult> Privacy()
    {
        var user = await _userService.GetAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(user);
    }
}