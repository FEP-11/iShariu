using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Services;

public class MongoDBService<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoDBService(IOptions<iShariuDatabaseSettings> dbSettings)
    {
        MongoClient client = new MongoClient(dbSettings.Value.ConnectionString);
        IMongoDatabase database = client.GetDatabase(dbSettings.Value.DatabaseName);

        _collection = database.GetCollection<T>(dbSettings.Value.CollectionName);
    }

    public async Task<List<T>> GetAsync() => await _collection.Find(new BsonDocument()).ToListAsync();

    public async Task<T> GetAsync(string id)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task PostAsync(T entity) => await _collection.InsertOneAsync(entity);

    public async Task PutAsync(string id)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        UpdateDefinition<T> update = Builders<T>.Update.AddToSet("Role", "user");
        await _collection.UpdateOneAsync(filter, update);
    }
    
    public async Task DeleteAsync(string id)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<List<T>> GetCreatorsAsync()
    {
        var filter = Builders<T>.Filter.Eq("Role", "creator");
        return await _collection.Find(filter).ToListAsync();
    }
}
