using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public UserController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpGet]
    public async Task<List<User>> GetAsync() => await _mongoDBService.GetAsync();

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] User user)
    {
        await _mongoDBService.PostAsync(user);
        return CreatedAtAction("GetAsync", new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(string id)
    {
        await _mongoDBService.PutAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await _mongoDBService.DeleteUserAsync(id);
        return NoContent();
    }
}
