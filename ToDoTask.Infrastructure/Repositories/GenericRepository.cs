using Microsoft.EntityFrameworkCore;
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

    public async Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool trackChanges = true) where TEntity : class
    {
        var query = _context.Set<TEntity>().Where(e => EF.Property<Guid>(e, "Id") == id);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
