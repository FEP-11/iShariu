namespace WebApp.Services;

public interface IEntityService<T>
{
    Task<List<T>> GetAsync();
    Task<T> GetAsync(string id);
    Task PostAsync(T entity);
    Task PutAsync(string id, T updatedEntity);
    Task DeleteAsync(string id);
}