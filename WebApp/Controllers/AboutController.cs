using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class AboutController : Controller
{
    private readonly ILogger<AboutController> _logger;
    private readonly MongoDBService _mongoDBService;
    
    public AboutController(ILogger<AboutController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _mongoDBService = mongoDBService;
    }
    
    [Route("/about/terms")]
    public async Task<IActionResult> Terms()
    {
        var user = await _mongoDBService.GetByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(user);
    }
    
    public  IActionResult Index()
    {
        return View();
    }
}