using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApp.Services;

public class EntityService<T> : IEntityService<T>
{
    private readonly IMongoCollection<T> _collection;

    public EntityService(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<List<T>> GetAsync() => await _collection.Find(new BsonDocument()).ToListAsync();

    public async Task<T> GetAsync(string id)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task PostAsync(T entity) => await _collection.InsertOneAsync(entity);

    public async Task PutAsync(string id, T updatedEntity)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.ReplaceOneAsync(filter, updatedEntity);
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }
}