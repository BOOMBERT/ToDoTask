namespace ToDoTask.Domain.Repositories;

public interface IGenericRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool trackChanges = true) where TEntity : class;
    Task<bool> SaveChangesAsync();
}
