using Microsoft.EntityFrameworkCore;
using ToDoTask.Domain.Repositories;
using ToDoTask.Infrastructure.Persistence;

namespace ToDoTask.Infrastructure.Repositories;

public class ToDoItemsRepository : IToDoItemsRepository
{
    private readonly AppDbContext _context;

    public ToDoItemsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task UpdateCompletionPercentageAsync(Guid id, decimal completionPercentage) 
    {
        await _context.ToDoItems
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.CompletionPercentage, completionPercentage));
    }
}
