using ToDoTask.Domain.Constants;

namespace ToDoTask.Application.Interfaces;

public interface IPaginationAndSortingQuery
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
    string? SortBy { get; set; }
    SortDirection? SortDirection { get; set; }
}
