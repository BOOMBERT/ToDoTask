using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Entities;
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

    public async Task<(IEnumerable<ToDoItem>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        DateTime? filterExpiryDateTimeUtcStart,
        DateTime? filterExpiryDateTimeUtcEnd)
    {
        var query = _context.ToDoItems.AsNoTracking();

        if (filterExpiryDateTimeUtcStart != null && filterExpiryDateTimeUtcEnd != null)
        {
            query = query.Where(t => t.ExpiryDateTimeUtc >= filterExpiryDateTimeUtcStart && t.ExpiryDateTimeUtc <= filterExpiryDateTimeUtcEnd);
        }

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var lowerSearchPhrase = searchPhrase.ToLower();

            query = query.Where(t => 
                t.Title.ToLower().Contains(lowerSearchPhrase) ||
                (!string.IsNullOrWhiteSpace(t.Description) && t.Description.ToLower().Contains(lowerSearchPhrase)));
        }

        if (!string.IsNullOrWhiteSpace(sortBy) && sortDirection != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<ToDoItem, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(ToDoItem.Title), t => t.Title },
                { nameof(ToDoItem.Description), t => t.Description },
                { nameof(ToDoItem.ExpiryDateTimeUtc), t => t.ExpiryDateTimeUtc },
                { nameof(ToDoItem.CompletionPercentage), t => t.CompletionPercentage }
            };

            if (columnsSelector.TryGetValue(sortBy, out var selectedColumn))
            {
                query = sortDirection == SortDirection.Ascending
                    ? query.OrderBy(selectedColumn)
                    : query.OrderByDescending(selectedColumn);
            }
        }

        var totalCount = await query.CountAsync();

        var toDoItems = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToArrayAsync();

        return (toDoItems, totalCount);
    }
}
