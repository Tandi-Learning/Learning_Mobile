namespace Yahtzee.Data.Interfaces;

public interface IRepository<T>
{
    Task<List<T>> ListAsync();
    // Optional: list by parent id. Default implementation returns empty list.
    Task<List<T>> ListAsync(int id)
    {
        return Task.FromResult(new List<T>());
    }
    Task<T?> GetAsync(int id);
    Task<int> SaveItemAsync(T item);
    Task<int> DeleteItemAsync(T item);
}
