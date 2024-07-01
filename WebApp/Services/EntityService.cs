using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Services;

public class EntityService
{
    private readonly MongoDBService<User> _userService;
    private readonly MongoDBService<Course> _courseService;
    
    public EntityService(MongoDBService<User> userService, MongoDBService<Course> courseService, IOptions<iShariuDatabaseSettings> settings)
    {
        _userService = userService;
        _courseService = courseService;

        MongoClient client = new MongoClient(settings.Value.ConnectionString);
    }
    
    public async Task<List<User>> GetAllCreators()
    {
        var filter = Builders<User>.Filter.Eq("Role", UserRole.Creator);
        return await _userService.GetAsync(filter);
    }

    public async Task<List<Course>> GetBestSellingCoursesAsync()
    {
        FilterDefinition<Course> filter = Builders<Course>.Filter.Empty;
        Expression<Func<Course, object>> sortExpression = item => item.RevenueGenerated;
        var sort = Builders<Course>.Sort.Descending(sortExpression);
        return await _courseService.Collection.Find(filter).Sort(sort).Limit(3).ToListAsync();
    }
}