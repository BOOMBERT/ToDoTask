using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Entities;

namespace ToDoTask.Domain.Repositories;

public interface IToDoItemsRepository
{
    Task UpdateCompletionPercentageAsync(Guid id, decimal completionPercentage);
    Task<(IEnumerable<ToDoItem>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        DateTime? filterExpiryDateTimeUtcStart,
        DateTime? filterExpiryDateTimeUtcEnd);
}
