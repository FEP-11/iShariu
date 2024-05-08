using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Services;

public class EntityService
{
    private readonly MongoDBService<User> _userService;
    private readonly MongoDBService<Course> _courseService;
    
    public EntityService(MongoDBService<User> userService, MongoDBService<Course> courseService)
    {
        _userService = userService;
        _courseService = courseService;
    }
    
    public async Task<List<User>> GetAllCreators()
    {
        var filter = Builders<User>.Filter.Eq("Role", UserRole.Creator);
        return await _userService.GetAsync(filter);
    }

    public async Task<List<Course>> GetBestSellingCoursesAsync()
    {
        return await _courseService.GetBestSellingCoursesAsync();
    }
}