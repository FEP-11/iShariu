using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApp.Models;
using WebApp.Services;

public class MongoDBService
{
    private readonly IEntityService<User> _userService;
    private readonly IEntityService<Course> _courseService;

    public MongoDBService(IOptions<iShariuDatabaseSettings> settings)
    {
        MongoClient client = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _userService = new EntityService<User>(database, settings.Value.UsersCollectionName);
        _courseService = new EntityService<Course>(database, "Course");
    }

    // User methods
    public Task<List<User>> GetUsersAsync() => _userService.GetAsync();
    public Task<User> GetUserAsync(string id) => _userService.GetAsync(id);
    public Task PostUserAsync(User user) => _userService.PostAsync(user);
    public Task PutUserAsync(string id, User updatedUser) => _userService.PutAsync(id, updatedUser);
    public Task DeleteUserAsync(string id) => _userService.DeleteAsync(id);

    // Course methods
    public Task<List<Course>> GetCoursesAsync() => _courseService.GetAsync();
    public Task<Course> GetCourseAsync(string id) => _courseService.GetAsync(id);
    public Task PostCourseAsync(Course course) => _courseService.PostAsync(course);
    public Task PutCourseAsync(string id, Course updatedCourse) => _courseService.PutAsync(id, updatedCourse);
    public Task DeleteCourseAsync(string id) => _courseService.DeleteAsync(id);
}
