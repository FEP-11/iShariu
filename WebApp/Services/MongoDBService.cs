using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Course> _courseCollection;

        public MongoDBService(IOptions<iShariuDatabaseSettings> settings)
        {
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(settings.Value.UsersCollectionName);
            _courseCollection = database.GetCollection<Course>("Course"); 
        }

        // User methods
        public async Task<List<User>> GetAsync() => await _userCollection.Find(new BsonDocument()).ToListAsync();

        public async Task<User> GetAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _userCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task PostAsync(User user) => await _userCollection.InsertOneAsync(user);

        public async Task PutAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            UpdateDefinition<User> update = Builders<User>.Update.AddToSet<string>("Role", "user");
            await _userCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteUserAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _userCollection.DeleteOneAsync(filter);
        }

        public async Task<List<User>> GetAllCreators()
        {
            var filter = Builders<User>.Filter.Eq("Role", "creator");
            return await _userCollection.Find(filter).Limit(10).ToListAsync();
        }

        // Course methods
        public async Task<List<Course>> GetCoursesAsync() => await _courseCollection.Find(new BsonDocument()).ToListAsync();

        public async Task<Course> GetCourseAsync(string id)
        {
            FilterDefinition<Course> filter = Builders<Course>.Filter.Eq(c => c.Id, id);
            return await _courseCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task PostCourseAsync(Course course) => await _courseCollection.InsertOneAsync(course);

        public async Task PutCourseAsync(string id, Course updatedCourse)
        {
            FilterDefinition<Course> filter = Builders<Course>.Filter.Eq("Id", id);
            await _courseCollection.ReplaceOneAsync(filter, updatedCourse);
        }

        public async Task DeleteCourseAsync(string id)
        {
            FilterDefinition<Course> filter = Builders<Course>.Filter.Eq("Id", id);
            await _courseCollection.DeleteOneAsync(filter);
        }

        public async Task<List<Course>> GetBestSellingCoursesAsync()
        {
            var filter = Builders<Course>.Filter.Empty;
            var sort = Builders<Course>.Sort.Descending(c => c.RevenueGenerated);
            return await _courseCollection.Find(filter).Sort(sort).Limit(10).ToListAsync();
        }
        
        public async Task PutAsync(string id, User updatedUser)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _userCollection.ReplaceOneAsync(filter, updatedUser);
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Username, username);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<User> GetByIdAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }
        
    }
}