using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly MongoDBService<User> _mongoDBService;

    public UserController(MongoDBService<User> mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpGet]
    public async Task<List<User>> GetAsync() => await _mongoDBService.GetAsync();

    [HttpGet("{id}")]
    public async Task<User> GetAsync(string id)
    {
        var user = await _mongoDBService.GetAsync(id);
        
        return user;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] User user)
    {
        await _mongoDBService.PostAsync(user);
        return CreatedAtAction("GetAsync", new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(User user)
    {
        await _mongoDBService.PutAsync(user);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await _mongoDBService.DeleteAsync(id);
        return NoContent();
    }
}