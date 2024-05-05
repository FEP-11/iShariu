using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IEntityService<User> _userService;

    public UserController(IEntityService<User> userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<List<User>> GetAsync() => await _userService.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetAsync(string id)
    {
        var user = await _userService.GetAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] User user)
    {
        await _userService.PostAsync(user);
        return CreatedAtAction(nameof(GetAsync), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(string id, [FromBody] User updatedUser)
    {
        await _userService.PutAsync(id, updatedUser);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}