using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Services;

public class MongoDBService
{
    private readonly IMongoCollection<User> _userCollection;

    public MongoDBService(IOptions<iShariuDatabaseSettings> settings)
    {
        MongoClient client = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _userCollection = database.GetCollection<User>(settings.Value.UsersCollectionName);
    }

    public async Task PostAsync(User user) => await _userCollection.InsertOneAsync(user);

    public async Task<List<User>> GetAsync() => await _userCollection.Find(new BsonDocument()).ToListAsync();

    public async Task PutAsync(string id)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
        UpdateDefinition<User> update = Builders<User>.Update.AddToSet<string>("Role", "user");
        await _userCollection.UpdateOneAsync(filter, update);
    }
    
    public async Task DeleteAsync(string id)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
        await _userCollection.DeleteOneAsync(filter);
    }
}
