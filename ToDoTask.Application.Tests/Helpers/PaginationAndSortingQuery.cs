using ToDoTask.Application.Interfaces;
using ToDoTask.Domain.Constants;

namespace ToDoTask.Application.Tests.Helpers;

public class PaginationAndSortingQuery : IPaginationAndSortingQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}
