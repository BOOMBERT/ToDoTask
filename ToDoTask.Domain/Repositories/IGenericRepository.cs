namespace ToDoTask.Domain.Repositories;

public interface IGenericRepository
{
    Task AddAsync<T>(T entity) where T : class;
    Task<bool> SaveChangesAsync();
}
