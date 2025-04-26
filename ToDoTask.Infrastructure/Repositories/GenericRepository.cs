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

    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public async Task<TEntity?> GetEntityAsync<TEntity>(Guid id, bool trackChanges = true) where TEntity : class
    {
        var query = _context.Set<TEntity>().Where(e => EF.Property<Guid>(e, "Id") == id);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    public async Task DeleteAsync<TEntity>(Guid id) where TEntity : class
    {
        await _context.Set<TEntity>().Where(e => EF.Property<Guid>(e, "Id") == id).ExecuteDeleteAsync();
    }

    public async Task<bool> EntityExistsAsync<TEntity>(Guid id) where TEntity : class
    {
        return await _context.Set<TEntity>().AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
