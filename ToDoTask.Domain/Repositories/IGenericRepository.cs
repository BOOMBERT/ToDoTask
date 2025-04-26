namespace ToDoTask.Domain.Repositories;

public interface IGenericRepository
{
    Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
    Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool trackChanges = true) where TEntity : class;
    Task DeleteAsync<TEntity>(Guid id) where TEntity : class;
    Task<bool> EntityExistsAsync<TEntity>(Guid id) where TEntity : class;
    Task<bool> SaveChangesAsync();
}
