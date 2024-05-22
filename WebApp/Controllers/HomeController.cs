using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly MongoDBService<User> _userCollection;
    private readonly MongoDBService<Message> _messageService;

    public HomeController(MongoDBService<User> userCollection, MongoDBService<Message> messageService)
    {
        _userCollection = userCollection;
        _messageService = messageService;
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
    
    [HttpPost]
    public async Task<IActionResult> SubmitMessage(Message message)
    {
        if (message == null || string.IsNullOrEmpty(message.Content))
            return BadRequest("suck dick");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        message.SkibidiMan = userId;
        
        await _messageService.PostAsync(message);
        return RedirectToAction("Index");
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
