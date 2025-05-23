﻿using MediatR;
using ToDoTask.Application.Common;
using ToDoTask.Application.Interfaces;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Domain.Constants;

namespace ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;

/// <summary>
/// Query for retrieving a paginated, optionally sorted and filtered list of ToDo items, along with pagination info.
/// </summary>
/// <remarks>
/// - SearchPhrase: Not required and cannot exceed 512 characters.
/// - PageNumber: Defaults to 1 and must be greater than or equal to 1.
/// - PageSize: Defaults to 5 and must be one of the allowed values: [5, 10, 20, 50].
/// - SortBy: Not required and must be null if SortDirection is null, or must be one of the allowed sortable columns when SortDirection is specified.
/// - SortDirection: Not required and must be provided if SortBy is provided, or must be null if SortBy is null.
/// - DateTimeRangeFilter: Not required and must be null if TimeZoneId is null, or must be one of the allowed values for DateTimeRange when TimeZoneId is specified.
/// - TimeZoneId: Not required and must be null if DateTimeRangeFilter is null, or must be a valid time zone ID (e.g., 'Europe/Warsaw', 'America/New_York') when DateTimeRangeFilter is specified. 
/// </remarks>
public class GetAllToDoItemsQuery : IRequest<PagedResponse<ToDoItemDto>>, IPaginationAndSortingQuery
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
    public DateTimeRange? DateTimeRangeFilter { get; set; }
    public string? TimeZoneId { get; set; }
}
