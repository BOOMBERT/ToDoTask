using ToDoTask.Domain.Repositories;
using ToDoTask.Infrastructure.Persistence;

namespace ToDoTask.Infrastructure.Repositories;

public class GenericRepository : IGenericRepository
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
