using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;

namespace WebApp.Services
{
    public class MongoDBService<T> where T : IIdentifiable
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<iShariuDatabaseSettings> settings)
        {
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);

            string collectionName = typeof(T) == typeof(User) ? settings.Value.UsersCollectionName : settings.Value.CoursesCollectionName;
            _collection = database.GetCollection<T>(collectionName);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            // Create collections if they don't exist
            CreateCollectionIfNotExists(settings.Value.UsersCollectionName);
            CreateCollectionIfNotExists(settings.Value.CoursesCollectionName);
        }
        
        public async Task<List<T>> GetAsync() => await _collection.Find(new BsonDocument()).ToListAsync();
        
        public async Task<T> GetAsync(string id)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAsync(FilterDefinition<T> filter) => await _collection.Find(filter).Limit(10).ToListAsync();
        
        public async Task PostAsync(T item) => await _collection.InsertOneAsync(item);

        public async Task PutAsync(T item)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", item.Id);
            await _collection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }
        
        public async Task<List<T>> GetBestSellingCoursesAsync()
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;

            if (typeof(T) == typeof(Course))
            {
                Expression<Func<T, object>> sortExpression = item => ((Course)(object)item).RevenueGenerated;
                var sort = Builders<T>.Sort.Descending(sortExpression);
                return await _collection.Find(filter).Limit(3).Sort(sort).ToListAsync();
            }
            else
            {
                return await _collection.Find(filter).ToListAsync();
            }
        }
        
        [NonAction]
        private void CreateCollectionIfNotExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionsOptions { Filter = filter };
            bool exists = _database.ListCollections(options).Any();

            if (!exists) _database.CreateCollection(collectionName);
        }
    }
}